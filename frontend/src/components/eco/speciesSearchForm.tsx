import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { kingdoms } from "@/lib/eco/kingdoms";

interface SpeciesSearchFormProps {
    searchQuery: string;
    setSearchQuery: (query: string) => void;
    kingdom: string;
    setKingdom: (kingdom: string) => void;
    onSubmit: (e: React.FormEvent) => void;
    isLoading: boolean;
}

export default function SpeciesSearchForm({
    searchQuery,
    setSearchQuery,
    kingdom,
    setKingdom,
    onSubmit,
    isLoading
}: SpeciesSearchFormProps) {
    const selectedKingdom = kingdoms.find(k => k.value === kingdom) || kingdoms[0];

    return (
        <form onSubmit={onSubmit} className="space-y-4">
            <div className="flex gap-3">
                <select
                    value={kingdom}
                    onChange={(e) => setKingdom(e.target.value)}
                    className={`w-32 px-3 py-2 border border-input rounded-md text-sm ring-offset-background focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 text-white font-medium ${selectedKingdom.color}`}
                >
                    {kingdoms.map(k => (
                        <option key={k.value} value={k.value} className="bg-background text-foreground">
                            {k.label}
                        </option>
                    ))}
                </select>
                <Input
                    type="text"
                    placeholder="Search species by scientific or common name"
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="flex-1"
                />
                <Button type="submit" disabled={isLoading || !searchQuery.trim()}>
                    {isLoading ? "Searching..." : "Search"}
                </Button>
            </div>
        </form>
    );
}
