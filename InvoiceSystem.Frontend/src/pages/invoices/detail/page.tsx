import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { ArrowLeft, Printer } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import api from "@/services/api";
import type { Invoice } from "@/types";

export default function InvoiceDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [invoice, setInvoice] = useState<Invoice | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchInvoice() {
      if (!id) return;
      try {
        const response = await api.get<Invoice>(`/invoices/${id}`);
        setInvoice(response.data);
      } catch (error) {
        console.error("Failed to fetch invoice details", error);
      } finally {
        setLoading(false);
      }
    }
    fetchInvoice();
  }, [id]);

  if (loading) return <div className="p-10">Loading invoice details...</div>;
  if (!invoice) return <div className="p-10">Invoice not found</div>;

  return (
    <div className="w-full max-w-4xl mx-auto">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
        <div className="flex items-center gap-4">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => navigate("/invoices")}
          >
            <ArrowLeft className="h-5 w-5" />
          </Button>
          <div>
            <h1 className="text-2xl sm:text-3xl font-bold tracking-tight">
              Invoice {invoice.invoiceCode}
            </h1>
            <p className="text-sm sm:text-base text-muted-foreground">
              Issued on {new Date(invoice.invoiceDate).toLocaleDateString()} • Due{" "}
              {new Date(invoice.dueDate).toLocaleDateString()}
            </p>
          </div>
        </div>
        <Button
          variant="outline"
          onClick={() => window.print()}
          className="w-full sm:w-auto"
        >
          <Printer className="mr-2 h-4 w-4" /> Print
        </Button>
      </div>

      <div className="grid gap-6 md:grid-cols-2 mb-8">
        <Card>
          <CardHeader>
            <CardTitle>Client Details</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-lg font-medium">{invoice.clientName}</div>
            <div className="text-sm text-muted-foreground">
              {invoice.clientAbn}
            </div>
          </CardContent>
        </Card>
        <Card className="flex flex-col justify-center items-start md:items-end p-6">
          <div className="text-sm text-muted-foreground">Total Amount</div>
          <div className="text-4xl font-bold">
            {new Intl.NumberFormat("en-AU", {
              style: "currency",
              currency: "AUD",
            }).format(Number(invoice.totalAmount) || 0)}
          </div>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Line Items</CardTitle>
        </CardHeader>
        <CardContent className="overflow-x-auto">
          <Table className="min-w-[600px]">
            <TableHeader>
              <TableRow>
                <TableHead>Product</TableHead>
                <TableHead className="text-right">Price</TableHead>
                <TableHead className="text-right">Qty</TableHead>
                <TableHead className="text-right">Total</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {invoice.items?.map((item) => (
                <TableRow key={item.productId}>
                  <TableCell className="font-medium">
                    {item.productName}
                  </TableCell>
                  <TableCell className="text-right">
                    {new Intl.NumberFormat("en-AU", {
                      style: "currency",
                      currency: "AUD",
                    }).format(Number(item.price) || 0)}
                  </TableCell>
                  <TableCell className="text-right">{item.quantity}</TableCell>
                  <TableCell className="text-right font-medium">
                    {new Intl.NumberFormat("en-AU", {
                      style: "currency",
                      currency: "AUD",
                    }).format(item.total)}
                  </TableCell>
                </TableRow>
              ))}
              {(!invoice.items || invoice.items.length === 0) && (
                <TableRow>
                  <TableCell
                    colSpan={4}
                    className="text-center text-muted-foreground"
                  >
                    No items found
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  );
}
