import { useAtom } from "jotai";
import { darkModeAtom } from "@/atoms/theme";
import { Switch } from "@/components/ui/switch";

export default function DarkModeToggle() {
    const [darkMode, setDarkMode] = useAtom(darkModeAtom);

    return (
        <label className="flex items-center gap-2 cursor-pointer select-none">
            <span className="text-xs text-gray-500 dark:text-gray-400">☀️</span>
            <Switch
                checked={darkMode}
                onCheckedChange={setDarkMode}
                aria-label="Toggle dark mode"
            />
            <span className="text-xs text-gray-500 dark:text-gray-400">🌙</span>
        </label>
    );
}