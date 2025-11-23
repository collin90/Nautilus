import type { ReactNode } from "react";

export function Page({ children }: { children: ReactNode }) {
  return (
    <div className="min-h-screen bg-background flex items-center justify-center">
      {children}
    </div>
  );
}
