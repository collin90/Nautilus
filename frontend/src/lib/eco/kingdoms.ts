export interface Kingdom {
    value: string;
    label: string;
    color: string;
}

export const kingdoms: Kingdom[] = [
    { value: "all", label: "All Kingdoms", color: "bg-gray-500" },
    { value: "animalia", label: "Animalia", color: "bg-blue-500" },
    { value: "plantae", label: "Plantae", color: "bg-green-500" },
    { value: "fungi", label: "Fungi", color: "bg-orange-500" },
    { value: "bacteria", label: "Bacteria", color: "bg-purple-500" },
    { value: "archaea", label: "Archaea", color: "bg-pink-500" },
    { value: "protista", label: "Protista", color: "bg-teal-500" },
    { value: "chromista", label: "Chromista", color: "bg-yellow-600" },
];

export const getKingdomColor = (kingdom?: string | null): string => {
    const found = kingdoms.find(k => k.value.toLowerCase() === kingdom?.toLowerCase());
    return found?.color || "bg-gray-500";
};

export const getKingdomLabel = (kingdom?: string | null): string => {
    const found = kingdoms.find(k => k.value.toLowerCase() === kingdom?.toLowerCase());
    return found?.label || kingdom || "Unknown";
};
