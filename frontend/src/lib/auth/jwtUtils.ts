/**
 * Decodes a JWT token and returns the payload.
 * Does not validate the signature - validation should be done server-side.
 */
export function decodeJwt(token: string): any {
    try {
        const parts = token.split('.');
        if (parts.length !== 3) {
            return null;
        }

        const payload = parts[1];
        const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
        return JSON.parse(decoded);
    } catch (error) {
        console.error('Error decoding JWT:', error);
        return null;
    }
}

/**
 * Checks if a JWT token is expired.
 */
export function isTokenExpired(token: string): boolean {
    const payload = decodeJwt(token);
    if (!payload || !payload.exp) {
        return true;
    }

    const expirationTime = payload.exp * 1000; // Convert to milliseconds
    return Date.now() >= expirationTime;
}

/**
 * Extracts user information from a JWT token.
 */
export function getUserFromToken(token: string): { userId: string; username: string; email: string } | null {
    const payload = decodeJwt(token);
    if (!payload) {
        return null;
    }

    return {
        userId: payload.sub || payload.nameid || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        username: payload.name || payload.unique_name || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
        email: payload.email || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']
    };
}
