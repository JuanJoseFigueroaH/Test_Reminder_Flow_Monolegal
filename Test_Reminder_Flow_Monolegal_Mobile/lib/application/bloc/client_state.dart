import 'package:equatable/equatable.dart';
import 'package:reminder_flow_mobile/domain/entities/client.dart';

abstract class ClientState extends Equatable {
  const ClientState();

  @override
  List<Object?> get props => [];
}

class ClientInitial extends ClientState {}

class ClientLoading extends ClientState {}

class ClientLoaded extends ClientState {
  final List<Client> clients;
  final int total;

  const ClientLoaded({required this.clients, required this.total});

  @override
  List<Object?> get props => [clients, total];
}

class ClientError extends ClientState {
  final String message;

  const ClientError(this.message);

  @override
  List<Object?> get props => [message];
}
