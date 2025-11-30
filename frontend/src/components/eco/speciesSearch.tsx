import { useState } from "react";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useApiQuery } from "@/lib/api";

interface TaxonomicTree {
    kingdom?: string;
    phylum?: string;
    class?: string;
    order?: string;
    family?: string;
    genus?: string;
    species?: string;
    vernacularNames: string[];
}

interface SpeciesSearchResponse {
    results: TaxonomicTree[];
}

export default function SpeciesSearch() {
    const [searchQuery, setSearchQuery] = useState("");
    const [submittedQuery, setSubmittedQuery] = useState("");
    const [kingdomFilter, setKingdomFilter] = useState("animalia");

    const { data, isLoading, error } = useApiQuery<SpeciesSearchResponse>(
        `/species?query=${encodeURIComponent(submittedQuery)}&kingdom=${encodeURIComponent(kingdomFilter)}`,
        { enabled: !!submittedQuery } as any
    );

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        if (searchQuery.trim()) {
            setSubmittedQuery(searchQuery.trim());
        }
    };

    const getKingdomColor = (kingdom?: string) => {
        switch (kingdom?.toLowerCase()) {
            case 'animalia':
                return 'bg-blue-500 text-white';
            case 'plantae':
                return 'bg-green-500 text-white';
            case 'fungi':
                return 'bg-orange-500 text-white';
            case 'bacteria':
                return 'bg-purple-500 text-white';
            case 'archaea':
                return 'bg-pink-500 text-white';
            case 'protista':
                return 'bg-teal-500 text-white';
            case 'chromista':
                return 'bg-yellow-600 text-white';
            default:
                return 'bg-gray-500 text-white';
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-background p-4">
            <Card className="w-full max-w-4xl">
                <CardHeader>
                    <CardTitle className="text-center">Species Search</CardTitle>
                </CardHeader>
                <CardContent>
                    <form onSubmit={handleSearch} className="space-y-4">
                        <div className="flex gap-2">
                            <Input
                                type="text"
                                placeholder="Search for a species! (e.g., lion, eagle, dolphin)"
                                value={searchQuery}
                                onChange={(e) => setSearchQuery(e.target.value)}
                                className="flex-1"
                            />
                            <select
                                value={kingdomFilter}
                                onChange={(e) => setKingdomFilter(e.target.value)}
                                className="px-3 py-2 border border-input bg-background rounded-md text-sm ring-offset-background focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2"
                            >
                                <option value="all">All Kingdoms</option>
                                <option value="animalia">Animalia</option>
                                <option value="plantae">Plantae</option>
                                <option value="fungi">Fungi</option>
                                <option value="bacteria">Bacteria</option>
                                <option value="archaea">Archaea</option>
                                <option value="protista">Protista</option>
                                <option value="chromista">Chromista</option>
                            </select>
                            <Button type="submit" disabled={isLoading || !searchQuery.trim()}>
                                {isLoading ? "Searching..." : "Search"}
                            </Button>
                        </div>
                    </form>

                    {error && (
                        <div className="mt-4 p-4 bg-red-100 dark:bg-red-900 text-red-800 dark:text-red-200 rounded">
                            Error loading species data
                        </div>
                    )}

                    {submittedQuery && !isLoading && data && (
                        <div className="mt-6">
                            <h3 className="text-lg font-semibold mb-3">
                                Results for "{submittedQuery}": {data.results?.length || 0} species found
                            </h3>
                            {data.results && data.results.length > 0 ? (
                                <div className="space-y-4">
                                    {data.results.map((tree, index) => (
                                        <Card key={index} className="p-4">
                                            <div className="space-y-2">
                                                <div className="flex items-center gap-2 flex-wrap">
                                                    <span className={`px-2 py-1 rounded text-xs font-semibold ${getKingdomColor(tree.kingdom)}`}>
                                                        {tree.kingdom || "Unknown"}
                                                    </span>
                                                    <span className="font-bold text-lg">{tree.species}</span>
                                                    {tree.vernacularNames.length > 0 && (
                                                        <span className="text-sm text-muted-foreground">
                                                            ({tree.vernacularNames.join(", ")})
                                                        </span>
                                                    )}
                                                </div>
                                                <div className="text-sm text-muted-foreground space-y-1">
                                                    {tree.phylum && (
                                                        <div className="flex gap-2">
                                                            <span className="font-semibold min-w-20">Phylum:</span>
                                                            <span>{tree.phylum}</span>
                                                        </div>
                                                    )}
                                                    {tree.class && (
                                                        <div className="flex gap-2">
                                                            <span className="font-semibold min-w-20">Class:</span>
                                                            <span>{tree.class}</span>
                                                        </div>
                                                    )}
                                                    {tree.order && (
                                                        <div className="flex gap-2">
                                                            <span className="font-semibold min-w-20">Order:</span>
                                                            <span>{tree.order}</span>
                                                        </div>
                                                    )}
                                                    {tree.family && (
                                                        <div className="flex gap-2">
                                                            <span className="font-semibold min-w-20">Family:</span>
                                                            <span>{tree.family}</span>
                                                        </div>
                                                    )}
                                                    {tree.genus && (
                                                        <div className="flex gap-2">
                                                            <span className="font-semibold min-w-20">Genus:</span>
                                                            <span>{tree.genus}</span>
                                                        </div>
                                                    )}
                                                </div>
                                            </div>
                                        </Card>
                                    ))}
                                </div>
                            ) : (
                                <p className="text-muted-foreground">
                                    No species found matching "{submittedQuery}"
                                </p>
                            )}
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
}
