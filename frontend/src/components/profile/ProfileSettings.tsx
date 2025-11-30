import { Button } from "@/components/ui/button";
import { useNavigate, useParams } from "react-router-dom";
import { useApiQuery } from "@/lib/api";
import { useAtom } from "jotai";
import { usernameAtom, emailAtom } from "@/atoms/auth";
import DarkModeToggle from "@/components/ui/darkModeToggle";
import { useEffect } from "react";
import { useAuthUrl } from "@/hooks/useAuthUrl";

export default function ProfileSettings() {
    const navigate = useNavigate();
    const { userGuid } = useParams();
    const { buildUrl } = useAuthUrl();
    const [username, setUsername] = useAtom(usernameAtom);
    const [email, setEmail] = useAtom(emailAtom);
    const shouldFetch = !username || !email;
    const { data, isLoading, error } = useApiQuery<{ userName: string; email: string }>(`/profile/${userGuid}`);

    useEffect(() => {
        if (data) {
            if (!username) setUsername(data.userName);
            if (!email) setEmail(data.email);
        }
    }, [data, setUsername, setEmail, username, email]);

    return (
        <div className="min-h-screen flex items-center justify-center bg-background">
            <div className="bg-card p-8 rounded shadow w-[380px]">
                <div className="flex justify-between items-center mb-4">
                    <h2 className="text-xl font-bold">Profile Settings</h2>
                    <DarkModeToggle />
                </div>
                <div className="text-center">
                    {shouldFetch && isLoading ? (
                        <p>Loading...</p>
                    ) : error ? (
                        <p className="text-red-600">Error loading profile</p>
                    ) : (
                        <>
                            <div className="mb-2">Username: <span className="font-mono">{username}</span></div>
                            <div className="mb-6">Email: <span className="font-mono">{email}</span></div>
                        </>
                    )}
                    <Button onClick={() => navigate(buildUrl(`/home/${userGuid}`))}>Done</Button>
                </div>
            </div>
        </div>
    );
}
