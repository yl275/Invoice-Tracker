import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useSignIn, useUser } from "@clerk/clerk-react";

// Demo credentials – this project is a toy, so we keep them in the client.
const DEMO_ID = "demo-invoicesys";
const DEMO_PASSWORD = "Demo1234!invoice";

export function DemoLoginButton() {
  const { signIn, isLoaded: signInLoaded, setActive } = useSignIn();
  const { isSignedIn } = useUser();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const handleClick = async () => {
    if (!signInLoaded || loading) return;
    setLoading(true);
    try {
      // If already signed in, just go straight to dashboard.
      if (!isSignedIn) {
        const result = await signIn!.create({
          identifier: DEMO_ID,
          password: DEMO_PASSWORD,
        });

        if (result.status === "complete") {
          await setActive!({ session: result.createdSessionId });
        } else {
          // Fallback: even if status is not complete, try to continue.
          console.warn("Demo sign-in not complete:", result);
        }
      }

      navigate("/dashboard");
    } catch (error) {
      console.error("Demo login failed", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <button
      type="button"
      className="underline underline-offset-4 hover:text-white disabled:opacity-60"
      onClick={handleClick}
      disabled={!signInLoaded || loading}
    >
      {loading ? "Signing in..." : "Try this demo"}
    </button>
  );
}

