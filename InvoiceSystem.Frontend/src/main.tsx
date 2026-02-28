import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { ClerkProvider } from "@clerk/clerk-react";
import "./index.css";
import App from "./App.tsx";
import { setApiAuth } from "./services/api";

const publishableKey = import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;
if (!publishableKey && import.meta.env.MODE !== "test") {
  console.warn("VITE_CLERK_PUBLISHABLE_KEY is not set. Auth will use X-User-Id header for dev.");
  // Dev bypass: no Clerk, use X-User-Id
  setApiAuth(null);
}

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    {publishableKey ? (
      <ClerkProvider publishableKey={publishableKey}>
        <App />
      </ClerkProvider>
    ) : (
      <App />
    )}
  </StrictMode>,
);
