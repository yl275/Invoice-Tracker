import { useEffect } from "react";
import { useAuth } from "@clerk/clerk-react";
import { setApiAuth } from "@/services/api";

/**
 * Must be rendered inside ClerkProvider. Sets up API to add Bearer token to requests.
 */
export function AuthSetup() {
  const { getToken } = useAuth();

  useEffect(() => {
    setApiAuth(() => getToken());
  }, [getToken]);

  return null;
}
