import { Check } from "lucide-react";

export default function PricingPage() {
  return (
    <section className="space-y-6">
      <h1 className="text-3xl font-bold tracking-tight">Pricing</h1>
      <p className="text-muted-foreground">
        Start free and upgrade when your team needs advanced workflow and reporting features.
      </p>
      <div className="grid gap-4 md:grid-cols-3">
        <article className="rounded-xl border bg-card p-5">
          <h2 className="font-semibold">Starter</h2>
          <p className="mt-2 text-sm text-muted-foreground">Ideal for solo professionals and tiny teams.</p>
          <p className="mt-4 text-4xl font-bold">$0</p>
          <ul className="mt-8 space-y-1.5 text-sm">
            <li className="flex items-center gap-2">
              <Check className="h-4 w-4 text-green-600" />
              <span>1 Team Member</span>
            </li>
            <li className="flex items-center gap-2">
              <Check className="h-4 w-4 text-green-600" />
              <span>Unlimited Invoices</span>
            </li>
          </ul>
        </article>

        <article className="relative rounded-xl border-2 border-foreground bg-card p-5">
          <span className="absolute left-4 top-0 -translate-y-1/2 rounded-md bg-muted px-3 py-1 text-[10px] font-semibold uppercase tracking-wide text-muted-foreground">
            Most Popular
          </span>
          <h2 className="font-semibold">Pro</h2>
          <p className="mt-2 text-sm text-muted-foreground">For professional teams with high invoice volume.</p>
          <div className="mt-4 flex items-baseline gap-3">
            <p className="text-xl font-semibold text-muted-foreground line-through">$5</p>
            <p className="text-3xl font-bold">$1</p>
            <span className="rounded-full bg-emerald-100 px-2 py-0.5 text-xs font-semibold text-emerald-700">
              80% OFF
            </span>
          </div>
          <p className="mt-1 text-sm text-muted-foreground">One-time payment, lifetime Pro.</p>
          <ul className="mt-5 space-y-1.5 text-sm">
            <li className="flex items-center gap-2">
              <Check className="h-4 w-4 text-green-600" />
              <span>Unlimited Team Members</span>
            </li>
            <li className="flex items-center gap-2">
              <Check className="h-4 w-4 text-green-600" />
              <span>Unlimited Invoices</span>
            </li>
            <li className="flex items-center gap-2">
              <Check className="h-4 w-4 text-green-600" />
              <span>Priority Email Support</span>
            </li>
            <li className="flex items-center gap-2">
              <Check className="h-4 w-4 text-green-600" />
              <span>Workflow Automation</span>
            </li>
          </ul>
        </article>

        <article className="rounded-xl border bg-card p-5">
          <h2 className="font-semibold">Enterprise</h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Custom solutions for large-scale organizations with unique requirements.
          </p>
          <a
            href="mailto:InvoiceSys-request-demo@yl1.org?subject=Request%20a%20demo%20for%20InvoiceSys"
            className="mt-4 inline-flex w-full items-center justify-center rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700"
          >
            REQUEST A DEMO
          </a>
          <ul className="mt-5 space-y-1.5 text-sm">
            <li className="flex items-center gap-2">
              <Check className="h-4 w-4 text-green-600" />
              <span>Everything in Pro</span>
            </li>
            <li className="flex items-center gap-2">
              <Check className="h-4 w-4 text-green-600" />
              <span>Dedicated Account Manager</span>
            </li>
            <li className="flex items-center gap-2">
              <Check className="h-4 w-4 text-green-600" />
              <span>Custom API Integrations</span>
            </li>
            <li className="flex items-center gap-2">
              <Check className="h-4 w-4 text-green-600" />
              <span>Tailored Reporting</span>
            </li>
            <li className="flex items-center gap-2">
              <Check className="h-4 w-4 text-green-600" />
              <span>SSO &amp; SLA</span>
            </li>
          </ul>
        </article>
      </div>
    </section>
  );
}
