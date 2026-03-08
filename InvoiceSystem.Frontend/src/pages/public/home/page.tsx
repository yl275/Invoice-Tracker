import { SignedIn, SignedOut } from "@clerk/clerk-react";
import { CalendarDays, Printer, Save, Workflow } from "lucide-react";
import { Link, useNavigate } from "react-router-dom";
import { BentoCard, BentoGrid } from "@/components/ui/bento-grid";
import TextType from "@/components/TextType";
import { DemoLoginButton } from "@/components/DemoLoginButton";

const useClerk = !!import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;
const ctaButtonClassName =
  "inline-flex items-center rounded-md bg-foreground px-4 py-2 text-sm font-medium text-background hover:opacity-90";
const pricingButtonClassName =
  "inline-flex items-center rounded-md px-3 py-1.5 text-sm font-semibold";

const bentoItems = [
  {
    name: "Print",
    description: "Generate clean invoice print views for clients and internal records.",
    href: "/docs",
    cta: "Learn more",
    Icon: Printer,
    className: "md:col-span-2",
    background: (
      <div className="absolute inset-0 bg-gradient-to-br from-slate-100 to-slate-50 dark:from-slate-800/80 dark:to-slate-950" />
    ),
  },
  {
    name: "Workflow",
    description: "Keep billing operations moving from draft to sent to paid.",
    href: "/docs",
    cta: "Learn more",
    Icon: Workflow,
    className: "md:col-span-1",
    background: (
      <div className="absolute inset-0 bg-gradient-to-br from-indigo-100/70 to-slate-50 dark:from-indigo-800/75 dark:to-slate-950" />
    ),
  },
  {
    name: "Save",
    description: "Store invoice and client updates safely with quick edit access.",
    href: "/docs",
    cta: "Learn more",
    Icon: Save,
    className: "md:col-span-1",
    background: (
      <div className="absolute inset-0 bg-gradient-to-br from-emerald-100/70 to-slate-50 dark:from-emerald-800/75 dark:to-slate-950" />
    ),
  },
  {
    name: "Calendar",
    description: "Use the calendar to filter your files by date.",
    href: "/docs",
    cta: "Learn more",
    Icon: CalendarDays,
    className: "md:col-span-2",
    background: (
      <div className="absolute inset-0 bg-gradient-to-br from-amber-100/70 to-slate-50 dark:from-amber-800/75 dark:to-slate-950" />
    ),
  },
];

export default function HomePage() {
  const navigate = useNavigate();

  return (
    <section className="snap-y snap-mandatory">
      {/* Hero: content + demo straddling into bento (Supademo-style) */}
      <div className="relative flex min-h-screen snap-start flex-col bg-gradient-to-br from-slate-950 via-black to-slate-900 pt-24 pb-0 md:pt-28">
        <div className="flex flex-1 flex-col items-center justify-center space-y-4 text-center">
          <h1 className="min-h-20 p-4 text-3xl font-bold tracking-tight text-white md:min-h-24 md:text-4xl">
            <TextType
              texts={[
                "Simple invoicing for growing teams",
                "Automate workflow and save time",
                "Scale billing with confidence",
              ]}
              typingSpeed={30}
              pauseDuration={1500}
              showCursor
              cursorCharacter="_"
              deletingSpeed={50}
              variableSpeedEnabled={false}
              variableSpeedMin={60}
              variableSpeedMax={120}
              cursorBlinkDuration={0.5}
            />
          </h1>
          <p className="max-w-2xl text-slate-300">
            Create invoices, manage clients and products, and keep everything organized in one place.
          </p>
          <div className="flex items-center gap-4 pt-2">
            {useClerk ? (
              <>
                <SignedIn>
                  <Link to="/dashboard" className={`${ctaButtonClassName} bg-white text-slate-900 hover:bg-slate-200`}>
                    Join us free
                  </Link>
                </SignedIn>
                <SignedOut>
                  <Link to="/sign-up" className={`${ctaButtonClassName} bg-white text-slate-900 hover:bg-slate-200`}>
                    Join us free
                  </Link>
                </SignedOut>
              </>
            ) : (
              <Link to="/dashboard" className={`${ctaButtonClassName} bg-white text-slate-900 hover:bg-slate-200`}>
                Join us free
              </Link>
            )}
            <Link to="/pricing" className={`${pricingButtonClassName} bg-white/15 text-white hover:bg-white/25`}>
              View pricing
            </Link>
          </div>
          <div className="pt-4 text-sm text-slate-300 space-y-1">
            <span className="mr-2">Still hesitate?</span>
            {useClerk ? (
              <span>
                <DemoLoginButton />{" "}
                <span className="ml-2 text-xs text-slate-400">
                  (demo account: demo-invoicesys / Demo1234!invoice)
                </span>
              </span>
            ) : (
              <button
                type="button"
                className="underline underline-offset-4 hover:text-white"
                onClick={() => navigate("/dashboard")}
              >
                Try this demo
              </button>
            )}
          </div>
        </div>

        {/* Demo: 65% width, 50% in hero / 50% in bento (translateY 50%) */}
        <div
          className="relative z-10 mx-auto w-[65%] translate-y-1/2 rounded-xl border border-white/10 bg-slate-900/50 shadow-2xl shadow-black/50 backdrop-blur-sm"
          style={{ aspectRatio: "2.11" }}
        >
          <iframe
            src="https://app.supademo.com/embed/cmmhi8e8u0dr8zdh1eggfqfvh?embed_v=2&utm_source=embed"
            loading="lazy"
            title="Create and Manage Invoices on invoice.yuanli.au"
            allow="clipboard-write"
            frameBorder={0}
            allowFullScreen
            className="absolute inset-0 h-full w-full rounded-xl"
          />
        </div>
      </div>

      {/* Bento: spacing from demo above and support below */}
      <div className="flex min-h-screen snap-start items-center bg-slate-100 dark:bg-slate-900 pt-[min(32vw,380px)] pb-20">
        <div className="mx-auto w-full max-w-6xl px-4 py-6 md:px-6 md:py-8">
          <BentoGrid className="grid-cols-1 auto-rows-[18rem] md:grid-cols-3">
            {bentoItems.map((item) => (
              <BentoCard
                key={item.name}
                name={item.name}
                description={item.description}
                href={item.href}
                cta={item.cta}
                Icon={item.Icon}
                className={item.className}
                background={item.background}
              />
            ))}
          </BentoGrid>
        </div>
      </div>

      <div className="flex min-h-screen snap-start items-center justify-center bg-gradient-to-br from-slate-950 via-black to-slate-900 pt-16 pb-10 text-center">
        <div>
          <h2 className="text-2xl font-semibold tracking-tight text-white">Support</h2>
          <p className="mt-2 text-slate-300">
            Regardless of membership tier, 90% of support cases will receive a response within 1 day.
          </p>
          <div className="mt-4">
            <Link to="/support" className="text-sm font-medium text-white underline underline-offset-4">
              Contact support
            </Link>
          </div>
        </div>
      </div>
    </section>
  );
}
