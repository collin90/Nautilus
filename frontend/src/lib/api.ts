const BASE = (import.meta as any).env?.VITE_API_BASE || "http://localhost:5106"

export async function post<T>(path: string, body: unknown) {
    const res = await fetch(`${BASE}${path}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body),
    })

    const text = await res.text()
    let json: any = null
    try {
        json = text ? JSON.parse(text) : null
    } catch (e) {
        // non-json response
    }

    if (!res.ok) {
        const msg = json?.message || json?.error || text || res.statusText
        throw new Error(msg || "Request failed")
    }

    return json as T
}

export const apiBase = BASE
