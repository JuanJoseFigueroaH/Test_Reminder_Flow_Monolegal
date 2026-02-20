import 'package:flutter/material.dart';
import 'package:reminder_flow_mobile/domain/entities/invoice.dart';

class StatusBadge extends StatelessWidget {
  final InvoiceStatus status;

  const StatusBadge({super.key, required this.status});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
      decoration: BoxDecoration(
        color: _getBackgroundColor(),
        borderRadius: BorderRadius.circular(20),
      ),
      child: Text(
        status.displayName,
        style: TextStyle(
          fontSize: 12,
          fontWeight: FontWeight.w500,
          color: _getTextColor(),
        ),
      ),
    );
  }

  Color _getBackgroundColor() {
    switch (status) {
      case InvoiceStatus.pendiente:
        return const Color(0xFFF0FDF4);
      case InvoiceStatus.primerrecordatorio:
        return const Color(0xFFDBEAFE);
      case InvoiceStatus.segundorecordatorio:
        return const Color(0xFFFEF3C7);
      case InvoiceStatus.desactivado:
        return const Color(0xFFFEE2E2);
    }
  }

  Color _getTextColor() {
    switch (status) {
      case InvoiceStatus.pendiente:
        return const Color(0xFF166534);
      case InvoiceStatus.primerrecordatorio:
        return const Color(0xFF1E40AF);
      case InvoiceStatus.segundorecordatorio:
        return const Color(0xFF92400E);
      case InvoiceStatus.desactivado:
        return const Color(0xFF991B1B);
    }
  }
}
