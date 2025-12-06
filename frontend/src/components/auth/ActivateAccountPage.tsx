import { useEffect, useState, useRef } from "react";
import { useSearchParams } from "react-router-dom";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { activateAccount } from "@/lib/auth/api/requests";
import useNavigate from "@/hooks/useNavigate";

export default function ActivateAccountPage() {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [status, setStatus] = useState<"loading" | "success" | "error">("loading");
    const [message, setMessage] = useState("");
    const hasActivated = useRef(false);

    useEffect(() => {
        // Prevent duplicate activation calls
        if (hasActivated.current) return;

        const token = searchParams.get("token");

        if (!token) {
            setStatus("error");
            setMessage("No activation token provided.");
            return;
        }

        hasActivated.current = true;

        // Activate account
        activateAccount(token)
            .then((response) => {
                setStatus("success");
                setMessage(response.message || "Account activated successfully!");
            })
            .catch((error) => {
                setStatus("error");
                setMessage(error.message || "Failed to activate account. The token may be invalid or expired.");
            });
    }, [searchParams]);

    return (
        <div className="min-h-screen flex items-center justify-center bg-background">
            <Card className="w-[420px]">
                <CardHeader>
                    <CardTitle className="text-center">Account Activation</CardTitle>
                </CardHeader>
                <CardContent className="text-center space-y-4">
                    {status === "loading" && (
                        <div>
                            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto mb-4" />
                            <p className="text-muted-foreground">Activating your account...</p>
                        </div>
                    )}

                    {status === "success" && (
                        <div>
                            <div className="text-green-600 text-5xl mb-4">✓</div>
                            <p className="text-lg font-semibold mb-2">Success!</p>
                            <p className="text-muted-foreground mb-6">{message}</p>
                            <Button onClick={() => navigate("/login")} className="w-full">
                                Go to Login
                            </Button>
                        </div>
                    )}

                    {status === "error" && (
                        <div>
                            <div className="text-red-600 text-5xl mb-4">✗</div>
                            <p className="text-lg font-semibold mb-2">Activation Failed</p>
                            <p className="text-muted-foreground mb-6">{message}</p>
                            <div className="space-y-2">
                                <Button onClick={() => navigate("/register")} className="w-full">
                                    Create New Account
                                </Button>
                                <Button onClick={() => navigate("/login")} variant="outline" className="w-full">
                                    Back to Login
                                </Button>
                            </div>
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
}
