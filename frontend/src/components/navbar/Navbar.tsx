import type { ReactNode } from "react";

export default function Navbar({ children }: { children: ReactNode }) {
    return (
        <nav className="w-full flex justify-end items-center p-4 border-b border-border">
            {children}
        </nav>
    );
}
