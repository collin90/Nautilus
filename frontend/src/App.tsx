import { useAtom } from "jotai";
import { darkModeAtom } from "@/atoms/theme";
import { Page } from "@/components/layout/page";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import HomePage from "@/components/home/HomePage";
import LoginPage from "@/components/auth/LoginPage";
import RegisterPage from "@/components/auth/RegisterPage";
import UserHomePage from "@/components/home/UserHomePage";
import DarkModeToggle from "@/components/ui/darkModeToggle";

export default function App() {
  const [darkMode] = useAtom(darkModeAtom);
  return (
    <div className={darkMode ? "dark min-h-screen" : "min-h-screen"}>
      <div className="absolute top-4 right-4 z-50">
        <DarkModeToggle />
      </div>
      <BrowserRouter>
        <Page>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/home/:userGuid" element={<UserHomePage />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </Page>
      </BrowserRouter>
    </div>
  );
}
