import { getKingdomColor } from "@/lib/eco/kingdoms";

interface SpeciesSearchResultRowProps {
    scientificName: string;
    kingdom: string | null;
    vernacularNames: string[];
    imageUrl: string | null;
}

export default function SpeciesSearchResultRow({
    scientificName,
    kingdom,
    vernacularNames,
    imageUrl
}: SpeciesSearchResultRowProps) {
    return (
        <div className="flex items-center gap-4 p-3 hover:bg-muted/50 rounded-lg transition-colors">
            {/* Species Image */}
            <div className="flex-shrink-0">
                {imageUrl ? (
                    <img
                        src={imageUrl}
                        alt={scientificName}
                        className="w-20 h-20 object-cover rounded-md border border-border"
                        onError={(e) => {
                            e.currentTarget.src = 'data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" width="80" height="80"><rect fill="%23ddd" width="80" height="80"/><text x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" fill="%23999" font-family="sans-serif" font-size="10">No Image</text></svg>';
                        }}
                    />
                ) : (
                    <div className="w-20 h-20 flex items-center justify-center bg-muted rounded-md border border-border">
                        <svg
                            className="w-8 h-8 text-muted-foreground"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                        >
                            <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth={2}
                                d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"
                            />
                        </svg>
                    </div>
                )}
            </div>

            {/* Species Info */}
            <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2">
                    {kingdom && (
                        <span className={`px-2 py-0.5 rounded text-xs font-semibold text-white ${getKingdomColor(kingdom)}`}>
                            {kingdom}
                        </span>
                    )}
                    <div className="font-semibold text-base italic">
                        {scientificName}
                    </div>
                </div>
                {vernacularNames.length > 0 && (
                    <div className="text-sm text-muted-foreground mt-0.5">
                        {vernacularNames.join(", ")}
                    </div>
                )}
            </div>
        </div>
    );
}
