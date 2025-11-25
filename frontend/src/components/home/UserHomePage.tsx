import { Card } from "@/components/ui/card"
import { usernameAtom } from "@/atoms/auth"
import { useAtom } from "jotai"

export default function LoginPage() {
    const [username] = useAtom(usernameAtom)
    return (
        <div className="min-h-screen flex items-center justify-center bg-background">
            <Card className="w-[380px]">
                Welcome to Nautilus, {username}!
            </Card>
        </div>
    )
}