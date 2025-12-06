import { post } from "@/lib/api"
import type { LoginRequest, RegisterRequest } from "@/dto/auth/auth"

export async function login(identifier: string, password: string) {
    const body: LoginRequest = { Identifier: identifier, Password: password }
    return post<any>("/auth/login", body)
}

export async function register(userName: string, email: string, password: string) {
    const body: RegisterRequest = { UserName: userName, Email: email, Password: password }
    return post<any>("/auth/register", body)
}

export async function activateAccount(token: string) {
    return post<{ message: string }>("/auth/activate", { Token: token })
}

export async function requestPasswordReset(email: string) {
    return post<{ message: string }>("/auth/request-password-reset", { Email: email })
}

export async function resetPassword(token: string, newPassword: string) {
    return post<{ message: string }>("/auth/reset-password", { Token: token, NewPassword: newPassword })
}
