import { Card } from "@/components/ui/card";
import { useAtom } from "jotai";
import { usernameAtom, userGuidAtom, emailAtom, passwordAtom } from "@/atoms/auth";
import { useEffect } from "react";
import { useApiQuery } from "@/lib/api";
import { useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { useAuthUrl } from "@/hooks/useAuthUrl";

export default function UserHomePage() {
    const [username, setUsername] = useAtom(usernameAtom);
    const [, setUserGuid] = useAtom(userGuidAtom);
    const [email, setEmail] = useAtom(emailAtom);
    const [, setPassword] = useAtom(passwordAtom);
    const navigate = useNavigate();
    const { userGuid } = useParams();
    const { buildUrl } = useAuthUrl();
    const { data, isLoading, error } = useApiQuery<{ userName: string; email: string }>(`/profile/${userGuid}`);

    useEffect(() => {
        if (data) {
            setUsername(data.userName);
            setEmail(data.email);
        }
    }, [data, setUsername, setEmail]);

    const handleLogout = () => {
        setUsername("");
        setUserGuid("");
        setEmail("");
        setPassword("");
        navigate("/");
    };

    return (
        <div className="flex items-center justify-center h-[calc(100vh-64px)]">
            <Card className="w-[380px] p-6">
                {isLoading ? (
                    <p>Loading...</p>
                ) : error ? (
                    <p>Error loading profile</p>
                ) : (
                    <>
                        <h2 className="text-2xl font-bold mb-2">Welcome to Nautilus, {username}!</h2>
                        <div className="text-sm text-muted-foreground mb-6">{email}</div>
                        <div className="flex gap-2">
                            <Button
                                variant="secondary"
                                onClick={() => navigate(buildUrl(`/profile/${userGuid}`))}
                            >
                                Profile Settings
                            </Button>
                            <Button
                                variant="destructive"
                                onClick={handleLogout}
                            >
                                Log out
                            </Button>
                        </div>
                    </>
                )}
            </Card>
        </div>
    );
}