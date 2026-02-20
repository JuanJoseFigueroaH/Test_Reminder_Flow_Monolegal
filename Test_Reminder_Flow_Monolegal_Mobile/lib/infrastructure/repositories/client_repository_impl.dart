import 'package:dartz/dartz.dart';
import 'package:injectable/injectable.dart';
import 'package:reminder_flow_mobile/domain/entities/client.dart';
import 'package:reminder_flow_mobile/domain/ports/client_repository.dart';
import 'package:reminder_flow_mobile/infrastructure/datasources/api_client.dart';

@LazySingleton(as: ClientRepository)
class ClientRepositoryImpl implements ClientRepository {
  final ApiClient _apiClient;

  ClientRepositoryImpl(this._apiClient);

  @override
  Future<Either<String, ClientListResponse>> getAll() async {
    try {
      final response = await _apiClient.get('/clients');
      if (response.statusCode == 200) {
        final data = response.data['data'];
        return Right(ClientListResponse.fromJson(data));
      }
      return Left(response.data['message'] ?? 'Error fetching clients');
    } catch (e) {
      return Left('Error: ${e.toString()}');
    }
  }

  @override
  Future<Either<String, Client>> getById(String id) async {
    try {
      final response = await _apiClient.get('/clients/$id');
      if (response.statusCode == 200) {
        final data = response.data['data'];
        return Right(Client.fromJson(data));
      }
      return Left(response.data['message'] ?? 'Error fetching client');
    } catch (e) {
      return Left('Error: ${e.toString()}');
    }
  }

  @override
  Future<Either<String, Client>> create(Map<String, dynamic> clientData) async {
    try {
      final response = await _apiClient.post('/clients', data: clientData);
      if (response.statusCode == 200 || response.statusCode == 201) {
        final data = response.data['data'];
        return Right(Client.fromJson(data));
      }
      return Left(response.data['message'] ?? 'Error creating client');
    } catch (e) {
      return Left('Error: ${e.toString()}');
    }
  }
}
