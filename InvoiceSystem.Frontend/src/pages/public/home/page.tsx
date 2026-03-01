import { SignedIn, SignedOut } from "@clerk/clerk-react";
import { Link } from "react-router-dom";

const useClerk = !!import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;
const ctaButtonClassName =
  "inline-flex items-center rounded-md bg-foreground px-4 py-2 text-sm font-medium text-background hover:opacity-90";
const pricingButtonClassName =
  "inline-flex items-center rounded-md border-2 border-foreground px-3 py-1.5 text-sm font-semibold text-foreground";

export default function HomePage() {
  return (
    <section className="space-y-4 flex flex-col items-center justify-center">
      <h1 className="text-3xl font-bold tracking-tight md:text-4xl p-4">
        Simple invoicing for growing teams
      </h1>
      <p className="max-w-2xl text-muted-foreground">
        Create invoices, manage clients and products, and keep everything organized in one place.
      </p>
      <div className="flex items-center gap-4 pt-2">
        {useClerk ? (
          <>
            <SignedIn>
              <Link to="/dashboard" className={ctaButtonClassName}>
                Join us free
              </Link>
            </SignedIn>
            <SignedOut>
              <Link to="/sign-up" className={ctaButtonClassName}>
                Join us free
              </Link>
            </SignedOut>
          </>
        ) : (
          <Link to="/dashboard" className={ctaButtonClassName}>
            Join us free
          </Link>
        )}
        <Link to="/pricing" className={pricingButtonClassName}>
          View pricing
        </Link>
      </div>
    </section>
  );
}
