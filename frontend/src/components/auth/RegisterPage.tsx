import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

type Props = {
    onNavigate: (path: string) => void;
};

export default function RegisterPage({ onNavigate }: Props) {
    return (
        <div className="min-h-screen flex items-center justify-center bg-background">
            <Card className="w-[420px]">
                <CardHeader>
                    <CardTitle className="text-center">Create account</CardTitle>
                </CardHeader>
                <CardContent>
                    <form
                        className="space-y-4"
                        onSubmit={(e) => {
                            e.preventDefault();
                            // UI-only: no server calls yet
                        }}
                    >
                        <Input label="Username" type="text" placeholder="choose-a-username" />

                        <Input label="Email" type="email" placeholder="you@example.com" />

                        <Input label="Password" type="password" placeholder="Choose a password" />

                        <Input label="Confirm password" type="password" placeholder="Confirm password" />

                        <div className="flex items-center justify-between">
                            <Button type="submit">Create account</Button>
                            <Button
                                variant="link"
                                size="sm"
                                type="button"
                                onClick={() => onNavigate("/login")}
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
