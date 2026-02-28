import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
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
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useNavigate, useParams } from "react-router-dom";
import api from "@/services/api";
import { ArrowLeft } from "lucide-react";
import { useEffect } from "react";

// Checking recent file list, UseToast is likely not imported or I need to find where it is or if it exists.
// Wait, I saw "use-toast.ts" in the conversation history which implies it exists.
// Let me double check if I can import it. The conversation mentioned "use-toast.ts" in src/hooks or components.
// I will assume it is at "@/hooks/use-toast" or similar. Actually, let's stick to console.log if unsure, but better to check first?
// No, I'll stick to the pattern in "create" page but add fetch logic. Create page didn't use toast, but I should probably add it for better UX.
// I'll stick to console error for now to be safe and match "create" page, or standardise.
// "create" page had: // Ideally show toast here status.
// I will just implement the basic functionality first.

const clientFormSchema = z.object({
  name: z
    .string()
    .min(1, "Name is required")
    .max(100, "Name must be less than 100 characters"),
  abn: z
    .string()
    .min(1, "ABN is required")
    .regex(/^\d{11}$/, "ABN must be exactly 11 digits"),
  phoneNumber: z
    .string()
    .min(1, "Phone number is required")
    .regex(/^[\d\s\-+()]{8,20}$/, "Phone number format is invalid"),
});

type ClientFormValues = z.infer<typeof clientFormSchema>;

export default function EditClientPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const form = useForm<ClientFormValues>({
    resolver: zodResolver(clientFormSchema),
    defaultValues: {
      name: "",
      abn: "",
      phoneNumber: "",
    },
  });

  useEffect(() => {
    async function fetchClient() {
      if (!id) return;
      try {
        const response = await api.get(`/clients/${id}`);
        const { name, abn, phoneNumber } = response.data;
        form.reset({ name, abn, phoneNumber });
      } catch (error) {
        console.error("Failed to fetch client", error);
      }
    }
    fetchClient();
  }, [id, form]);

  async function onSubmit(data: ClientFormValues) {
    if (!id) return;
    try {
      await api.put(`/clients/${id}`, data);
      navigate("/clients");
    } catch (error) {
      console.error("Failed to update client", error);
    }
  }

  return (
    <div className="w-full max-w-2xl mx-auto">
      <div className="flex items-center gap-4 mb-8">
        <Button
          variant="ghost"
          size="icon"
          onClick={() => navigate("/clients")}
        >
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <h1 className="text-3xl font-bold tracking-tight">Edit Client</h1>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Client Information</CardTitle>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Client Name</FormLabel>
                    <FormControl>
                      <Input placeholder="Acme Corp" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="abn"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>ABN</FormLabel>
                    <FormControl>
                      <Input placeholder="12 345 678 901" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="phoneNumber"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Phone Number</FormLabel>
                    <FormControl>
                      <Input placeholder="0400 000 000" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <div className="flex justify-end gap-4">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => navigate("/clients")}
                >
                  Cancel
                </Button>
                <Button type="submit">Update Client</Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}
