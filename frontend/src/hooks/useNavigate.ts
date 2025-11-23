import { useCallback } from "react"

export default function useNavigate() {
    return useCallback((to: string) => {
        if (typeof window === "undefined") return
        if (window.location.pathname === to) return
        window.history.pushState(null, "", to)
        // notify listeners (App listens for popstate)
        window.dispatchEvent(new PopStateEvent("popstate"))
    }, [])
}
