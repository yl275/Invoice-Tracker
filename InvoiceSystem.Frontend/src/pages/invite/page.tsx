import { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { useAuth, useUser } from "@clerk/clerk-react";
import { Button } from "@/components/ui/button";
import api from "@/services/api";

const useClerk = !!import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;

export default function InviteAcceptPage() {
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token") ?? "";
  const navigate = useNavigate();
  const { isSignedIn, isLoaded } = useAuth();
  const { user } = useUser();
  const [status, setStatus] = useState<"idle" | "accepting" | "success" | "error">("idle");
  const [message, setMessage] = useState("");

  const primaryEmail = user?.primaryEmailAddress?.emailAddress ?? "";

  useEffect(() => {
    if (!token) return;
    if (useClerk && !isLoaded) return;
    if (useClerk && !isSignedIn) {
      const returnUrl = `/invite?token=${encodeURIComponent(token)}`;
      window.location.href = `/sign-in?redirect_url=${encodeURIComponent(returnUrl)}`;
      return;
    }
    if (useClerk && isSignedIn && user === undefined) return;

    setStatus("accepting");
    api
      .post("/teams/invitations/accept", { token, email: primaryEmail || undefined })
      .then(() => {
        setStatus("success");
        setMessage("You have joined the team. Redirecting…");
        setTimeout(() => navigate("/team", { replace: true }), 1500);
      })
      .catch((err) => {
        setStatus("error");
        const msg =
          (typeof err.response?.data === "string" ? err.response?.data : err.response?.data?.message) ??
          err.message ??
          "Invalid or expired invite.";
        setMessage(String(msg));
      });
  }, [isLoaded, isSignedIn, user, token, primaryEmail, navigate]);

  if (!token) {
    return (
      <div className="min-h-screen flex items-center justify-center p-4">
        <div className="text-center">
          <h1 className="text-xl font-semibold">Missing invite token</h1>
          <p className="text-muted-foreground mt-2">Use the link from your invitation email.</p>
          <Button className="mt-4" onClick={() => navigate("/")}>
            Go home
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center p-4">
      <div className="text-center max-w-sm">
        {status === "accepting" && (
          <>
            <p className="text-muted-foreground">Accepting invitation…</p>
          </>
        )}
        {status === "success" && <p className="text-emerald-600">{message}</p>}
        {status === "error" && (
          <>
            <h1 className="text-xl font-semibold text-destructive">Invite failed</h1>
            <p className="text-muted-foreground mt-2">{message}</p>
            <Button className="mt-4" variant="outline" onClick={() => navigate("/team")}>
              Go to Team
            </Button>
          </>
        )}
      </div>
    </div>
  );
}
