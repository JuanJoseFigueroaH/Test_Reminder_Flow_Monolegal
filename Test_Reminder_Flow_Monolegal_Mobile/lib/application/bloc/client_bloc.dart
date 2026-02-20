import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/client_event.dart';
import 'package:reminder_flow_mobile/application/bloc/client_state.dart';
import 'package:reminder_flow_mobile/domain/ports/client_repository.dart';

class ClientBloc extends Bloc<ClientEvent, ClientState> {
  final ClientRepository _repository;

  ClientBloc(this._repository) : super(ClientInitial()) {
    on<LoadClients>(_onLoadClients);
    on<CreateClient>(_onCreateClient);
  }

  Future<void> _onLoadClients(
    LoadClients event,
    Emitter<ClientState> emit,
  ) async {
    emit(ClientLoading());
    final result = await _repository.getAll();
    result.fold(
      (error) => emit(ClientError(error)),
      (response) => emit(ClientLoaded(
        clients: response.clients,
        total: response.total,
      )),
    );
  }

  Future<void> _onCreateClient(
    CreateClient event,
    Emitter<ClientState> emit,
  ) async {
    final result = await _repository.create(event.clientData);
    result.fold(
      (error) => emit(ClientError(error)),
      (_) => add(LoadClients()),
    );
  }
}
