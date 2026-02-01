import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import Tooltip from "@/components/ui/tooltip";
import { useState, useMemo } from "react";
import { register as apiRegister, resendActivationEmail } from "@/lib/auth/api/requests";
import useNavigate from "@/hooks/useNavigate";

export default function RegisterPage() {
    const navigate = useNavigate()
    const [username, setUsername] = useState("")
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")
    const [confirm, setConfirm] = useState("")

    const [usernameError, setUsernameError] = useState<string | null>(null)
    const [emailError, setEmailError] = useState<string | null>(null)
    const [passwordError, setPasswordError] = useState<string | null>(null)
    const [confirmError, setConfirmError] = useState<string | null>(null)
    const [generalError, setGeneralError] = useState<string | null>(null)
    const [success, setSuccess] = useState<string | null>(null)
    const [registeredEmail, setRegisteredEmail] = useState<string | null>(null)
    const [loading, setLoading] = useState(false)
    const [resending, setResending] = useState(false)

    function scorePassword(pw: string) {
        let score = 0
        if (pw.length >= 8) score++
        if (/[A-Z]/.test(pw)) score++
        if (/[0-9]/.test(pw)) score++
        if (/[^A-Za-z0-9]/.test(pw)) score++
        return Math.min(score, 4)
    }

    const strength = useMemo(() => scorePassword(password), [password])
    const strengthLabel = ["Very weak", "Weak", "Fair", "Good", "Strong"][strength]

    const allFilled = username.trim() !== "" && email.trim() !== "" && password !== "" && confirm !== ""
    const passwordsMatch = password === confirm
    const passwordTooWeak = strength < 2
    const canSubmit = allFilled && passwordsMatch && !passwordTooWeak && !loading
    let tooltipMessage: string | null = null
    if (!allFilled) {
        tooltipMessage = "All fields must be filled out to create new account"
    } else if (!passwordsMatch) {
        tooltipMessage = "Passwords do not match"
    } else if (passwordTooWeak) {
        tooltipMessage = "Password is too weak to create account"
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-background">
            <Card className="w-[420px]">
                <CardHeader>
                    <CardTitle className="text-center">Create account</CardTitle>
                </CardHeader>
                <CardContent>
                    <form
                        className="space-y-4"
                        onSubmit={async (e) => {
                            e.preventDefault()
                            setUsernameError(null)
                            setEmailError(null)
                            setPasswordError(null)
                            setConfirmError(null)
                            setGeneralError(null)
                            setSuccess(null)

                            if (!username.trim()) setUsernameError("Username is required")
                            if (!email.trim()) setEmailError("Email is required")
                            if (!password) setPasswordError("Password is required")
                            if (!confirm) setConfirmError("Please confirm your password")
                            if (password && confirm && password !== confirm) setConfirmError("Passwords do not match")

                            // enforce minimal strength: score >= 2
                            if (password && scorePassword(password) < 2) setPasswordError("Password is too weak")

                            if (!username.trim() || !email.trim() || !password || !confirm || (password !== confirm) || (password && scorePassword(password) < 2)) {
                                return
                            }

                            setLoading(true)
                            try {
                                await apiRegister(username.trim(), email.trim(), password)
                                setRegisteredEmail(email.trim())
                                setSuccess("Account created! Please check your email to activate your account.")
                                // Clear form
                                setUsername("")
                                setEmail("")
                                setPassword("")
                                setConfirm("")
                            } catch (err: any) {
                                setGeneralError(err?.message || "Registration failed")
                            } finally {
                                setLoading(false)
                            }
                        }}
                    >
                        <Input label="Username" type="text" placeholder="choose-a-username" value={username} onChange={(e) => setUsername(e.target.value)} error={usernameError} />

                        <Input label="Email" type="email" placeholder="you@example.com" value={email} onChange={(e) => setEmail(e.target.value)} error={emailError} />

                        <div>
                            <Input label="Password" type="password" placeholder="Choose a password" value={password} onChange={(e) => setPassword(e.target.value)} error={passwordError} />
                            {password ? (
                                <div className="mt-2">
                                    <div className="h-2 w-full bg-gray-200 rounded">
                                        <div className={`h-2 rounded ${strength <= 1 ? 'bg-red-500' : strength === 2 ? 'bg-yellow-400' : strength === 3 ? 'bg-amber-400' : 'bg-green-500'}`} style={{ width: `${(strength / 4) * 100}%` }} />
                                    </div>
                                    <p className="text-xs mt-1 text-muted-foreground">Strength: {strengthLabel}</p>
                                </div>
                            ) : null}
                        </div>

                        <Input label="Confirm password" type="password" placeholder="Confirm password" value={confirm} onChange={(e) => setConfirm(e.target.value)} error={confirmError} />
                        {(confirm.length > 0 && confirm !== password) && (
                            <p className="text-sm text-red-600">
                                Passwords do not match!
                            </p>
                        )}
                        {generalError ? <p className="text-sm text-red-600">{generalError}</p> : null}
                        {success ? (
                            <div className="space-y-3 p-4 bg-green-50 border border-green-200 rounded-md">
                                <p className="text-sm text-green-800 font-medium">{success}</p>
                                <p className="text-xs text-green-700">
                                    We've sent an activation link to <strong>{registeredEmail}</strong>.
                                    Click the link in the email to activate your account.
                                </p>
                                <p className="text-xs text-green-600">
                                    💡 Don't see the email? Check your spam folder.
                                </p>
                                <Button
                                    variant="outline"
                                    size="sm"
                                    type="button"
                                    onClick={async () => {
                                        if (!registeredEmail) return
                                        setResending(true)
                                        try {
                                            await resendActivationEmail(registeredEmail)
                                            setSuccess("Account created! A new activation email has been sent.")
                                        } catch (err: any) {
                                            setGeneralError("Failed to resend activation email. Please try again later.")
                                        } finally {
                                            setResending(false)
                                        }
                                    }}
                                    disabled={resending}
                                    className="w-full"
                                >
                                    {resending ? "Sending..." : "Resend activation email"}
                                </Button>
                            </div>
                        ) : null}

                        <div className="flex items-center justify-between">
                            {tooltipMessage ? (
                                <Tooltip message={tooltipMessage} placement="south">
                                    <span className="inline-block">
                                        <Button type="submit" disabled={!canSubmit}>{loading ? 'Creating...' : 'Create account'}</Button>
                                    </span>
                                </Tooltip>
                            ) : (
                                <Button type="submit" disabled={!canSubmit}>{loading ? 'Creating...' : 'Create account'}</Button>
                            )}
                            <Button
                                variant="link"
                                size="sm"
                                type="button"
                                onClick={() => navigate("/login")}
                            >
                                Back to sign in
                            </Button>
                        </div>
                    </form>
                </CardContent>
            </Card>
        </div>
    );
}
