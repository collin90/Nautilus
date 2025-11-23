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
