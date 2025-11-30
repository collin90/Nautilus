import { useState } from "react";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useApiQuery } from "@/lib/api";
import SpeciesSearchResultRow from "./speciesSearchResultRow";
import { kingdoms } from "@/lib/eco/kingdoms";

interface SpeciesSearchResult {
    scientificName: string;
    kingdom: string | null;
    vernacularNames: string[];
    imageUrl: string | null;
}

interface SpeciesSearchResponse {
    results: SpeciesSearchResult[];
}

export default function SpeciesSearch() {
    const [searchQuery, setSearchQuery] = useState("");
    const [submittedQuery, setSubmittedQuery] = useState("");
    const [kingdom, setKingdom] = useState("animalia");
    const [submittedKingdom, setSubmittedKingdom] = useState("animalia");

    const { data, isLoading, error } = useApiQuery<SpeciesSearchResponse>(
        `/species?query=${encodeURIComponent(submittedQuery)}&kingdom=${encodeURIComponent(submittedKingdom)}`,
        { enabled: !!submittedQuery } as any
    );

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        if (searchQuery.trim()) {
            setSubmittedQuery(searchQuery.trim());
            setSubmittedKingdom(kingdom);
        }
    };

    const selectedKingdom = kingdoms.find(k => k.value === kingdom) || kingdoms[0];

    return (
        <div className="min-h-screen flex items-center justify-center bg-background p-4">
            <Card className="w-full max-w-3xl">
                <CardHeader>
                    <CardTitle className="text-center">Species Search</CardTitle>
                </CardHeader>
                <CardContent>
                    <form onSubmit={handleSearch} className="space-y-4">
                        <div className="flex gap-2">
                            <select
                                value={kingdom}
                                onChange={(e) => setKingdom(e.target.value)}
                                className={`px-3 py-2 border border-input rounded-md text-sm ring-offset-background focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 text-white font-medium ${selectedKingdom.color}`}
                            >
                                {kingdoms.map(k => (
                                    <option key={k.value} value={k.value}>
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

                    {error && (
                        <div className="mt-4 p-4 bg-red-100 dark:bg-red-900 text-red-800 dark:text-red-200 rounded-md">
                            Error loading species data
                        </div>
                    )}

                    {submittedQuery && !isLoading && data && (
                        <div className="mt-6">
                            <div className="text-sm text-muted-foreground mb-3">
                                {data.results?.length || 0} {data.results?.length === 1 ? 'result' : 'results'} for "{submittedQuery}"
                                {submittedKingdom !== "all" && (
                                    <span className="ml-1">
                                        in {kingdoms.find(k => k.value === submittedKingdom)?.label}
                                    </span>
                                )}
                            </div>
                            {data.results && data.results.length > 0 ? (
                                <div className="border rounded-lg divide-y">
                                    {data.results.map((species, index) => (
                                        <SpeciesSearchResultRow
                                            key={index}
                                            scientificName={species.scientificName}
                                            kingdom={species.kingdom}
                                            vernacularNames={species.vernacularNames}
                                            imageUrl={species.imageUrl}
                                        />
                                    ))}
                                </div>
                            ) : (
                                <div className="text-center py-8 text-muted-foreground">
                                    No species found matching "{submittedQuery}"
                                </div>
                            )}
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
}
