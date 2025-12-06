import { useState, useEffect, useMemo } from "react";
import { useSearchParams } from "react-router-dom";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import Tooltip from "@/components/ui/tooltip";
import { resetPassword } from "@/lib/auth/api/requests";
import useNavigate from "@/hooks/useNavigate";

export default function ResetPasswordPage() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [token, setToken] = useState<string | null>(null);
    const [password, setPassword] = useState("");
    const [confirm, setConfirm] = useState("");
    const [passwordError, setPasswordError] = useState<string | null>(null);
    const [confirmError, setConfirmError] = useState<string | null>(null);
    const [generalError, setGeneralError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const tokenParam = searchParams.get("token");
        if (!tokenParam) {
            setGeneralError("No reset token provided. Please use the link from your email.");
        } else {
            setToken(tokenParam);
        }
    }, [searchParams]);

    function scorePassword(pw: string) {
        let score = 0;
        if (pw.length >= 8) score++;
        if (/[A-Z]/.test(pw)) score++;
        if (/[0-9]/.test(pw)) score++;
        if (/[^A-Za-z0-9]/.test(pw)) score++;
        return Math.min(score, 4);
    }

    const strength = useMemo(() => scorePassword(password), [password]);
    const strengthLabel = ["Very weak", "Weak", "Fair", "Good", "Strong"][strength];

    const allFilled = password !== "" && confirm !== "";
    const passwordsMatch = password === confirm;
    const passwordTooWeak = strength < 2;
    const canSubmit = allFilled && passwordsMatch && !passwordTooWeak && !loading && token;

    let tooltipMessage: string | null = null;
    if (!token) {
        tooltipMessage = "Invalid reset token";
    } else if (!allFilled) {
        tooltipMessage = "All fields must be filled out";
    } else if (!passwordsMatch) {
        tooltipMessage = "Passwords do not match";
    } else if (passwordTooWeak) {
        tooltipMessage = "Password is too weak";
    }

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setPasswordError(null);
        setConfirmError(null);
        setGeneralError(null);
        setSuccess(null);

        if (!token) {
            setGeneralError("Invalid reset token");
            return;
        }

        if (!password) {
            setPasswordError("Password is required");
            return;
        }

        if (!confirm) {
            setConfirmError("Please confirm your password");
            return;
        }

        if (password !== confirm) {
            setConfirmError("Passwords do not match");
            return;
        }

        if (scorePassword(password) < 2) {
            setPasswordError("Password is too weak");
            return;
        }

        setLoading(true);
        try {
            const response = await resetPassword(token, password);
            setSuccess(response.message || "Password reset successfully!");

            // Redirect to login after 2 seconds
            setTimeout(() => {
                navigate("/login");
            }, 2000);
        } catch (err: any) {
            setGeneralError(err?.message || "Failed to reset password. The token may be invalid or expired.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-background">
            <Card className="w-[420px]">
                <CardHeader>
                    <CardTitle className="text-center">Reset Password</CardTitle>
                </CardHeader>
                <CardContent>
                    {!token ? (
                        <div className="text-center space-y-4">
                            <div className="text-red-600 text-5xl mb-4">✗</div>
                            <p className="text-muted-foreground">{generalError}</p>
                            <Button onClick={() => navigate("/login")} className="w-full">
                                Back to Login
                            </Button>
                        </div>
                    ) : (
                        <form className="space-y-4" onSubmit={handleSubmit}>
                            <div>
                                <Input
                                    label="New Password"
                                    type="password"
                                    placeholder="Choose a password"
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    error={passwordError}
                                />
                                {password ? (
                                    <div className="mt-2">
                                        <div className="h-2 w-full bg-gray-200 rounded">
                                            <div
                                                className={`h-2 rounded ${strength <= 1
                                                        ? "bg-red-500"
                                                        : strength === 2
                                                            ? "bg-yellow-400"
                                                            : strength === 3
                                                                ? "bg-amber-400"
                                                                : "bg-green-500"
                                                    }`}
                                                style={{ width: `${(strength / 4) * 100}%` }}
                                            />
                                        </div>
                                        <p className="text-xs mt-1 text-muted-foreground">
                                            Strength: {strengthLabel}
                                        </p>
                                    </div>
                                ) : null}
                            </div>

                            <Input
                                label="Confirm Password"
                                type="password"
                                placeholder="Confirm your password"
                                value={confirm}
                                onChange={(e) => setConfirm(e.target.value)}
                                error={confirmError}
                            />

                            {generalError ? <p className="text-sm text-red-600">{generalError}</p> : null}
                            {success ? (
                                <div className="text-center space-y-2">
                                    <p className="text-sm text-green-600">{success}</p>
                                    <p className="text-xs text-muted-foreground">Redirecting to login...</p>
                                </div>
                            ) : null}

                            <div className="flex items-center justify-between">
                                {tooltipMessage ? (
                                    <Tooltip message={tooltipMessage} placement="south">
                                        <span className="inline-block">
                                            <Button type="submit" disabled={!canSubmit}>
                                                {loading ? "Resetting..." : "Reset Password"}
                                            </Button>
                                        </span>
                                    </Tooltip>
                                ) : (
                                    <Button type="submit" disabled={!canSubmit}>
                                        {loading ? "Resetting..." : "Reset Password"}
                                    </Button>
                                )}
                                <Button
                                    variant="link"
                                    size="sm"
                                    type="button"
                                    onClick={() => navigate("/login")}
                                >
                                    Back to Login
                                </Button>
                            </div>
                        </form>
                    )}
                </CardContent>
            </Card>
        </div>
    );
}
