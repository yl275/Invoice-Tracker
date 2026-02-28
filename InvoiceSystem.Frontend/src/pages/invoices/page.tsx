import { useEffect, useState } from "react";
import type { Invoice } from "@/types";
import api from "@/services/api";
import { columns } from "./columns";
import { DataTable } from "@/components/ui/data-table";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { Link } from "react-router-dom";

export default function InvoicesPage() {
  const [data, setData] = useState<Invoice[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchData() {
      try {
        const response = await api.get<Invoice[]>("/invoices");
        setData(response.data);
      } catch (error) {
        console.error("Failed to fetch invoices", error);
      } finally {
        setLoading(false);
      }
    }

    fetchData();
  }, []);

  return (
    <div className="container mx-auto py-10">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
        <h1 className="text-3xl font-bold tracking-tight">Invoices</h1>
        <Link to="/invoices/create" className="w-full sm:w-auto">
          <Button className="w-full sm:w-auto transition-transform hover:scale-105 active:scale-95">
            <Plus className="mr-2 h-4 w-4" /> Create Invoice
          </Button>
        </Link>
      </div>

      {loading ? (
        <div>Loading...</div>
      ) : (
        <DataTable columns={columns} data={data} />
      )}
    </div>
  );
}
