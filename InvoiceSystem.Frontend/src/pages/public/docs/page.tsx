export default function DocsPage() {
  return (
    <section className="space-y-6">
      <h1 className="text-3xl font-bold tracking-tight">Docs</h1>
      <p className="text-muted-foreground">
        Find guides for account setup, invoice lifecycle, and API integrations.
      </p>
      <div className="space-y-4">
        <article className="rounded-lg border p-5">
          <h2 className="font-semibold">Getting Started</h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Set up your workspace, create your first client, and send your first invoice.
          </p>
        </article>
        <article className="rounded-lg border p-5">
          <h2 className="font-semibold">Authentication</h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Learn how sign-in works and how user identity is attached to API requests.
          </p>
        </article>
        <article className="rounded-lg border p-5">
          <h2 className="font-semibold">API Reference</h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Use the backend API to manage invoices, products, and clients programmatically.
          </p>
        </article>
      </div>
    </section>
  );
}
