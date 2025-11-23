import { useEffect, useState } from "react";
import { useAtom } from "jotai";
import { darkModeAtom } from "@/atoms/theme";
import { Page } from "@/components/layout/page";
import LoginPage from "@/components/auth/LoginPage";
import DarkModeToggle from "@/components/ui/DarkModeToggle";
import RegisterPage from "@/components/auth/RegisterPage";

export default function App() {
  const [path, setPath] = useState<string>(() => window.location.pathname || "/");
  const [darkMode] = useAtom(darkModeAtom);

  useEffect(() => {
    // On first render, only redirect to `/login` when the user hits the app root `/`.
    // This allows direct visits to routes like `/register` (e.g. page refresh) to stay put.
    if (window.location.pathname === "/" || window.location.pathname === "") {
      window.history.replaceState(null, "", "/login");
      setPath("/login");
    } else {
      setPath(window.location.pathname);
    }

    const onPop = () => setPath(window.location.pathname);
    window.addEventListener("popstate", onPop);
    return () => window.removeEventListener("popstate", onPop);
  }, []);

  const renderRoute = () => {
    switch (path) {
      case "/register":
        return <RegisterPage />;
      case "/login":
      default:
        return <LoginPage />;
    }
  };

  return (
    <div className={darkMode ? "dark min-h-screen" : "min-h-screen"}>
      <div className="absolute top-4 right-4 z-50">
        <DarkModeToggle />
      </div>
      <Page>
        {renderRoute()}
      </Page>    </div>

  );
}
