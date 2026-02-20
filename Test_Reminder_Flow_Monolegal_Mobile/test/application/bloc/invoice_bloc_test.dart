import 'package:bloc_test/bloc_test.dart';
import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_event.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_state.dart';
import 'package:reminder_flow_mobile/domain/entities/invoice.dart';
import 'package:reminder_flow_mobile/domain/entities/reminder_result.dart';
import 'package:reminder_flow_mobile/domain/ports/invoice_repository.dart';

class MockInvoiceRepository extends Mock implements InvoiceRepository {}

void main() {
  late InvoiceBloc bloc;
  late MockInvoiceRepository mockRepository;

  setUp(() {
    mockRepository = MockInvoiceRepository();
    bloc = InvoiceBloc(mockRepository);
  });

  tearDown(() {
    bloc.close();
  });

  group('InvoiceBloc', () {
    final testInvoices = [
      Invoice(
        id: '1',
        clientId: 'c1',
        invoiceNumber: 'INV-001',
        amount: 150000,
        status: InvoiceStatus.primerrecordatorio,
        dueDate: DateTime.now(),
        createdAt: DateTime.now(),
        updatedAt: DateTime.now(),
        clientName: 'Test Client',
        clientEmail: 'test@email.com',
      ),
    ];

    final testResponse = InvoiceListResponse(
      invoices: testInvoices,
      total: 1,
    );

    test('initial state is InvoiceInitial', () {
      expect(bloc.state, equals(InvoiceInitial()));
    });

    blocTest<InvoiceBloc, InvoiceState>(
      'emits [InvoiceLoading, InvoiceLoaded] when LoadInvoices is successful',
      build: () {
        when(() => mockRepository.getAll())
            .thenAnswer((_) async => Right(testResponse));
        return bloc;
      },
      act: (bloc) => bloc.add(LoadInvoices()),
      expect: () => [
        InvoiceLoading(),
        InvoiceLoaded(invoices: testInvoices, total: 1),
      ],
    );

    blocTest<InvoiceBloc, InvoiceState>(
      'emits [InvoiceLoading, InvoiceError] when LoadInvoices fails',
      build: () {
        when(() => mockRepository.getAll())
            .thenAnswer((_) async => const Left('Error loading invoices'));
        return bloc;
      },
      act: (bloc) => bloc.add(LoadInvoices()),
      expect: () => [
        InvoiceLoading(),
        const InvoiceError('Error loading invoices'),
      ],
    );

    blocTest<InvoiceBloc, InvoiceState>(
      'emits [RemindersProcessing, RemindersProcessed] when ProcessReminders is successful',
      build: () {
        final processResult = ProcessRemindersResponse(
          processedCount: 1,
          results: [
            const ReminderResult(
              invoiceId: '1',
              invoiceNumber: 'INV-001',
              clientName: 'Test',
              clientEmail: 'test@email.com',
              previousStatus: 'primerrecordatorio',
              newStatus: 'segundorecordatorio',
              emailSent: true,
            ),
          ],
        );
        when(() => mockRepository.processReminders())
            .thenAnswer((_) async => Right(processResult));
        return bloc;
      },
      act: (bloc) => bloc.add(ProcessReminders()),
      expect: () => [
        RemindersProcessing(),
        isA<RemindersProcessed>(),
      ],
    );

    blocTest<InvoiceBloc, InvoiceState>(
      'emits [SeedingData, DataSeeded] when SeedData is successful',
      build: () {
        when(() => mockRepository.seedData())
            .thenAnswer((_) async => const Right(null));
        return bloc;
      },
      act: (bloc) => bloc.add(SeedData()),
      expect: () => [
        SeedingData(),
        DataSeeded(),
      ],
    );
  });
}
