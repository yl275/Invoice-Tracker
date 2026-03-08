import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm, useFieldArray } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Trash2, Plus } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import api from "@/services/api";
import type { Client, Product, CreateInvoiceRequest } from "@/types";

const DUE_MODE = { DAYS: "days", DATE: "date" } as const;

const invoiceFormSchema = z
  .object({
    invoiceCode: z.string().min(1, "Invoice code is required"),
    invoiceDate: z.string().min(1, "Date is required"),
    dueMode: z.enum([DUE_MODE.DAYS, DUE_MODE.DATE]),
    dueDate: z.string().optional(),
    dueInDays: z.string().optional(),
    clientId: z.string().min(1, "Client is required"),
    items: z
      .array(
        z.object({
          productId: z.string().min(1, "Product is required"),
          quantity: z.number().min(1, "Quantity must be at least 1"),
        }),
      )
      .min(1, "At least one item is required"),
  })
  .superRefine((data, ctx) => {
    if (data.dueMode === DUE_MODE.DAYS) {
      const n = data.dueInDays ? Number(data.dueInDays) : NaN;
      if (!Number.isInteger(n) || n < 1) {
        ctx.addIssue({
          path: ["dueInDays"],
          message: "Due in days must be at least 1",
          code: z.ZodIssueCode.custom,
        });
      }
    } else {
      if (!data.dueDate || !data.dueDate.trim()) {
        ctx.addIssue({
          path: ["dueDate"],
          message: "Due date is required",
          code: z.ZodIssueCode.custom,
        });
      }
    }
  });

type InvoiceFormValues = z.infer<typeof invoiceFormSchema>;

export default function CreateInvoicePage() {
  const navigate = useNavigate();
  const [clients, setClients] = useState<Client[]>([]);
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);

  const form = useForm<InvoiceFormValues>({
    resolver: zodResolver(invoiceFormSchema),
    defaultValues: {
      invoiceCode: `INV-${Math.floor(Math.random() * 10000)}`,
      invoiceDate: new Date().toISOString().split("T")[0],
      dueMode: DUE_MODE.DAYS,
      dueDate: "",
      dueInDays: "30",
      clientId: "",
      items: [{ productId: "", quantity: 1 }],
    },
  });

  const { fields, append, remove } = useFieldArray({
    name: "items",
    control: form.control,
  });

  useEffect(() => {
    const fetchResources = async () => {
      try {
        const [clientsRes, productsRes] = await Promise.all([
          api.get<Client[]>("/clients"),
          api.get<Product[]>("/products"),
        ]);
        setClients(clientsRes.data);
        setProducts(productsRes.data);
      } catch (error) {
        console.error("Failed to load resources", error);
      }
    };
    fetchResources();
  }, []);

  async function onSubmit(data: InvoiceFormValues) {
    setLoading(true);
    try {
      const payload: CreateInvoiceRequest = {
        invoiceCode: data.invoiceCode,
        invoiceDate: new Date(data.invoiceDate).toISOString(),
        ...(data.dueMode === DUE_MODE.DATE && data.dueDate
          ? { dueDate: new Date(data.dueDate).toISOString() }
          : { dueInDays: Number(data.dueInDays) || 30 }),
        clientId: data.clientId,
        items: data.items.map((item) => ({
          productId: item.productId,
          quantity: item.quantity,
        })),
      };

      await api.post("/invoices", payload);
      navigate("/invoices");
    } catch (error) {
      console.error("Failed to create invoice", error);
      // Ideally show toast here
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="container max-w-2xl mx-auto py-10">
      <div className="mb-6">
        <h1 className="text-3xl font-bold tracking-tight">Create Invoice</h1>
        <p className="text-muted-foreground">
          Draft a new invoice for a client.
        </p>
      </div>

      <Form {...form}>
        <form
          onSubmit={form.handleSubmit((data) => onSubmit(data))}
          className="space-y-8"
        >
          <div className="grid grid-cols-2 gap-4">
            <FormField
              control={form.control}
              name="invoiceCode"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Invoice Code</FormLabel>
                  <FormControl>
                    <Input placeholder="INV-001" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="invoiceDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Date</FormLabel>
                  <FormControl>
                    <Input type="date" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="dueMode"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Due</FormLabel>
                  <Select
                    value={field.value}
                    onValueChange={(v) => {
                      field.onChange(v);
                      if (v === DUE_MODE.DAYS) form.setValue("dueDate", "");
                      else form.setValue("dueInDays", "30");
                    }}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      <SelectItem value={DUE_MODE.DAYS}>Due in X days</SelectItem>
                      <SelectItem value={DUE_MODE.DATE}>Due date</SelectItem>
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
            {form.watch("dueMode") === DUE_MODE.DAYS ? (
              <FormField
                control={form.control}
                name="dueInDays"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Due in days</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        min={1}
                        value={field.value ?? ""}
                        onChange={field.onChange}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            ) : (
              <FormField
                control={form.control}
                name="dueDate"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Due date</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}
          </div>

          <FormField
            control={form.control}
            name="clientId"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Client</FormLabel>
                <Select
                  onValueChange={field.onChange}
                  defaultValue={field.value}
                >
                  <FormControl>
                    <SelectTrigger>
                      <SelectValue placeholder="Select a client" />
                    </SelectTrigger>
                  </FormControl>
                  <SelectContent>
                    {clients.map((client) => (
                      <SelectItem key={client.id} value={client.id}>
                        {client.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                <FormMessage />
              </FormItem>
            )}
          />

          <Separator />

          <div className="space-y-4">
            <div className="flex justify-between items-center">
              <h3 className="text-lg font-medium">Items</h3>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={() => append({ productId: "", quantity: 1 })}
              >
                <Plus className="mr-2 h-4 w-4" /> Add Item
              </Button>
            </div>

            {fields.map((field, index) => (
              <div key={field.id} className="flex gap-4 items-end">
                <FormField
                  control={form.control}
                  name={`items.${index}.productId`}
                  render={({ field }) => (
                    <FormItem className="flex-1">
                      <FormLabel className={index !== 0 ? "sr-only" : ""}>
                        Product
                      </FormLabel>
                      <Select
                        onValueChange={field.onChange}
                        defaultValue={field.value}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Select product" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {products.map((product) => (
                            <SelectItem key={product.id} value={product.id}>
                              {product.name} (
                              {new Intl.NumberFormat("en-AU", {
                                style: "currency",
                                currency: "AUD",
                              }).format(product.price)}
                              )
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name={`items.${index}.quantity`}
                  render={({ field }) => (
                    <FormItem className="w-24">
                      <FormLabel className={index !== 0 ? "sr-only" : ""}>
                        Qty
                      </FormLabel>
                      <FormControl>
                        <Input
                          type="number"
                          min="1"
                          {...field}
                          onChange={(e) =>
                            field.onChange(e.target.valueAsNumber)
                          }
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  className="mb-0.5"
                  onClick={() => remove(index)}
                  disabled={fields.length === 1}
                >
                  <Trash2 className="h-4 w-4 text-destructive" />
                </Button>
              </div>
            ))}
          </div>

          <div className="flex justify-end gap-4 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate("/invoices")}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={loading}>
              {loading ? "Creating..." : "Create Invoice"}
            </Button>
          </div>
        </form>
      </Form>
    </div>
  );
}
