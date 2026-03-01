import { zodResolver } from "@hookform/resolvers/zod";
import { ChevronDown } from "lucide-react";
import { useForm } from "react-hook-form";
import { Link } from "react-router-dom";
import * as z from "zod";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";

const supportFormSchema = z.object({
  name: z
    .string()
    .min(1, "Name is required")
    .max(100, "Name must be less than 100 characters"),
  email: z.string().email("Please enter a valid email address"),
  message: z
    .string()
    .min(10, "Message must be at least 10 characters")
    .max(2000, "Message must be less than 2000 characters"),
});

type SupportFormValues = z.infer<typeof supportFormSchema>;

const supportMailbox = "form-InvoiceSys@yl1.org";
const faqs = [
  {
    question: "How do I create my first invoice?",
    answer:
      "Go to Dashboard -> Invoices -> Create. Fill client, product, and quantity, then save to generate your first invoice in seconds.",
  },
  {
    question: "How does sign-in and account access work?",
    answer:
      "Use your account on the sign-in page. After login, you can access your dashboard and manage invoices, clients, and products securely.",
  },
  {
    question: "How can I update clients and products?",
    answer:
      "Open Clients or Products from the dashboard, select the item, and use Edit to update details. Changes are saved immediately.",
  },
];

export default function SupportPage() {
  const form = useForm<SupportFormValues>({
    resolver: zodResolver(supportFormSchema),
    defaultValues: {
      name: "",
      email: "",
      message: "",
    },
  });

  function onSubmit(data: SupportFormValues) {
    const subject = encodeURIComponent(`InvoiceSys support request from ${data.name}`);
    const body = encodeURIComponent(
      `Name: ${data.name}\nEmail: ${data.email}\n\nMessage:\n${data.message}`,
    );

    window.location.href = `mailto:${supportMailbox}?subject=${subject}&body=${body}`;
  }

  return (
    <section className="mx-auto max-w-5xl space-y-8">
      <div className="space-y-2 text-center">
        <h1 className="text-3xl font-bold tracking-tight">Support</h1>
        <p className="text-muted-foreground">
          Tell us what happened and we will prepare an email with your request details.
        </p>
        <p className="text-sm font-medium text-muted-foreground">
          We usually reply within 24 hours.
        </p>
      </div>

      <div className="grid items-start gap-6 md:grid-cols-[300px_1fr] md:gap-8">
        <aside className="rounded-xl border bg-card p-5 shadow-sm">
          <h2 className="text-lg font-semibold">Need help quickly?</h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Check common questions first, or contact us directly if your issue is urgent.
          </p>

          <div className="mt-5 space-y-2 text-sm">
            <h3 className="font-medium">FAQ</h3>
            <div className="space-y-2">
              {faqs.map((faq) => (
                <details
                  key={faq.question}
                  className="group rounded-md border bg-background p-5 transition-all hover:border-slate-400 hover:shadow-sm open:border-slate-400 dark:hover:border-slate-600 dark:open:border-slate-600"
                >
                  <summary className="flex cursor-pointer list-none items-center justify-between gap-2 rounded-md px-2 py-2 text-sm font-medium text-foreground transition-colors hover:bg-slate-200 marker:content-none dark:hover:bg-slate-700/70">
                    <span>{faq.question}</span>
                    <ChevronDown className="h-4 w-4 text-muted-foreground transition-transform group-open:rotate-180" />
                  </summary>
                  <p className="mt-2 text-sm text-muted-foreground">{faq.answer}</p>
                  <p className="mt-2 text-xs font-medium text-muted-foreground">
                    Info: You can still contact support if this does not solve your issue.
                  </p>
                </details>
              ))}
            </div>
            <Link to="/docs" className="inline-block text-sm text-muted-foreground hover:text-foreground hover:underline">
              Read full documentation
            </Link>
          </div>
        </aside>

        <div className="rounded-xl border bg-card p-5 shadow-sm md:p-6">
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-5">
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Name</FormLabel>
                    <FormControl>
                      <Input
                        placeholder="Your name"
                        className="hover:border-foreground/40 focus-visible:ring-2"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Email</FormLabel>
                    <FormControl>
                      <Input
                        type="email"
                        placeholder="you@example.com"
                        className="hover:border-foreground/40 focus-visible:ring-2"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="message"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Message</FormLabel>
                    <FormControl>
                      <textarea
                        className="flex min-h-32 w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-sm transition-colors placeholder:text-muted-foreground hover:border-foreground/40 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
                        placeholder="Describe your issue..."
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <div className="border-t pt-4">
                <Button
                  type="submit"
                  className="h-10 min-w-44 p-5 cursor-pointer"
                >
                  Send support request
                </Button>
                <p className="mt-3 text-sm text-muted-foreground">
                  Prefer manual email?{" "}
                  <a
                    href={`mailto:${supportMailbox}`}
                    className="font-medium text-foreground hover:underline"
                  >
                    Send directly to {supportMailbox}
                  </a>
                </p>
              </div>
            </form>
          </Form>
        </div>
      </div>
    </section>
  );
}
