import 'package:dartz/dartz.dart';
import 'package:injectable/injectable.dart';
import 'package:reminder_flow_mobile/domain/entities/invoice.dart';
import 'package:reminder_flow_mobile/domain/entities/reminder_result.dart';
import 'package:reminder_flow_mobile/domain/ports/invoice_repository.dart';
import 'package:reminder_flow_mobile/infrastructure/datasources/api_client.dart';

@LazySingleton(as: InvoiceRepository)
class InvoiceRepositoryImpl implements InvoiceRepository {
  final ApiClient _apiClient;

  InvoiceRepositoryImpl(this._apiClient);

  @override
  Future<Either<String, InvoiceListResponse>> getAll() async {
    try {
      final response = await _apiClient.get('/invoices');
      if (response.statusCode == 200) {
        final data = response.data['data'];
        return Right(InvoiceListResponse.fromJson(data));
      }
      return Left(response.data['message'] ?? 'Error fetching invoices');
    } catch (e) {
      return Left('Error: ${e.toString()}');
    }
  }

  @override
  Future<Either<String, Invoice>> getById(String id) async {
    try {
      final response = await _apiClient.get('/invoices/$id');
      if (response.statusCode == 200) {
        final data = response.data['data'];
        return Right(Invoice.fromJson(data));
      }
      return Left(response.data['message'] ?? 'Error fetching invoice');
    } catch (e) {
      return Left('Error: ${e.toString()}');
    }
  }

  @override
  Future<Either<String, InvoiceListResponse>> getByClient(String clientId) async {
    try {
      final response = await _apiClient.get('/invoices/client/$clientId');
      if (response.statusCode == 200) {
        final data = response.data['data'];
        return Right(InvoiceListResponse.fromJson(data));
      }
      return Left(response.data['message'] ?? 'Error fetching invoices');
    } catch (e) {
      return Left('Error: ${e.toString()}');
    }
  }

  @override
  Future<Either<String, ProcessRemindersResponse>> processReminders() async {
    try {
      final response = await _apiClient.post('/reminders/process');
      if (response.statusCode == 200) {
        final data = response.data['data'];
        return Right(ProcessRemindersResponse.fromJson(data));
      }
      return Left(response.data['message'] ?? 'Error processing reminders');
    } catch (e) {
      return Left('Error: ${e.toString()}');
    }
  }

  @override
  Future<Either<String, Invoice>> create(Map<String, dynamic> invoiceData) async {
    try {
      final response = await _apiClient.post('/invoices', data: invoiceData);
      if (response.statusCode == 200 || response.statusCode == 201) {
        final data = response.data['data'];
        return Right(Invoice.fromJson(data));
      }
      return Left(response.data['message'] ?? 'Error creating invoice');
    } catch (e) {
      return Left('Error: ${e.toString()}');
    }
  }
}
