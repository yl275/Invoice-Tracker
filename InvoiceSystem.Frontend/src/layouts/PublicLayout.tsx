import { SignOutButton, SignedIn, SignedOut } from "@clerk/clerk-react";
import { Link, NavLink, Outlet } from "react-router-dom";

const useClerk = !!import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;

function navClassName(isActive: boolean): string {
  return isActive
    ? "text-foreground font-medium"
    : "text-muted-foreground hover:text-foreground";
}

export function PublicLayout() {
  return (
    <div className="flex min-h-screen flex-col bg-background">
      <header className="border-b">
        <div className="mx-auto flex h-16 max-w-6xl items-center justify-between px-4 md:px-6">
          <div className="flex items-center gap-6">
            <Link to="/" className="text-lg font-semibold tracking-tight">
              InvoiceSys
            </Link>
            <nav className="flex items-center gap-4 text-sm">
              <NavLink to="/pricing" className={({ isActive }) => navClassName(isActive)}>
                Pricing
              </NavLink>
              <NavLink to="/docs" className={({ isActive }) => navClassName(isActive)}>
                Docs
              </NavLink>
              <NavLink to="/support" className={({ isActive }) => navClassName(isActive)}>
                Support
              </NavLink>
            </nav>
          </div>

          <div className="flex items-center gap-4 text-sm">
            <Link to="/dashboard" className="text-muted-foreground hover:text-foreground">
              Dashboard
            </Link>
            {useClerk ? (
              <>
                <SignedOut>
                  <Link to="/sign-in" className="font-medium hover:underline">
                    Sign in
                  </Link>
                </SignedOut>
                <SignedIn>
                  <SignOutButton>
                    <button type="button" className="font-medium hover:underline">
                      Sign out
                    </button>
                  </SignOutButton>
                </SignedIn>
              </>
            ) : null}
          </div>
        </div>
      </header>
      <main className="mx-auto w-full max-w-6xl flex-1 px-4 py-10 md:px-6">
        <Outlet />
      </main>
      <footer className="border-t">
        <div className="mx-auto flex w-full max-w-6xl flex-col gap-3 px-4 py-6 text-sm text-muted-foreground md:flex-row md:items-center md:justify-between md:px-6">
          <p>© {new Date().getFullYear()} InvoiceSys</p>
          <div className="flex flex-wrap items-center gap-4">
            <Link to="/support" className="hover:text-foreground">
              Support
            </Link>
            <Link to="/docs" className="hover:text-foreground">
              Docs
            </Link>
            <Link to="/pricing" className="hover:text-foreground">
              Pricing
            </Link>
            <a href="mailto:form-InvoiceSys@yl1.org" className="hover:text-foreground">
              Contact us
            </a>
          </div>
        </div>
      </footer>
    </div>
  );
}
