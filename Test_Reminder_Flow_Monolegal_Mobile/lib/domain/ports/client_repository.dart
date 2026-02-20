import 'package:dartz/dartz.dart';
import 'package:reminder_flow_mobile/domain/entities/client.dart';

abstract class ClientRepository {
  Future<Either<String, ClientListResponse>> getAll();
  Future<Either<String, Client>> getById(String id);
  Future<Either<String, Client>> create(Map<String, dynamic> clientData);
}
