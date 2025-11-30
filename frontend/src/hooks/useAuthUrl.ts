import { useAtom } from "jotai";
import { userGuidAtom } from "@/atoms/auth";

/**
 * Hook that returns a function to build URLs with authentication state.
 * When user is logged in, appends ?user=<guid> to all URLs.
 */
export function useAuthUrl() {
    const [userGuid] = useAtom(userGuidAtom);

    const buildUrl = (path: string): string => {
        if (!userGuid) return path;

        const separator = path.includes("?") ? "&" : "?";
        return `${path}${separator}user=${userGuid}`;
    };

    return { buildUrl, userGuid };
}
