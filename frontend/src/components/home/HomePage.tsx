import { Card } from "@/components/ui/card";
import Navbar from "@/components/navbar/Navbar";

export default function HomePage() {
    return (
        <div className="min-h-screen bg-background">
            <Navbar />
            <div className="flex items-center justify-center h-[calc(100vh-64px)]">
                <Card className="w-[380px]">
                    Welcome to Nautilus
                </Card>
            </div>
        </div>
    );
}