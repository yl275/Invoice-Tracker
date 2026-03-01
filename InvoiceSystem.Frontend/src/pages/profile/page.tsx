import { useEffect, useMemo, useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import * as z from "zod";
import api from "@/services/api";
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import type { BusinessProfile, UpsertBusinessProfileRequest } from "@/types";

const paymentMethodValues = ["BankTransfer", "PayId"] as const;

const profileFormSchema = z
  .object({
    name: z.string().min(1, "Name is required"),
    email: z.string().email("Email is invalid"),
    phone: z.string().min(1, "Phone is required"),
    postalLocation: z.string().min(1, "Postal location is required"),
    website: z.string().url("Website must be a valid URL").or(z.literal("")),
    abn: z.string().min(1, "ABN is required"),
    paymentMethod: z.enum(paymentMethodValues),
    bankBsb: z.string().optional(),
    bankAccountNumber: z.string().optional(),
    payId: z.string().optional(),
  })
  .superRefine((values, ctx) => {
    if (values.paymentMethod === "BankTransfer") {
      if (!values.bankBsb?.trim()) {
        ctx.addIssue({
          path: ["bankBsb"],
          code: z.ZodIssueCode.custom,
          message: "BSB is required for bank transfer",
        });
      }
      if (!values.bankAccountNumber?.trim()) {
        ctx.addIssue({
          path: ["bankAccountNumber"],
          code: z.ZodIssueCode.custom,
          message: "Account number is required for bank transfer",
        });
      }
    }

    if (values.paymentMethod === "PayId") {
      const payId = values.payId?.trim() ?? "";
      const isEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(payId);
      const isPhone = /^[\d\s\-+()]{8,20}$/.test(payId);
      if (!payId || (!isEmail && !isPhone)) {
        ctx.addIssue({
          path: ["payId"],
          code: z.ZodIssueCode.custom,
          message: "PayId must be a valid phone number or email",
        });
      }
    }
  });

type ProfileFormValues = z.infer<typeof profileFormSchema>;

export default function ProfilePage() {
  const [loading, setLoading] = useState(false);
  const form = useForm<ProfileFormValues>({
    resolver: zodResolver(profileFormSchema),
    defaultValues: {
      name: "",
      email: "",
      phone: "",
      postalLocation: "",
      website: "",
      abn: "",
      paymentMethod: "BankTransfer",
      bankBsb: "",
      bankAccountNumber: "",
      payId: "",
    },
  });

  const paymentMethod = form.watch("paymentMethod");

  useEffect(() => {
    async function fetchProfile() {
      try {
        const response = await api.get<BusinessProfile>("/business-profile");
        const data = response.data;
        form.reset({
          name: data.name,
          email: data.email,
          phone: data.phone,
          postalLocation: data.postalLocation,
          website: data.website ?? "",
          abn: data.abn,
          paymentMethod: data.paymentMethod,
          bankBsb: data.bankBsb ?? "",
          bankAccountNumber: data.bankAccountNumber ?? "",
          payId: data.payId ?? "",
        });
      } catch (error: unknown) {
        // 404 means profile is not created yet; keep defaults.
        const status = (error as { response?: { status?: number } })?.response?.status;
        if (status !== 404) {
          console.error("Failed to load business profile", error);
        }
      }
    }
    fetchProfile();
  }, [form]);

  const pageDescription = useMemo(() => {
    if (paymentMethod === "PayId") {
      return "Set your personal invoice header and receive payment via PayId (phone or email).";
    }
    return "Set your personal invoice header and receive payment via bank transfer.";
  }, [paymentMethod]);

  async function onSubmit(values: ProfileFormValues) {
    setLoading(true);
    try {
      const payload: UpsertBusinessProfileRequest = {
        name: values.name,
        email: values.email,
        phone: values.phone,
        postalLocation: values.postalLocation,
        website: values.website || null,
        abn: values.abn,
        paymentMethod: values.paymentMethod,
        bankBsb:
          values.paymentMethod === "BankTransfer" ? values.bankBsb?.trim() || null : null,
        bankAccountNumber:
          values.paymentMethod === "BankTransfer"
            ? values.bankAccountNumber?.trim() || null
            : null,
        payId: values.paymentMethod === "PayId" ? values.payId?.trim() || null : null,
      };

      await api.put("/business-profile", payload);
    } catch (error) {
      console.error("Failed to save business profile", error);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="mx-auto w-full max-w-3xl space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Business Profile</h1>
        <p className="text-muted-foreground">{pageDescription}</p>
      </div>

      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          <div className="grid gap-4 md:grid-cols-2">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Name</FormLabel>
                  <FormControl>
                    <Input placeholder="Your business name" {...field} />
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
                    <Input placeholder="ABN number" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          <div className="grid gap-4 md:grid-cols-2">
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email</FormLabel>
                  <FormControl>
                    <Input type="email" placeholder="billing@company.com" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="phone"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Phone</FormLabel>
                  <FormControl>
                    <Input placeholder="0400 000 000" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          <FormField
            control={form.control}
            name="postalLocation"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Postal Location</FormLabel>
                <FormControl>
                  <Input placeholder="123 Main St, Sydney NSW 2000" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="website"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Website (optional)</FormLabel>
                <FormControl>
                  <Input placeholder="https://your-site.com" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="paymentMethod"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Payment Method</FormLabel>
                <Select onValueChange={field.onChange} defaultValue={field.value}>
                  <FormControl>
                    <SelectTrigger>
                      <SelectValue placeholder="Select payment method" />
                    </SelectTrigger>
                  </FormControl>
                  <SelectContent>
                    <SelectItem value="BankTransfer">Bank transfer</SelectItem>
                    <SelectItem value="PayId">PayId</SelectItem>
                  </SelectContent>
                </Select>
                <FormMessage />
              </FormItem>
            )}
          />

          {paymentMethod === "BankTransfer" ? (
            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="bankBsb"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>BSB</FormLabel>
                    <FormControl>
                      <Input placeholder="123-456" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="bankAccountNumber"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Account Number</FormLabel>
                    <FormControl>
                      <Input placeholder="123456789" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
          ) : (
            <FormField
              control={form.control}
              name="payId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>PayId (phone or email)</FormLabel>
                  <FormControl>
                    <Input placeholder="0400000000 or payid@yourcompany.com" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          )}

          <div className="flex justify-end">
            <Button type="submit" disabled={loading}>
              {loading ? "Saving..." : "Save profile"}
            </Button>
          </div>
        </form>
      </Form>
    </div>
  );
}
