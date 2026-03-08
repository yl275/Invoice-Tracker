export interface Client {
  id: string;
  name: string;
  abn: string;
  phoneNumber: string;
  email?: string | null;
  comment?: string | null;
}

export interface Product {
  id: string;
  name: string;
  sku: string;
  price: number;
}

export interface InvoiceLineItem {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface Invoice {
  id: string;
  invoiceCode: string;
  invoiceDate: string;
  dueDate: string;
  clientId: string;
  clientName: string;
  clientAbn: string;
  businessName: string;
  businessAbn: string;
  businessEmail: string;
  businessPhone: string;
  businessPostalLocation: string;
  businessWebsite?: string | null;
  businessPaymentMethod: PaymentMethod | string;
  businessBankBsb?: string | null;
  businessBankAccountNumber?: string | null;
  businessPayId?: string | null;
  totalAmount: number;
  items: InvoiceItemDto[];
}

export interface InvoiceItemDto {
  productId: string;
  productName: string;
  quantity: number;
  price: number;
  total: number;
}

export interface CreateInvoiceRequest {
  invoiceCode: string;
  invoiceDate: string;
  dueDate?: string;
  dueInDays?: number;
  clientId: string;
  items: CreateInvoiceItemRequest[];
}

export interface CreateInvoiceItemRequest {
  productId: string;
  quantity: number;
}

export interface TeamDto {
  id: string;
  name: string;
  createdAt: string;
}

export interface TeamMemberDto {
  userId: string;
  role: string;
  joinedAt: string;
}

export interface TeamInvitationDto {
  id: string;
  email: string;
  expiresAt: string;
  inviteLink: string;
}

export interface CreateClientRequest {
  name: string;
  abn: string;
  phoneNumber: string;
  email?: string | null;
  comment?: string | null;
}

export interface CreateProductRequest {
  name: string;
  sku: string;
  price: number;
}

export type PaymentMethod = "BankTransfer" | "PayId";

export interface BusinessProfile {
  id: string;
  name: string;
  email: string;
  phone: string;
  postalLocation: string;
  website?: string | null;
  abn: string;
  paymentMethod: PaymentMethod;
  bankBsb?: string | null;
  bankAccountNumber?: string | null;
  payId?: string | null;
}

export interface UpsertBusinessProfileRequest {
  name: string;
  email: string;
  phone: string;
  postalLocation: string;
  website?: string | null;
  abn: string;
  paymentMethod: PaymentMethod;
  bankBsb?: string | null;
  bankAccountNumber?: string | null;
  payId?: string | null;
}
