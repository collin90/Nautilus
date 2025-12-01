import { Link } from "react-router-dom";
import { useAtom } from "jotai";
import { usernameAtom } from "@/atoms/auth";

export default function Navbar() {
    const [username] = useAtom(usernameAtom);
    const isLoggedIn = !!username;

    return (
        <nav className="w-full h-16 border-b border-border flex items-center px-6 bg-background">
            <div className="flex items-center gap-2">
                <Link to="/" className="flex items-center gap-2 text-xl font-semibold hover:opacity-80 transition">
                    <img src="/logo.png" alt="Nautilus Logo" className="h-6 w-6" />
                    <span>Nautilus</span>
                </Link>
            </div>

            <div className="flex-1" />

            <div className="flex items-center gap-6">
                <Link to="/explore" className="text-sm font-medium hover:text-primary transition">
                    Explore
                </Link>

                {isLoggedIn ? (
                    <Link to="/profile" className="text-sm font-medium hover:text-primary transition">
                        {username}
                    </Link>
                ) : (
                    <div className="flex items-center gap-4">
                        <Link to="/login" className="text-sm font-medium hover:text-primary transition">
                            Login
                        </Link>
                        <Link to="/register" className="text-sm font-medium hover:text-primary transition">
                            Signup
                        </Link>
                    </div>
                )}
            </div>
        </nav>
    );
}
