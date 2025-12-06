import { useState } from "react";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { requestPasswordReset } from "@/lib/auth/api/requests";
import useNavigate from "@/hooks/useNavigate";

export default function RequestPasswordResetPage() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [emailError, setEmailError] = useState<string | null>(null);
    const [generalError, setGeneralError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setEmailError(null);
        setGeneralError(null);
        setSuccess(null);

        if (!email.trim()) {
            setEmailError("Email is required");
            return;
        }

        setLoading(true);
        try {
            const response = await requestPasswordReset(email.trim());
            setSuccess(response.message || "If an account with that email exists, a password reset link has been sent.");
            setEmail("");
        } catch (err: any) {
            setGeneralError(err?.message || "Failed to send password reset email.");
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
                    <form className="space-y-4" onSubmit={handleSubmit}>
                        <p className="text-sm text-muted-foreground">
                            Enter your email address and we'll send you a link to reset your password.
                        </p>

                        <Input
                            label="Email"
                            type="email"
                            placeholder="you@example.com"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            error={emailError}
                        />

                        {generalError ? <p className="text-sm text-red-600">{generalError}</p> : null}
                        {success ? <p className="text-sm text-green-600">{success}</p> : null}

                        <div className="flex items-center justify-between">
                            <Button type="submit" disabled={loading || !email.trim()}>
                                {loading ? "Sending..." : "Send Reset Link"}
                            </Button>
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
                </CardContent>
            </Card>
        </div>
    );
}
