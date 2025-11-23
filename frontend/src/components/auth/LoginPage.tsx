import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useState } from "react";

type Props = {
    onNavigate: (path: string) => void;
};

export default function LoginPage({ onNavigate }: Props) {
    const [mode, setMode] = useState<"username" | "email">("username");

    return (
        <div className="min-h-screen flex items-center justify-center bg-background">
            <Card className="w-[380px]">
                <CardHeader>
                    <CardTitle className="text-center">Sign in</CardTitle>
                </CardHeader>
                <CardContent>
                    <form
                        className="space-y-4"
                        onSubmit={(e) => {
                            e.preventDefault();
                            // UI-only: no server calls yet
                        }}
                    >
                        {mode === "username" ? (
                            <div>
                                <Input label="Username" type="text" placeholder="your-username" />
                                <div className="mt-2">
                                    <Button variant="link" size="sm" type="button" onClick={() => setMode("email")}>Use email instead</Button>
                                </div>
                            </div>
                        ) : (
                            <div>
                                <Input label="Email" type="email" placeholder="you@example.com" />
                                <div className="mt-2">
                                    <Button variant="link" size="sm" type="button" onClick={() => setMode("username")}>Use username instead</Button>
                                </div>
                            </div>
                        )}

                        <Input label="Password" type="password" placeholder="Your password" />

                        <div className="flex items-center justify-between">
                            <Button type="submit">Sign in</Button>
                            <Button variant="link" size="sm" type="button" onClick={() => onNavigate("/register")}>
                                Create account
                            </Button>
                        </div>
                    </form>
                </CardContent>
            </Card>
        </div>
    );
}
