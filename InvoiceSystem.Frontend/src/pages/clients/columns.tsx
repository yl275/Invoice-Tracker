"use client";

import type { ColumnDef } from "@tanstack/react-table";
import type { Client } from "@/types";
import { Button } from "@/components/ui/button";
import { Pencil } from "lucide-react";
import { Link } from "react-router-dom";

export const columns: ColumnDef<Client>[] = [
  {
    accessorKey: "name",
    header: "Name",
  },
  {
    accessorKey: "abn",
    header: "ABN",
  },
  {
    accessorKey: "phoneNumber",
    header: "Phone",
  },
  {
    id: "actions",
    cell: ({ row }) => {
      const client = row.original;
      return (
        <Button variant="ghost" size="icon" asChild>
          <Link to={`/clients/${client.id}/edit`}>
            <Pencil className="h-4 w-4" />
          </Link>
        </Button>
      );
    },
  },
];
