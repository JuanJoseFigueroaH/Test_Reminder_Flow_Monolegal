import 'package:bloc_test/bloc_test.dart';
import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:reminder_flow_mobile/application/bloc/client_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/client_event.dart';
import 'package:reminder_flow_mobile/application/bloc/client_state.dart';
import 'package:reminder_flow_mobile/domain/entities/client.dart';
import 'package:reminder_flow_mobile/domain/ports/client_repository.dart';

class MockClientRepository extends Mock implements ClientRepository {}

void main() {
  late ClientBloc bloc;
  late MockClientRepository mockRepository;

  setUp(() {
    mockRepository = MockClientRepository();
    bloc = ClientBloc(mockRepository);
  });

  tearDown(() {
    bloc.close();
  });

  group('ClientBloc', () {
    final testClients = [
      const Client(
        id: '1',
        name: 'Test Client',
        email: 'test@email.com',
        phone: '+57 300 123 4567',
      ),
    ];

    final testResponse = ClientListResponse(
      clients: testClients,
      total: 1,
    );

    test('initial state is ClientInitial', () {
      expect(bloc.state, equals(ClientInitial()));
    });

    blocTest<ClientBloc, ClientState>(
      'emits [ClientLoading, ClientLoaded] when LoadClients is successful',
      build: () {
        when(() => mockRepository.getAll())
            .thenAnswer((_) async => Right(testResponse));
        return bloc;
      },
      act: (bloc) => bloc.add(LoadClients()),
      expect: () => [
        ClientLoading(),
        ClientLoaded(clients: testClients, total: 1),
      ],
    );

    blocTest<ClientBloc, ClientState>(
      'emits [ClientLoading, ClientError] when LoadClients fails',
      build: () {
        when(() => mockRepository.getAll())
            .thenAnswer((_) async => const Left('Error loading clients'));
        return bloc;
      },
      act: (bloc) => bloc.add(LoadClients()),
      expect: () => [
        ClientLoading(),
        const ClientError('Error loading clients'),
      ],
    );
  });
}
