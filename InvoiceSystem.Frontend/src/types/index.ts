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
  clientId: string;
  clientName: string;
  clientAbn: string;
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
  clientId: string;
  items: CreateInvoiceItemRequest[];
}

export interface CreateInvoiceItemRequest {
  productId: string;
  quantity: number;
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
