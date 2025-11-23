import { useEffect, useState, useCallback } from "react";
import { Page } from "@/components/layout/page";
import LoginPage from "@/components/auth/LoginPage";
import RegisterPage from "@/components/auth/RegisterPage";

export default function App() {
  const [path, setPath] = useState<string>(() => window.location.pathname || "/");

  useEffect(() => {
    // On first render, ensure the user lands on /login regardless of route.
    if (window.location.pathname !== "/login") {
      window.history.replaceState(null, "", "/login");
      setPath("/login");
    }

    const onPop = () => setPath(window.location.pathname);
    window.addEventListener("popstate", onPop);
    return () => window.removeEventListener("popstate", onPop);
  }, []);

  const navigate = useCallback((to: string) => {
    if (to === path) return;
    window.history.pushState(null, "", to);
    setPath(to);
    // optionally trigger global popstate listeners
    window.dispatchEvent(new PopStateEvent("popstate"));
  }, [path]);

  const renderRoute = () => {
    switch (path) {
      case "/register":
        return <RegisterPage onNavigate={navigate} />;
      case "/login":
      default:
        return <LoginPage onNavigate={navigate} />;
    }
  };

  return (
    <Page>
      {renderRoute()}
    </Page>
  );
}
