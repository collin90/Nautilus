/**
 * Utility functions for managing JWT tokens in localStorage
 */

const TOKEN_KEY = 'nautilus_jwt_token';

export function setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
}

export function getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
}

export function removeToken(): void {
    localStorage.removeItem(TOKEN_KEY);
}

export function hasToken(): boolean {
    return !!getToken();
}
