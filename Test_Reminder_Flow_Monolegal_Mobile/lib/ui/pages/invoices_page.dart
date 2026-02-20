import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_event.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_state.dart';
import 'package:reminder_flow_mobile/ui/atoms/loading_indicator.dart';
import 'package:reminder_flow_mobile/ui/molecules/invoice_card.dart';
import 'package:reminder_flow_mobile/ui/theme/app_theme.dart';

class InvoicesPage extends StatefulWidget {
  const InvoicesPage({super.key});

  @override
  State<InvoicesPage> createState() => _InvoicesPageState();
}

class _InvoicesPageState extends State<InvoicesPage> {
  @override
  void initState() {
    super.initState();
    final state = context.read<InvoiceBloc>().state;
    if (state is! InvoiceLoaded) {
      context.read<InvoiceBloc>().add(LoadInvoices());
    }
  }

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<InvoiceBloc, InvoiceState>(
      builder: (context, state) {
        if (state is InvoiceLoading) {
          return const LoadingIndicator(message: 'Cargando facturas...');
        }

        if (state is InvoiceLoaded) {
          if (state.invoices.isEmpty) {
            return const Center(
              child: Text(
                'No hay facturas disponibles',
                style: TextStyle(color: AppTheme.textSecondary),
              ),
            );
          }

          return RefreshIndicator(
            onRefresh: () async {
              context.read<InvoiceBloc>().add(LoadInvoices());
            },
            child: ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: state.invoices.length,
              itemBuilder: (context, index) {
                return InvoiceCard(invoice: state.invoices[index]);
              },
            ),
          );
        }

        if (state is InvoiceError) {
          return Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Icon(Icons.error_outline, size: 48, color: AppTheme.dangerColor),
                const SizedBox(height: 16),
                Text(
                  state.message,
                  textAlign: TextAlign.center,
                  style: const TextStyle(color: AppTheme.textSecondary),
                ),
                const SizedBox(height: 16),
                ElevatedButton(
                  onPressed: () {
                    context.read<InvoiceBloc>().add(LoadInvoices());
                  },
                  child: const Text('Reintentar'),
                ),
              ],
            ),
          );
        }

        return const SizedBox.shrink();
      },
    );
  }
}
