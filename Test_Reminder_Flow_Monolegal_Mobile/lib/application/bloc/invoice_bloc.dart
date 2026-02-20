import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_event.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_state.dart';
import 'package:reminder_flow_mobile/domain/ports/invoice_repository.dart';

class InvoiceBloc extends Bloc<InvoiceEvent, InvoiceState> {
  final InvoiceRepository _repository;

  InvoiceBloc(this._repository) : super(InvoiceInitial()) {
    on<LoadInvoices>(_onLoadInvoices);
    on<ProcessReminders>(_onProcessReminders);
    on<CreateInvoice>(_onCreateInvoice);
  }

  Future<void> _onLoadInvoices(
    LoadInvoices event,
    Emitter<InvoiceState> emit,
  ) async {
    emit(InvoiceLoading());
    final result = await _repository.getAll();
    result.fold(
      (error) => emit(InvoiceError(error)),
      (response) => emit(InvoiceLoaded(
        invoices: response.invoices,
        total: response.total,
      )),
    );
  }

  Future<void> _onProcessReminders(
    ProcessReminders event,
    Emitter<InvoiceState> emit,
  ) async {
    emit(RemindersProcessing());
    final result = await _repository.processReminders();
    result.fold(
      (error) => emit(InvoiceError(error)),
      (response) => emit(RemindersProcessed(response)),
    );
  }

  Future<void> _onCreateInvoice(
    CreateInvoice event,
    Emitter<InvoiceState> emit,
  ) async {
    final result = await _repository.create(event.invoiceData);
    result.fold(
      (error) => emit(InvoiceError(error)),
      (_) => add(LoadInvoices()),
    );
  }
}
