#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Track process IDs
EMAIL_PID=""
SERVER_PID=""
FRONTEND_PID=""

# Cleanup function to kill all processes
cleanup() {
    echo -e "\n${YELLOW}Shutting down Nautilus...${NC}"
    
    if [ ! -z "$EMAIL_PID" ]; then
        kill $EMAIL_PID 2>/dev/null
    fi
    
    if [ ! -z "$SERVER_PID" ]; then
        kill $SERVER_PID 2>/dev/null
    fi
    
    if [ ! -z "$FRONTEND_PID" ]; then
        kill $FRONTEND_PID 2>/dev/null
    fi
    
    # Kill any remaining node/dotnet processes on our ports
    lsof -ti:3001 | xargs kill -9 2>/dev/null
    lsof -ti:5106 | xargs kill -9 2>/dev/null
    lsof -ti:5173 | xargs kill -9 2>/dev/null
    
    echo -e "${GREEN}Nautilus stopped.${NC}"
    exit 0
}

# Set up trap to catch Ctrl+C
trap cleanup SIGINT SIGTERM

echo -e "${BLUE}╔═══════════════════════════════════════╗${NC}"
echo -e "${BLUE}║     Starting Nautilus Development    ║${NC}"
echo -e "${BLUE}╔═══════════════════════════════════════╗${NC}\n"

# Start Email Service
echo -e "${YELLOW}Starting Email Service...${NC}"
cd email
npm run dev > ../logs/email.log 2>&1 &
EMAIL_PID=$!
cd ..

# Wait and check if email service started
sleep 2
if ! kill -0 $EMAIL_PID 2>/dev/null; then
    echo -e "${RED}✗ Email service failed to start${NC}"
    cat logs/email.log
    cleanup
fi
echo -e "${GREEN}✓ Email service running (PID: $EMAIL_PID)${NC}"

# Start .NET API
echo -e "${YELLOW}Starting .NET API...${NC}"
cd server/Nautilus.Api
dotnet run > ../../logs/server.log 2>&1 &
SERVER_PID=$!
cd ../..

# Wait and check if server started
sleep 3
if ! kill -0 $SERVER_PID 2>/dev/null; then
    echo -e "${RED}✗ .NET API failed to start${NC}"
    cat logs/server.log
    cleanup
fi
echo -e "${GREEN}✓ .NET API running (PID: $SERVER_PID)${NC}"

# Start Frontend
echo -e "${YELLOW}Starting Frontend...${NC}"
cd frontend
npm run dev > ../logs/frontend.log 2>&1 &
FRONTEND_PID=$!
cd ..

# Wait and check if frontend started
sleep 3
if ! kill -0 $FRONTEND_PID 2>/dev/null; then
    echo -e "${RED}✗ Frontend failed to start${NC}"
    cat logs/frontend.log
    cleanup
fi
echo -e "${GREEN}✓ Frontend running (PID: $FRONTEND_PID)${NC}\n"

# Success message
echo -e "${GREEN}╔═══════════════════════════════════════╗${NC}"
echo -e "${GREEN}║    🚀 Nautilus Running Successfully!  ║${NC}"
echo -e "${GREEN}╔═══════════════════════════════════════╗${NC}\n"

echo -e "${BLUE}Services:${NC}"
echo -e "  📧 Email Service:  ${BLUE}http://localhost:3001${NC}"
echo -e "  🔧 .NET API:       ${BLUE}http://localhost:5106${NC}"
echo -e "  🌐 Frontend:       ${BLUE}http://localhost:5173${NC}"

echo -e "\n${YELLOW}Press Ctrl+C to stop all services${NC}\n"

# Keep script running and monitor processes
while true; do
    # Check if any process has died
    if ! kill -0 $EMAIL_PID 2>/dev/null; then
        echo -e "${RED}✗ Email service died unexpectedly${NC}"
        cat logs/email.log
        cleanup
    fi
    
    if ! kill -0 $SERVER_PID 2>/dev/null; then
        echo -e "${RED}✗ .NET API died unexpectedly${NC}"
        cat logs/server.log
        cleanup
    fi
    
    if ! kill -0 $FRONTEND_PID 2>/dev/null; then
        echo -e "${RED}✗ Frontend died unexpectedly${NC}"
        cat logs/frontend.log
        cleanup
    fi
    
    sleep 2
done
