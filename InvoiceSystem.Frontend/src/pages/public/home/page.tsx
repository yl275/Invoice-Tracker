import { SignedIn, SignedOut } from "@clerk/clerk-react";
import { CalendarDays, Printer, Save, Workflow } from "lucide-react";
import { Link } from "react-router-dom";
import { BentoCard, BentoGrid } from "@/components/ui/bento-grid";
import TextType from "@/components/TextType";

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
      <div className="absolute inset-0 bg-gradient-to-br from-slate-100 to-transparent dark:from-slate-800/50" />
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
      <div className="absolute inset-0 bg-gradient-to-br from-indigo-100/70 to-transparent dark:from-indigo-900/30" />
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
      <div className="absolute inset-0 bg-gradient-to-br from-emerald-100/70 to-transparent dark:from-emerald-900/30" />
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
      <div className="absolute inset-0 bg-gradient-to-br from-amber-100/70 to-transparent dark:from-amber-900/30" />
    ),
  },
];

export default function HomePage() {
  return (
    <section className="-mx-4 snap-y snap-mandatory md:-mx-6">
      <div className="flex min-h-screen snap-start items-center justify-center bg-gradient-to-br from-slate-950 via-black to-slate-900">
        <div className="flex flex-col items-center justify-center space-y-4 text-center">
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
        </div>
      </div>

      <div className="flex min-h-screen snap-start items-center bg-white py-8">
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

      <div className="flex min-h-screen snap-start items-center justify-center bg-gradient-to-br from-slate-950 via-black to-slate-900 py-10 text-center">
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
