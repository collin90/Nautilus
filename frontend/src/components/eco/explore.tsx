import { useState } from "react";
import Loading from "@/components/ui/loading";
import { useApiQuery } from "@/lib/api";
import SpeciesSearchResultRow from "./speciesSearchResultRow";
import SpeciesSearchForm from "./speciesSearchForm";
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

export default function Explore() {
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

    return (
        <div className="flex justify-center p-8">
            <div className="w-full max-w-6xl">
                <h1 className="text-3xl font-bold mb-6">Explore Species</h1>

                <SpeciesSearchForm
                    searchQuery={searchQuery}
                    setSearchQuery={setSearchQuery}
                    kingdom={kingdom}
                    setKingdom={setKingdom}
                    onSubmit={handleSearch}
                    isLoading={isLoading}
                />

                {isLoading && <Loading />}

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
            </div>
        </div>
    );
}
