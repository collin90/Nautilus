import { Card } from "@/components/ui/card";
import Navbar from "@/components/navbar/Navbar";
import { Link } from "react-router-dom";

export default function HomePage() {
    return (
        <div className="min-h-screen bg-background">
            <Navbar>
                <Link to="/species-search">
                    <button className="mr-2 px-4 py-2 rounded bg-accent text-accent-foreground hover:bg-accent/80 transition">Search Species</button>
                </Link>
                <Link to="/login">
                    <button className="mr-2 px-4 py-2 rounded bg-primary text-primary-foreground hover:bg-primary/80 transition">Login</button>
                </Link>
                <Link to="/register">
                    <button className="px-4 py-2 rounded bg-secondary text-secondary-foreground hover:bg-secondary/80 transition">Create Account</button>
                </Link>
            </Navbar>
            <div className="flex items-center justify-center h-[calc(100vh-64px)]">
                <Card className="w-[380px]">
                    Welcome to Nautilus
                </Card>
            </div>
        </div>
    );
}