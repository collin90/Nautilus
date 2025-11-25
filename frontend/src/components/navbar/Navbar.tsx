import { Link } from "react-router-dom";

export default function Navbar() {
    return (
        <nav className="w-full flex justify-end items-center p-4 border-b border-border">
            <Link to="/login">
                <button className="mr-2 px-4 py-2 rounded bg-primary text-primary-foreground hover:bg-primary/80 transition">Login</button>
            </Link>
            <Link to="/register">
                <button className="px-4 py-2 rounded bg-secondary text-secondary-foreground hover:bg-secondary/80 transition">Create Account</button>
            </Link>
        </nav>
    );
}
