import 'package:flutter_test/flutter_test.dart';
import 'package:reminder_flow_mobile/domain/entities/invoice.dart';

void main() {
  group('Invoice', () {
    test('should create Invoice from JSON', () {
      final json = {
        'id': '123',
        'client_id': 'c1',
        'invoice_number': 'INV-001',
        'amount': 150000.0,
        'status': 'primerrecordatorio',
        'due_date': '2024-01-15T00:00:00.000Z',
        'created_at': '2024-01-01T00:00:00.000Z',
        'updated_at': '2024-01-01T00:00:00.000Z',
        'client_name': 'Test Client',
        'client_email': 'test@email.com',
      };

      final invoice = Invoice.fromJson(json);

      expect(invoice.id, '123');
      expect(invoice.clientId, 'c1');
      expect(invoice.invoiceNumber, 'INV-001');
      expect(invoice.amount, 150000.0);
      expect(invoice.status, InvoiceStatus.primerrecordatorio);
      expect(invoice.clientName, 'Test Client');
      expect(invoice.clientEmail, 'test@email.com');
    });

    test('should convert Invoice to JSON', () {
      final invoice = Invoice(
        id: '123',
        clientId: 'c1',
        invoiceNumber: 'INV-001',
        amount: 150000.0,
        status: InvoiceStatus.primerrecordatorio,
        dueDate: DateTime(2024, 1, 15),
        createdAt: DateTime(2024, 1, 1),
        updatedAt: DateTime(2024, 1, 1),
        clientName: 'Test Client',
        clientEmail: 'test@email.com',
      );

      final json = invoice.toJson();

      expect(json['id'], '123');
      expect(json['client_id'], 'c1');
      expect(json['invoice_number'], 'INV-001');
      expect(json['amount'], 150000.0);
      expect(json['status'], 'primerrecordatorio');
    });
  });

  group('InvoiceStatus', () {
    test('should return correct display name for primerrecordatorio', () {
      expect(
        InvoiceStatus.primerrecordatorio.displayName,
        'Primer Recordatorio',
      );
    });

    test('should return correct display name for segundorecordatorio', () {
      expect(
        InvoiceStatus.segundorecordatorio.displayName,
        'Segundo Recordatorio',
      );
    });

    test('should return correct display name for desactivado', () {
      expect(
        InvoiceStatus.desactivado.displayName,
        'Desactivado',
      );
    });

    test('should parse status from string', () {
      expect(
        InvoiceStatusExtension.fromString('primerrecordatorio'),
        InvoiceStatus.primerrecordatorio,
      );
      expect(
        InvoiceStatusExtension.fromString('segundorecordatorio'),
        InvoiceStatus.segundorecordatorio,
      );
      expect(
        InvoiceStatusExtension.fromString('desactivado'),
        InvoiceStatus.desactivado,
      );
    });
  });
}
