import 'package:equatable/equatable.dart';

enum InvoiceStatus {
  pendiente,
  primerrecordatorio,
  segundorecordatorio,
  desactivado,
}

extension InvoiceStatusExtension on InvoiceStatus {
  String get displayName {
    switch (this) {
      case InvoiceStatus.pendiente:
        return 'Pendiente';
      case InvoiceStatus.primerrecordatorio:
        return 'Primer Recordatorio';
      case InvoiceStatus.segundorecordatorio:
        return 'Segundo Recordatorio';
      case InvoiceStatus.desactivado:
        return 'Desactivado';
    }
  }

  static InvoiceStatus fromString(String value) {
    switch (value) {
      case 'pendiente':
        return InvoiceStatus.pendiente;
      case 'primerrecordatorio':
        return InvoiceStatus.primerrecordatorio;
      case 'segundorecordatorio':
        return InvoiceStatus.segundorecordatorio;
      case 'desactivado':
        return InvoiceStatus.desactivado;
      default:
        return InvoiceStatus.pendiente;
    }
  }
}

class Invoice extends Equatable {
  final String id;
  final String clientId;
  final String invoiceNumber;
  final double amount;
  final InvoiceStatus status;
  final DateTime dueDate;
  final DateTime createdAt;
  final DateTime updatedAt;
  final String? clientName;
  final String? clientEmail;

  const Invoice({
    required this.id,
    required this.clientId,
    required this.invoiceNumber,
    required this.amount,
    required this.status,
    required this.dueDate,
    required this.createdAt,
    required this.updatedAt,
    this.clientName,
    this.clientEmail,
  });

  factory Invoice.fromJson(Map<String, dynamic> json) {
    return Invoice(
      id: json['id'] ?? '',
      clientId: json['client_id'] ?? '',
      invoiceNumber: json['invoice_number'] ?? '',
      amount: (json['amount'] ?? 0).toDouble(),
      status: InvoiceStatusExtension.fromString(json['status'] ?? ''),
      dueDate: DateTime.parse(json['due_date'] ?? DateTime.now().toIso8601String()),
      createdAt: DateTime.parse(json['created_at'] ?? DateTime.now().toIso8601String()),
      updatedAt: DateTime.parse(json['updated_at'] ?? DateTime.now().toIso8601String()),
      clientName: json['client_name'],
      clientEmail: json['client_email'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'client_id': clientId,
      'invoice_number': invoiceNumber,
      'amount': amount,
      'status': status.name,
      'due_date': dueDate.toIso8601String(),
      'created_at': createdAt.toIso8601String(),
      'updated_at': updatedAt.toIso8601String(),
      'client_name': clientName,
      'client_email': clientEmail,
    };
  }

  @override
  List<Object?> get props => [
        id,
        clientId,
        invoiceNumber,
        amount,
        status,
        dueDate,
        createdAt,
        updatedAt,
        clientName,
        clientEmail,
      ];
}

class InvoiceListResponse {
  final List<Invoice> invoices;
  final int total;

  InvoiceListResponse({required this.invoices, required this.total});

  factory InvoiceListResponse.fromJson(Map<String, dynamic> json) {
    return InvoiceListResponse(
      invoices: (json['invoices'] as List)
          .map((e) => Invoice.fromJson(e))
          .toList(),
      total: json['total'] ?? 0,
    );
  }
}
