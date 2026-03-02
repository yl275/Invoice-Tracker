import { useEffect, useState } from "react";
import { Link, Outlet, useLocation } from "react-router-dom";
import { UserButton } from "@clerk/clerk-react";
import { FileText, Users, Package, Menu, X, UserRound, Settings, Moon, Sun } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { getInitialTheme, setTheme, type Theme } from "@/theme";
import api from "@/services/api";

const useClerk = !!import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;

type SidebarProps = React.HTMLAttributes<HTMLDivElement>;

export function MainLayout({ className }: SidebarProps) {
  const location = useLocation();
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isSettingsOpen, setIsSettingsOpen] = useState(false);
  const [theme, setThemeState] = useState<Theme>("light");
  const [seeding, setSeeding] = useState(false);
  const [seedMessage, setSeedMessage] = useState<string | null>(null);

  useEffect(() => {
    setThemeState(getInitialTheme());
  }, []);

  const handleThemeChange = (next: Theme) => {
    setThemeState(next);
    setTheme(next);
  };

  const handleSeedData = async () => {
    setSeeding(true);
    setSeedMessage(null);
    try {
      const response = await api.post("/dev/seed");
      const data = response.data as {
        clientsCreated?: number;
        productsCreated?: number;
        invoicesCreated?: number;
      };
      setSeedMessage(
        `Seeded ${data.clientsCreated ?? 0} clients, ${data.productsCreated ?? 0} products, ${data.invoicesCreated ?? 0} invoices.`,
      );
    } catch (error) {
      console.error("Failed to seed data", error);
      setSeedMessage("Failed to seed data. See console for details.");
    } finally {
      setSeeding(false);
    }
  };

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
      <aside className="border-r bg-slate-50/40 dark:bg-slate-900/40 hidden md:flex md:flex-col">
        <div className="p-6 flex items-center justify-between">
          <Link to="/" className="text-2xl font-bold tracking-tight hover:opacity-80">
            InvoiceSys
          </Link>
          {useClerk && <UserButton afterSignOutUrl="/sign-in" />}
        </div>
        <div className="px-4 py-2 flex-1 flex flex-col">
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

          <div className="mt-auto pt-4">
            <Dialog open={isSettingsOpen} onOpenChange={setIsSettingsOpen}>
              <Button
                variant="ghost"
                className="w-full justify-start text-sm text-muted-foreground"
                onClick={() => setIsSettingsOpen(true)}
              >
                <Settings className="mr-2 h-4 w-4" />
                Settings
              </Button>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Settings</DialogTitle>
                  <DialogDescription>
                    Personalize how InvoiceSys looks on this device.
                  </DialogDescription>
                </DialogHeader>
                <div className="mt-4 space-y-4">
                  <div className="space-y-2">
                    <p className="text-sm font-medium">Theme</p>
                    <div className="inline-flex rounded-md border bg-muted/40 p-1">
                      <Button
                        type="button"
                        size="sm"
                        variant={theme === "light" ? "default" : "ghost"}
                        className="gap-1"
                        onClick={() => handleThemeChange("light")}
                      >
                        <Sun className="h-4 w-4" />
                        <span>Light</span>
                      </Button>
                      <Button
                        type="button"
                        size="sm"
                        variant={theme === "dark" ? "default" : "ghost"}
                        className="gap-1"
                        onClick={() => handleThemeChange("dark")}
                      >
                        <Moon className="h-4 w-4" />
                        <span>Dark</span>
                      </Button>
                    </div>
                  </div>

p[]                  <div className="space-y-2 pt-4 border-t">
                    <p className="text-sm font-medium">Developer</p>
                    <Button
                      type="button"
                      size="sm"
                      variant="outline"
                      onClick={handleSeedData}
                      disabled={seeding}
                    >
                      {seeding ? "Seeding dummy data..." : "Seed dummy data"}
                    </Button>
                    {seedMessage && (
                      <p className="text-xs text-muted-foreground">{seedMessage}</p>
                    )}
                  </div>
                </div>
              </DialogContent>
            </Dialog>
          </div>
        </div>
      </aside>
      <main className="p-4 md:p-8">
        <Outlet />
      </main>
    </div>
  );
}
