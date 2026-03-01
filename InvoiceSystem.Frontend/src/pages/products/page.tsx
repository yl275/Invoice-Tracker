import { useEffect, useState } from "react";
import type { Product } from "@/types";
import api from "@/services/api";
import { createColumns } from "./columns";
import { DataTable } from "@/components/ui/data-table";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";

import { Link } from "react-router-dom";

export default function ProductsPage() {
  const [data, setData] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchData() {
      try {
        const response = await api.get<Product[]>("/products");
        setData(response.data);
      } catch (error) {
        console.error("Failed to fetch products", error);
      } finally {
        setLoading(false);
      }
    }

    fetchData();
  }, []);

  async function handleDelete(productId: string) {
    const target = data.find((p) => p.id === productId);
    const label = target?.name ? ` "${target.name}"` : "";
    const confirmed = window.confirm(`Delete product${label}?`);
    if (!confirmed) return;

    try {
      await api.delete(`/products/${productId}`);
      setData((prev) => prev.filter((item) => item.id !== productId));
    } catch (error) {
      console.error("Failed to delete product", error);
    }
  }

  return (
    <div className="container mx-auto py-10">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
        <h1 className="text-3xl font-bold tracking-tight">Products</h1>
        <Link to="/products/create" className="w-full sm:w-auto">
          <Button className="w-full sm:w-auto transition-transform hover:scale-105 active:scale-95">
            <Plus className="mr-2 h-4 w-4" /> Add Product
          </Button>
        </Link>
      </div>

      {loading ? (
        <div>Loading...</div>
      ) : (
        <DataTable columns={createColumns(handleDelete)} data={data} />
      )}
    </div>
  );
}
