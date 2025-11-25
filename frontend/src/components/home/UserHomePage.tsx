import { Card } from "@/components/ui/card";
import Navbar from "@/components/navbar/Navbar";
import { useAtom } from "jotai";
import { usernameAtom, userGuidAtom, emailAtom, passwordAtom } from "@/atoms/auth";
import { useEffect } from "react";
import { useApiQuery } from "@/lib/api";
import { useNavigate, useParams } from "react-router-dom";

export default function UserHomePage() {
    const [username, setUsername] = useAtom(usernameAtom);
    const [, setUserGuid] = useAtom(userGuidAtom);
    const [email, setEmail] = useAtom(emailAtom);
    const [, setPassword] = useAtom(passwordAtom);
    const navigate = useNavigate();
    const { userGuid } = useParams();
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
        <div className="min-h-screen bg-background">
            <Navbar>
                <button
                    className="mr-2 px-4 py-2 rounded bg-destructive text-destructive-foreground hover:bg-destructive/80 transition"
                    onClick={handleLogout}
                >
                    Log out
                </button>
                <button
                    className="px-4 py-2 rounded bg-secondary text-secondary-foreground hover:bg-secondary/80 transition"
                    onClick={() => navigate(`/profile/${userGuid}`)}
                >
                    Profile
                </button>
            </Navbar>
            <div className="flex items-center justify-center h-[calc(100vh-64px)]">
                <Card className="w-[380px]">
                    {isLoading ? "Loading..." : error ? "Error loading profile" : `Welcome to Nautilus, ${username}!`}
                    <div className="mt-2 text-sm text-muted-foreground">{email}</div>
                </Card>
            </div>
        </div>
    );
}