import { useState } from "react";
import { Link, Outlet, useLocation } from "react-router-dom";
import { UserButton } from "@clerk/clerk-react";
import { FileText, Users, Package, Menu, X, UserRound } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";

const useClerk = !!import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;

type SidebarProps = React.HTMLAttributes<HTMLDivElement>;

export function MainLayout({ className }: SidebarProps) {
  const location = useLocation();
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const navItems = [
    { name: "Invoices", href: "/invoices", icon: FileText },
    { name: "Clients", href: "/clients", icon: Users },
    { name: "Products", href: "/products", icon: Package },
    { name: "Profile", href: "/profile", icon: UserRound },
  ];

  return (
    <div
      className={cn(
        "min-h-screen grid grid-cols-1 md:grid-cols-[240px_1fr]",
        className,
      )}
    >
      {/* Mobile Header */}
      <header className="md:hidden flex items-center justify-between p-4 border-b bg-background sticky top-0 z-50">
        <Link to="/" className="text-xl font-bold tracking-tight hover:opacity-80">
          InvoiceSys
        </Link>
        <div className="flex items-center gap-2">
          {useClerk && <UserButton afterSignOutUrl="/sign-in" />}
          <Button
          variant="ghost"
          size="icon"
          onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
        >
            {isMobileMenuOpen ? (
              <X className="h-6 w-6" />
            ) : (
              <Menu className="h-6 w-6" />
            )}
          </Button>
        </div>
      </header>

      {/* Mobile Menu Overlay */}
      {isMobileMenuOpen && (
        <div className="md:hidden fixed inset-0 top-[65px] z-50 bg-background p-4 border-t animate-in slide-in-from-top-4 fade-in">
          <nav className="space-y-4">
            {navItems.map((item) => (
              <Link
                key={item.href}
                to={item.href}
                onClick={() => setIsMobileMenuOpen(false)}
              >
                <Button
                  variant={
                    location.pathname === item.href ||
                    (item.href !== "/" &&
                      location.pathname.startsWith(item.href))
                      ? "secondary"
                      : "ghost"
                  }
                  className="w-full justify-start text-lg h-12"
                >
                  <item.icon className="mr-4 h-5 w-5" />
                  {item.name}
                </Button>
              </Link>
            ))}
          </nav>
        </div>
      )}

      {/* Desktop Sidebar */}
      <aside className="border-r bg-slate-50/40 dark:bg-slate-900/40 hidden md:block">
        <div className="p-6 flex items-center justify-between">
          <Link to="/" className="text-2xl font-bold tracking-tight hover:opacity-80">
            InvoiceSys
          </Link>
          {useClerk && <UserButton afterSignOutUrl="/sign-in" />}
        </div>
        <div className="px-4 py-2">
          <nav className="space-y-2">
            {navItems.map((item) => (
              <Link key={item.href} to={item.href}>
                <Button
                  variant={
                    location.pathname === item.href ||
                    (item.href !== "/" &&
                      location.pathname.startsWith(item.href))
                      ? "secondary"
                      : "ghost"
                  }
                  className="w-full justify-start"
                >
                  <item.icon className="mr-2 h-4 w-4" />
                  {item.name}
                </Button>
              </Link>
            ))}
          </nav>
        </div>
      </aside>
      <main className="p-4 md:p-8">
        <Outlet />
      </main>
    </div>
  );
}
