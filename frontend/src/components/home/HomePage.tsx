import { Card } from "@/components/ui/card";

export default function HomePage() {
    return (
        <div className="flex items-center justify-center h-[calc(100vh-64px)]">
            <Card className="w-[380px] p-8 text-center">
                <h1 className="text-2xl font-bold mb-4">Welcome to Nautilus</h1>
                <p className="text-muted-foreground">
                    Explore the world of species and biodiversity
                </p>
            </Card>
        </div>
    );
}