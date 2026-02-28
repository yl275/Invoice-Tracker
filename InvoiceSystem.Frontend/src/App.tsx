import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { SignedIn, SignedOut, SignIn, SignUp } from "@clerk/clerk-react";
import { MainLayout } from "./layouts/MainLayout";
import InvoicesPage from "./pages/invoices/page";
import CreateInvoicePage from "./pages/invoices/create/page";
import InvoiceDetailPage from "./pages/invoices/detail/page";
import ClientsPage from "./pages/clients/page";
import CreateClientPage from "./pages/clients/create/page";
import EditClientPage from "./pages/clients/edit/page";
import ProductsPage from "./pages/products/page";
import CreateProductPage from "./pages/products/create/page";
import EditProductPage from "./pages/products/edit/page";
import { AuthSetup } from "./components/AuthSetup";

const publishableKey = import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;

function App() {
  return (
    <BrowserRouter>
      {publishableKey && <AuthSetup />}
      <Routes>
        {publishableKey && (
          <>
            <Route
              path="/sign-in/*"
              element={
                <div className="flex min-h-screen items-center justify-center">
                  <SignIn />
                </div>
              }
            />
            <Route path="/sign-up/*" element={<SignUp routing="path" path="/sign-up" signInUrl="/sign-in" />} />
          </>
        )}
        <Route path="/" element={publishableKey ? <RequireAuth><MainLayout /></RequireAuth> : <MainLayout />}>
          <Route index element={<Navigate to="/invoices" replace />} />
          <Route path="invoices" element={<InvoicesPage />} />
          <Route path="invoices/create" element={<CreateInvoicePage />} />
          <Route path="invoices/:id" element={<InvoiceDetailPage />} />
          <Route path="clients" element={<ClientsPage />} />
          <Route path="clients/create" element={<CreateClientPage />} />
          <Route path="clients/:id/edit" element={<EditClientPage />} />
          <Route path="products" element={<ProductsPage />} />
          <Route path="products/create" element={<CreateProductPage />} />
          <Route path="products/:id/edit" element={<EditProductPage />} />
          <Route path="*" element={<div>Not Found</div>} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

function RequireAuth({ children }: { children: React.ReactNode }) {
  return (
    <>
      <SignedIn>{children}</SignedIn>
      <SignedOut>
        <Navigate to="/sign-in" replace />
      </SignedOut>
    </>
  );
}

export default App;
