# Nautilus Local Development Launcher
# Run all services: Email, API, and Frontend

$ErrorActionPreference = "Stop"

# Color functions
function Write-Success { param($msg) Write-Host $msg -ForegroundColor Green }
function Write-Error-Msg { param($msg) Write-Host $msg -ForegroundColor Red }
function Write-Info { param($msg) Write-Host $msg -ForegroundColor Yellow }
function Write-Blue { param($msg) Write-Host $msg -ForegroundColor Cyan }

# Track jobs
$jobs = @()

# Cleanup function
function Cleanup {
    Write-Info "`nShutting down Nautilus..."
    
    # Stop all background jobs
    Get-Job | Stop-Job
    Get-Job | Remove-Job
    
    # Kill processes on our ports
    $ports = @(3001, 5106, 5173)
    foreach ($port in $ports) {
        $connections = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
        foreach ($conn in $connections) {
            Stop-Process -Id $conn.OwningProcess -Force -ErrorAction SilentlyContinue
        }
    }
    
    Write-Success "Nautilus stopped."
    exit
}

# Register Ctrl+C handler
$null = Register-EngineEvent -SourceIdentifier PowerShell.Exiting -Action { Cleanup }

Write-Blue "========================================"
Write-Blue "     Starting Nautilus Development     "
Write-Blue "========================================"
Write-Host ""

# Create logs directory if it doesn't exist
if (-not (Test-Path "logs")) {
    New-Item -ItemType Directory -Path "logs" | Out-Null
}

# Start Email Service
Write-Info "Starting Email Service..."
try {
    $emailJob = Start-Job -ScriptBlock {
        Set-Location $using:PWD\email
        npm run dev 2>&1
    }
    $jobs += $emailJob
    Start-Sleep -Seconds 2
    
    if ($emailJob.State -eq "Failed") {
        throw "Email service failed to start"
    }
    Write-Success "[OK] Email service running"
} catch {
    Write-Error-Msg "[FAIL] Email service failed to start"
    Receive-Job $emailJob
    Cleanup
}

# Start .NET API
Write-Info "Starting .NET API..."
try {
    $serverJob = Start-Job -ScriptBlock {
        Set-Location $using:PWD\server\Nautilus.Api
        dotnet run 2>&1
    }
    $jobs += $serverJob
    Start-Sleep -Seconds 3
    
    if ($serverJob.State -eq "Failed") {
        throw ".NET API failed to start"
    }
    Write-Success "[OK] .NET API running"
} catch {
    Write-Error-Msg "[FAIL] .NET API failed to start"
    Receive-Job $serverJob
    Cleanup
}

# Start Frontend
Write-Info "Starting Frontend..."
try {
    $frontendJob = Start-Job -ScriptBlock {
        Set-Location $using:PWD\frontend
        npm run dev 2>&1
    }
    $jobs += $frontendJob
    Start-Sleep -Seconds 3
    
    if ($frontendJob.State -eq "Failed") {
        throw "Frontend failed to start"
    }
    Write-Success "[OK] Frontend running"
} catch {
    Write-Error-Msg "[FAIL] Frontend failed to start"
    Receive-Job $frontendJob
    Cleanup
}

Write-Host ""
Write-Success "========================================"
Write-Success "   Nautilus Running Successfully!      "
Write-Success "========================================"
Write-Host ""

Write-Blue "Services:"
Write-Host "  Email Service:  " -NoNewline
Write-Blue "http://localhost:3001"
Write-Host "  .NET API:       " -NoNewline
Write-Blue "http://localhost:5106"
Write-Host "  Frontend:       " -NoNewline
Write-Blue "http://localhost:5173"

Write-Host ""
Write-Info "Press Ctrl+C to stop all services"
Write-Host ""

# Monitor jobs and stream output
try {
    while ($true) {
        # Check if any job failed
        foreach ($job in $jobs) {
            if ($job.State -eq "Failed") {
                Write-Error-Msg "`n[FAIL] A service died unexpectedly"
                Receive-Job $job
                Cleanup
            }
        }
        
        # Stream output from jobs
        foreach ($job in $jobs) {
            Receive-Job $job | ForEach-Object { Write-Host $_ }
        }
        
        Start-Sleep -Seconds 1
    }
} finally {
    Cleanup
}
