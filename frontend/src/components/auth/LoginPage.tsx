import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useState } from "react";
import { login } from "@/lib/auth/api/requests";
import useNavigate from "@/hooks/useNavigate";
import { useAtom } from "jotai";
import { usernameAtom, emailAtom, passwordAtom, userGuidAtom } from "@/atoms/auth";

export default function LoginPage() {
    const navigate = useNavigate()
    const [mode, setMode] = useState<"username" | "email">("username");
    const [username, setUsername] = useAtom(usernameAtom)
    const [email, setEmail] = useAtom(emailAtom)
    const [password, setPassword] = useAtom(passwordAtom)
    const [, setUserGuid] = useAtom(userGuidAtom)
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
                                if (!username.trim()) setIdentifierError("Username is required")
                            } else {
                                if (!email.trim()) setIdentifierError("Email is required")
                            }

                            if (!password) {
                                setPasswordError("Password is required")
                            }

                            const id = mode === "username" ? username.trim() : email.trim()
                            if (!id || !password) return

                            setLoading(true)
                            try {
                                const result = await login(id, password);
                                setSuccess("Signed in successfully")
                                const userGuid = result?.userId
                                setUserGuid(userGuid);
                                if (userGuid) {
                                    navigate(`/home/${userGuid}?user=${userGuid}`)
                                } else {
                                    setGeneralError("Could not get user ID from server response.")
                                }
                            } catch (err: any) {
                                setGeneralError(err?.message || "Sign in failed")
                            } finally {
                                setLoading(false)
                            }
                        }}
                    >
                        {mode === "username" ? (
                            <div>
                                <Input label="Username" type="text" placeholder="your-username" value={username} onChange={(e) => setUsername(e.target.value)} error={identifierError} />
                                <div className="mt-2">
                                    <Button variant="link" size="sm" type="button" onClick={() => setMode("email")}>Use email instead</Button>
                                </div>
                            </div>
                        ) : (
                            <div>
                                <Input label="Email" type="email" placeholder="you@example.com" value={email} onChange={(e) => setEmail(e.target.value)} error={identifierError} />
                                <div className="mt-2">
                                    <Button variant="link" size="sm" type="button" onClick={() => setMode("username")}>Use username instead</Button>
                                </div>
                            </div>
                        )}

                        <Input label="Password" type="password" placeholder="Your password" value={password} onChange={(e) => setPassword(e.target.value)} error={passwordError} />

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
