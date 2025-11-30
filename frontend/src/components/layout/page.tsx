import type { ReactNode } from "react";
import Navbar from "@/components/navbar/Navbar";
import { useAuthFromUrl } from "@/hooks/useAuthFromUrl";

export function Page({ children }: { children: ReactNode }) {
  useAuthFromUrl();

  return (
    <div className="min-h-screen bg-background">
      <Navbar />
      {children}
    </div>
  );
}
