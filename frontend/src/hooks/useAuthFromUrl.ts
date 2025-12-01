import { useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { useAtom } from "jotai";
import { userGuidAtom, usernameAtom, emailAtom } from "@/atoms/auth";
import { getToken, removeToken } from "@/lib/auth/tokenStorage";
import { getUserFromToken, isTokenExpired } from "@/lib/auth/jwtUtils";

/**
 * Hook that syncs authentication state with JWT token.
 * Reads the JWT token from localStorage and updates the auth atoms.
 * Falls back to URL parameter for backward compatibility.
 */
export function useAuthFromUrl() {
    const [searchParams] = useSearchParams();
    const [userGuid, setUserGuid] = useAtom(userGuidAtom);
    const [username, setUsername] = useAtom(usernameAtom);
    const [email, setEmail] = useAtom(emailAtom);

    useEffect(() => {
        // Try to restore auth from JWT token
        const token = getToken();

        if (token) {
            // Check if token is expired
            if (isTokenExpired(token)) {
                removeToken();
                setUserGuid("");
                setUsername("");
                setEmail("");
                return;
            }

            // Extract user info from token
            const userInfo = getUserFromToken(token);
            if (userInfo) {
                if (!userGuid && userInfo.userId) setUserGuid(userInfo.userId);
                if (!username && userInfo.username) setUsername(userInfo.username);
                if (!email && userInfo.email) setEmail(userInfo.email);
                return;
            }
        }

        // Fallback: try URL parameter (for backward compatibility)
        const userParam = searchParams.get("user");
        if (userParam && userParam !== userGuid) {
            setUserGuid(userParam);
        }
    }, [searchParams, userGuid, username, email, setUserGuid, setUsername, setEmail]);
}
