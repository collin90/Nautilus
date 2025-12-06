import { useAtom } from "jotai";
import { darkModeAtom } from "@/atoms/theme";
import { Page } from "@/components/layout/page";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import HomePage from "@/components/home/HomePage";
import LoginPage from "@/components/auth/LoginPage";
import RegisterPage from "@/components/auth/RegisterPage";
import ActivateAccountPage from "@/components/auth/ActivateAccountPage";
import RequestPasswordResetPage from "@/components/auth/RequestPasswordResetPage";
import ResetPasswordPage from "@/components/auth/ResetPasswordPage";
import UserHomePage from "@/components/home/UserHomePage";
import ProfileSettings from "@/components/profile/ProfileSettings";
import Explore from "@/components/eco/explore";
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

const queryClient = new QueryClient();

export default function App() {
  const [darkMode] = useAtom(darkModeAtom);
  return (
    <QueryClientProvider client={queryClient}>
      <div className={darkMode ? "dark min-h-screen" : "min-h-screen"}>
        <BrowserRouter>
          <Page>
            <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/explore" element={<Explore />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/activate" element={<ActivateAccountPage />} />
              <Route path="/request-reset" element={<RequestPasswordResetPage />} />
              <Route path="/reset-password" element={<ResetPasswordPage />} />
              <Route path="/home" element={<UserHomePage />} />
              <Route path="/profile" element={<ProfileSettings />} />
              <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
          </Page>
        </BrowserRouter>
      </div>
    </QueryClientProvider>
  );
}
