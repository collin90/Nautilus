import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useState } from "react";
import { login } from "@/lib/auth/api/requests";
import useNavigate from "@/hooks/useNavigate";
import { useSetAtom } from "jotai";
import { usernameAtom, emailAtom, userGuidAtom } from "@/atoms/auth";
import { setToken } from "@/lib/auth/tokenStorage";
import { getUserFromToken } from "@/lib/auth/jwtUtils";

export default function LoginPage() {
    const navigate = useNavigate()
    const [mode, setMode] = useState<"username" | "email">("username");
    // Local state for form inputs
    const [usernameInput, setUsernameInput] = useState("")
    const [emailInput, setEmailInput] = useState("")
    const [passwordInput, setPasswordInput] = useState("")
    // Global setters (only set after successful login)
    const setUsername = useSetAtom(usernameAtom)
    const setEmail = useSetAtom(emailAtom)
    const setUserGuid = useSetAtom(userGuidAtom)
    const [identifierError, setIdentifierError] = useState<string | null>(null)
    const [passwordError, setPasswordError] = useState<string | null>(null)
    const [generalError, setGeneralError] = useState<string | null>(null)
    const [loading, setLoading] = useState(false)
    const [success, setSuccess] = useState<string | null>(null)

    return (
        <div className="min-h-screen flex items-center justify-center bg-background">
            <Card className="w-[380px]">
                <CardHeader>
                    <CardTitle className="text-center">Sign in</CardTitle>
                </CardHeader>
                <CardContent>
                    <form
                        className="space-y-4"
                        onSubmit={async (e) => {
                            e.preventDefault();
                            setIdentifierError(null)
                            setPasswordError(null)
                            setGeneralError(null)
                            setSuccess(null)

                            // Validate identifier (username or email)
                            if (mode === "username") {
                                if (!usernameInput.trim()) setIdentifierError("Username is required")
                            } else {
                                if (!emailInput.trim()) setIdentifierError("Email is required")
                            }

                            if (!passwordInput) {
                                setPasswordError("Password is required")
                            }

                            const id = mode === "username" ? usernameInput.trim() : emailInput.trim()
                            if (!id || !passwordInput) return

                            setLoading(true)
                            try {
                                const result = await login(id, passwordInput);

                                // Store JWT token
                                if (!result?.token) {
                                    setGeneralError("No token received from server.");
                                    return;
                                }

                                setToken(result.token);

                                // Extract user info from JWT token
                                const userInfo = getUserFromToken(result.token);
                                if (!userInfo) {
                                    setGeneralError("Failed to decode token.");
                                    return;
                                }

                                // Set user info from JWT claims
                                setUsername(userInfo.username);
                                setEmail(userInfo.email);
                                setUserGuid(userInfo.userId);

                                setSuccess("Signed in successfully");
                                navigate("/home");
                            } catch (err: any) {
                                setGeneralError(err?.message || "Sign in failed")
                            } finally {
                                setLoading(false)
                            }
                        }}
                    >
                        {mode === "username" ? (
                            <div>
                                <Input label="Username" type="text" placeholder="your-username" value={usernameInput} onChange={(e) => setUsernameInput(e.target.value)} error={identifierError} />
                                <div className="mt-2">
                                    <Button variant="link" size="sm" type="button" onClick={() => setMode("email")}>Use email instead</Button>
                                </div>
                            </div>
                        ) : (
                            <div>
                                <Input label="Email" type="email" placeholder="you@example.com" value={emailInput} onChange={(e) => setEmailInput(e.target.value)} error={identifierError} />
                                <div className="mt-2">
                                    <Button variant="link" size="sm" type="button" onClick={() => setMode("username")}>Use username instead</Button>
                                </div>
                            </div>
                        )}

                        <Input label="Password" type="password" placeholder="Your password" value={passwordInput} onChange={(e) => setPasswordInput(e.target.value)} error={passwordError} />

                        <div className="text-right">
                            <Button variant="link" size="sm" type="button" onClick={() => navigate("/request-reset")}>
                                Forgot password?
                            </Button>
                        </div>

                        {generalError ? <p className="text-sm text-red-600">{generalError}</p> : null}
                        {success ? <p className="text-sm text-green-600">{success}</p> : null}

                        <div className="flex items-center justify-between">
                            <Button type="submit" disabled={loading}>{loading ? "Signing in..." : "Sign in"}</Button>
                            <Button variant="link" size="sm" type="button" onClick={() => navigate("/register")}>
                                Create new account
                            </Button>
                        </div>
                    </form>
                </CardContent>
            </Card>
        </div>
    );
}
