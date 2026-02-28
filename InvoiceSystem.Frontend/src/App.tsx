import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
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

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainLayout />}>
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

export default App;
