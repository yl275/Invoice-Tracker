import { useEffect, useState } from "react";
import type { Client } from "@/types";
import api from "@/services/api";
import { columns } from "./columns";
import { DataTable } from "@/components/ui/data-table";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { Link } from "react-router-dom";

export default function ClientsPage() {
  const [data, setData] = useState<Client[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchData() {
      try {
        const response = await api.get<Client[]>("/clients");
        setData(response.data);
      } catch (error) {
        console.error("Failed to fetch clients", error);
      } finally {
        setLoading(false);
      }
    }

    fetchData();
  }, []);

  return (
    <div className="container mx-auto py-10">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
        <h1 className="text-3xl font-bold tracking-tight">Clients</h1>
        <Link to="/clients/create" className="w-full sm:w-auto">
          <Button className="w-full sm:w-auto transition-transform hover:scale-105 active:scale-95">
            <Plus className="mr-2 h-4 w-4" /> Add Client
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
