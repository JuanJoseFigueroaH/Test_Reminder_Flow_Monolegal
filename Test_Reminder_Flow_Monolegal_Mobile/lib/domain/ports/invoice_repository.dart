import 'package:dartz/dartz.dart';
import 'package:reminder_flow_mobile/domain/entities/invoice.dart';
import 'package:reminder_flow_mobile/domain/entities/reminder_result.dart';

abstract class InvoiceRepository {
  Future<Either<String, InvoiceListResponse>> getAll();
  Future<Either<String, Invoice>> getById(String id);
  Future<Either<String, InvoiceListResponse>> getByClient(String clientId);
  Future<Either<String, ProcessRemindersResponse>> processReminders();
  Future<Either<String, Invoice>> create(Map<String, dynamic> invoiceData);
}
