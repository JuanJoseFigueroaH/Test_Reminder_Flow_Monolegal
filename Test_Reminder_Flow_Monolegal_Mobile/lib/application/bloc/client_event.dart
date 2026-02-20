import 'package:equatable/equatable.dart';

abstract class ClientEvent extends Equatable {
  const ClientEvent();

  @override
  List<Object?> get props => [];
}

class LoadClients extends ClientEvent {}

class CreateClient extends ClientEvent {
  final Map<String, dynamic> clientData;

  const CreateClient(this.clientData);

  @override
  List<Object?> get props => [clientData];
}
