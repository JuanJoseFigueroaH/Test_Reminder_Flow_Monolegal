import 'package:equatable/equatable.dart';
import 'package:reminder_flow_mobile/domain/entities/invoice.dart';
import 'package:reminder_flow_mobile/domain/entities/reminder_result.dart';

abstract class InvoiceState extends Equatable {
  const InvoiceState();

  @override
  List<Object?> get props => [];
}

class InvoiceInitial extends InvoiceState {}

class InvoiceLoading extends InvoiceState {}

class InvoiceLoaded extends InvoiceState {
  final List<Invoice> invoices;
  final int total;

  const InvoiceLoaded({required this.invoices, required this.total});

  int get pendienteCount =>
      invoices.where((i) => i.status == InvoiceStatus.pendiente).length;

  int get primerRecordatorioCount =>
      invoices.where((i) => i.status == InvoiceStatus.primerrecordatorio).length;

  int get segundoRecordatorioCount =>
      invoices.where((i) => i.status == InvoiceStatus.segundorecordatorio).length;

  int get desactivadoCount =>
      invoices.where((i) => i.status == InvoiceStatus.desactivado).length;

  @override
  List<Object?> get props => [invoices, total];
}

class InvoiceError extends InvoiceState {
  final String message;

  const InvoiceError(this.message);

  @override
  List<Object?> get props => [message];
}

class RemindersProcessing extends InvoiceState {}

class RemindersProcessed extends InvoiceState {
  final ProcessRemindersResponse result;

  const RemindersProcessed(this.result);

  @override
  List<Object?> get props => [result];
}

