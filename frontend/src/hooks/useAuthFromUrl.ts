import { useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { useAtom } from "jotai";
import { userGuidAtom, usernameAtom, emailAtom } from "@/atoms/auth";
import { useApiQuery } from "@/lib/api";

/**
 * Hook that syncs authentication state with URL query parameters.
 * Reads the 'user' parameter from URL and updates the auth atom.
 * Also fetches user profile data if userGuid exists but username is missing.
 */
export function useAuthFromUrl() {
    const [searchParams] = useSearchParams();
    const [userGuid, setUserGuid] = useAtom(userGuidAtom);
    const [username, setUsername] = useAtom(usernameAtom);
    const [email, setEmail] = useAtom(emailAtom);

    useEffect(() => {
        const userParam = searchParams.get("user");
        if (userParam && userParam !== userGuid) {
            setUserGuid(userParam);
        }
    }, [searchParams, userGuid, setUserGuid]);

    // Fetch profile data if we have userGuid but no username
    const shouldFetch = !!userGuid && !username;
    const { data } = useApiQuery<{ userName: string; email: string }>(
        `/profile/${userGuid}`,
        { enabled: shouldFetch } as any
    );

    useEffect(() => {
        if (data) {
            if (!username) setUsername(data.userName);
            if (!email) setEmail(data.email);
        }
    }, [data, username, email, setUsername, setEmail]);
}
