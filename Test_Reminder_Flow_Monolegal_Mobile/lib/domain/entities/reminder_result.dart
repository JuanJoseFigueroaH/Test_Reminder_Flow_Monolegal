import 'package:equatable/equatable.dart';

class ReminderResult extends Equatable {
  final String invoiceId;
  final String invoiceNumber;
  final String clientName;
  final String clientEmail;
  final String previousStatus;
  final String newStatus;
  final bool emailSent;

  const ReminderResult({
    required this.invoiceId,
    required this.invoiceNumber,
    required this.clientName,
    required this.clientEmail,
    required this.previousStatus,
    required this.newStatus,
    required this.emailSent,
  });

  factory ReminderResult.fromJson(Map<String, dynamic> json) {
    return ReminderResult(
      invoiceId: json['invoice_id'] ?? '',
      invoiceNumber: json['invoice_number'] ?? '',
      clientName: json['client_name'] ?? '',
      clientEmail: json['client_email'] ?? '',
      previousStatus: json['previous_status'] ?? '',
      newStatus: json['new_status'] ?? '',
      emailSent: json['email_sent'] ?? false,
    );
  }

  @override
  List<Object?> get props => [
        invoiceId,
        invoiceNumber,
        clientName,
        clientEmail,
        previousStatus,
        newStatus,
        emailSent,
      ];
}

class ProcessRemindersResponse {
  final int processedCount;
  final List<ReminderResult> results;

  ProcessRemindersResponse({
    required this.processedCount,
    required this.results,
  });

  factory ProcessRemindersResponse.fromJson(Map<String, dynamic> json) {
    return ProcessRemindersResponse(
      processedCount: json['processed_count'] ?? 0,
      results: (json['results'] as List)
          .map((e) => ReminderResult.fromJson(e))
          .toList(),
    );
  }
}
