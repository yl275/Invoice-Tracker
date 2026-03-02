import { useEffect, useState } from "react";
import { Link, Outlet, useLocation } from "react-router-dom";
import { UserButton } from "@clerk/clerk-react";
import { FileText, Users, Package, Menu, X, UserRound, Settings, Moon, Sun } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
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

const deleteWords = [
  "invoice",
  "client",
  "product",
  "profile",
  "workspace",
  "permanent",
  "delete",
  "confirm",
  "irreversible",
  "cleanup",
];

function generateDeletePhrase() {
  const parts: string[] = [];
  for (let i = 0; i < 5; i++) {
    const index = Math.floor(Math.random() * deleteWords.length);
    parts.push(deleteWords[index]);
  }
  return parts.join(" ");
}

export function MainLayout({ className }: SidebarProps) {
  const location = useLocation();
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isSettingsOpen, setIsSettingsOpen] = useState(false);
  const [theme, setThemeState] = useState<Theme>("light");
  const [seeding, setSeeding] = useState(false);
  const [seedMessage, setSeedMessage] = useState<string | null>(null);
  const [isPro, setIsPro] = useState(false);
  const [deletePhrase, setDeletePhrase] = useState("");
  const [deleteInput, setDeleteInput] = useState("");
  const [deleting, setDeleting] = useState(false);
  const [deleteMessage, setDeleteMessage] = useState<string | null>(null);

  useEffect(() => {
    setThemeState(getInitialTheme());
  }, []);

  useEffect(() => {
    // Fetch billing status (free / pro)
    api
      .get<{ plan: string; isPro: boolean }>("/billing/status")
      .then((res) => {
        setIsPro(Boolean(res.data?.isPro));
      })
      .catch((error) => {
        console.warn("Failed to load billing status", error);
      });
  }, []);

  const handleThemeChange = (next: Theme) => {
    setThemeState(next);
    setTheme(next);
  };

  useEffect(() => {
    if (isSettingsOpen && !deletePhrase) {
      setDeletePhrase(generateDeletePhrase());
      setDeleteInput("");
      setDeleteMessage(null);
    }
  }, [isSettingsOpen, deletePhrase]);

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

  const handleDeleteEverything = async () => {
    setDeleting(true);
    setDeleteMessage(null);
    try {
      const response = await api.delete("/dev/everything");
      const data = response.data as {
        invoicesDeleted?: number;
        clientsDeleted?: number;
        productsDeleted?: number;
        profilesDeleted?: number;
      };
      setDeleteMessage(
        `Deleted ${data.invoicesDeleted ?? 0} invoices, ${data.clientsDeleted ?? 0} clients, ${data.productsDeleted ?? 0} products, ${data.profilesDeleted ?? 0} profiles.`,
      );
    } catch (error) {
      console.error("Failed to delete data", error);
      setDeleteMessage("Failed to delete data. See console for details.");
    } finally {
      setDeleting(false);
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

          <div className="mt-auto pt-4 space-y-2">
            {!isPro ? (
              <Button
                type="button"
                variant="outline"
                className="w-full justify-center text-sm font-semibold"
                onClick={async () => {
                  try {
                    const res = await api.post<{ url: string }>("/billing/create-checkout-session");
                    if (res.data?.url) {
                      window.location.href = res.data.url;
                    }
                  } catch (error) {
                    console.error("Failed to start checkout", error);
                    alert("Failed to start Stripe checkout. Please try again.");
                  }
                }}
              >
                Upgrade to Pro
              </Button>
            ) : (
              <p className="text-xs font-semibold text-emerald-500">
                You are Pro
              </p>
            )}
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

                  <div className="space-y-4 pt-4 border-t">
                    <div className="space-y-2">
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

                    <div className="space-y-2 rounded-md bg-destructive/5 p-3">
                      <p className="text-sm font-semibold text-destructive">
                        Delete everything
                      </p>
                      <p className="text-xs text-muted-foreground">
                        Permanently delete all clients, products, invoices, and business profile
                        for this account. This action cannot be undone.
                      </p>
                      {deletePhrase && (
                        <p className="text-xs font-mono text-muted-foreground">
                          Type this to confirm:{" "}
                          <span className="break-all select-none">{deletePhrase}</span>
                        </p>
                      )}
                      <Input
                        type="text"
                        value={deleteInput}
                        onChange={(e) => setDeleteInput(e.target.value)}
                        placeholder="Type the confirmation phrase exactly"
                        className="h-8 text-xs"
                      />
                      <div className="flex items-center gap-2">
                        <Button
                          type="button"
                          size="sm"
                          variant="destructive"
                          disabled={
                            deleting ||
                            !deletePhrase ||
                            deleteInput.trim() !== deletePhrase.trim()
                          }
                          onClick={handleDeleteEverything}
                        >
                          {deleting ? "Deleting..." : "Delete everything"}
                        </Button>
                        <Button
                          type="button"
                          size="sm"
                          variant="outline"
                          className="text-xs hover:bg-muted/80"
                          onClick={() => {
                            setDeletePhrase(generateDeletePhrase());
                            setDeleteInput("");
                            setDeleteMessage(null);
                          }}
                        >
                          New phrase
                        </Button>
                      </div>
                      {deleteMessage && (
                        <p className="text-xs text-muted-foreground">{deleteMessage}</p>
                      )}
                    </div>
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
