import 'dart:async';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/client_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/client_event.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_event.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_state.dart';
import 'package:reminder_flow_mobile/ui/atoms/stat_card.dart';
import 'package:reminder_flow_mobile/ui/atoms/loading_indicator.dart';
import 'package:reminder_flow_mobile/ui/molecules/invoice_card.dart';
import 'package:reminder_flow_mobile/ui/pages/invoices_page.dart';
import 'package:reminder_flow_mobile/ui/pages/clients_page.dart';
import 'package:reminder_flow_mobile/ui/pages/create_client_page.dart';
import 'package:reminder_flow_mobile/ui/pages/create_invoice_page.dart';
import 'package:reminder_flow_mobile/ui/theme/app_theme.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  int _currentIndex = 0;
  Timer? _refreshTimer;

  @override
  void initState() {
    super.initState();
    context.read<InvoiceBloc>().add(LoadInvoices());
    context.read<ClientBloc>().add(LoadClients());
    _startAutoRefresh();
  }

  @override
  void dispose() {
    _refreshTimer?.cancel();
    super.dispose();
  }

  void _startAutoRefresh() {
    _refreshTimer = Timer.periodic(const Duration(seconds: 30), (_) {
      context.read<InvoiceBloc>().add(LoadInvoices());
      context.read<ClientBloc>().add(LoadClients());
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Reminder Flow'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () {
              context.read<InvoiceBloc>().add(LoadInvoices());
            },
          ),
        ],
      ),
      body: IndexedStack(
        index: _currentIndex,
        children: const [
          DashboardTab(),
          InvoicesPage(),
          ClientsPage(),
        ],
      ),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _currentIndex,
        onDestinationSelected: (index) {
          setState(() {
            _currentIndex = index;
          });
        },
        destinations: const [
          NavigationDestination(
            icon: Icon(Icons.dashboard_outlined),
            selectedIcon: Icon(Icons.dashboard),
            label: 'Dashboard',
          ),
          NavigationDestination(
            icon: Icon(Icons.receipt_long_outlined),
            selectedIcon: Icon(Icons.receipt_long),
            label: 'Facturas',
          ),
          NavigationDestination(
            icon: Icon(Icons.people_outline),
            selectedIcon: Icon(Icons.people),
            label: 'Clientes',
          ),
        ],
      ),
    );
  }
}

class DashboardTab extends StatelessWidget {
  const DashboardTab({super.key});

  @override
  Widget build(BuildContext context) {
    return BlocConsumer<InvoiceBloc, InvoiceState>(
      listener: (context, state) {
        if (state is InvoiceError) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(state.message),
              backgroundColor: AppTheme.dangerColor,
            ),
          );
        }
        if (state is RemindersProcessed) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Se procesaron ${state.result.processedCount} recordatorios'),
              backgroundColor: AppTheme.successColor,
            ),
          );
          context.read<InvoiceBloc>().add(LoadInvoices());
        }
      },
      builder: (context, state) {
        if (state is InvoiceLoading) {
          return const LoadingIndicator(message: 'Cargando facturas...');
        }

        if (state is RemindersProcessing) {
          return const LoadingIndicator(message: 'Procesando recordatorios...');
        }

        if (state is InvoiceLoaded) {
          return RefreshIndicator(
            onRefresh: () async {
              context.read<InvoiceBloc>().add(LoadInvoices());
              context.read<ClientBloc>().add(LoadClients());
            },
            child: SingleChildScrollView(
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: ElevatedButton.icon(
                          onPressed: () async {
                            final result = await Navigator.push(
                              context,
                              MaterialPageRoute(builder: (_) => const CreateClientPage()),
                            );
                            if (result == true) {
                              context.read<ClientBloc>().add(LoadClients());
                            }
                          },
                          icon: const Icon(Icons.person_add),
                          label: const Text('+ Cliente'),
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppTheme.primaryColor,
                          ),
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: ElevatedButton.icon(
                          onPressed: () async {
                            context.read<ClientBloc>().add(LoadClients());
                            final result = await Navigator.push(
                              context,
                              MaterialPageRoute(builder: (_) => const CreateInvoicePage()),
                            );
                            if (result == true) {
                              context.read<InvoiceBloc>().add(LoadInvoices());
                            }
                          },
                          icon: const Icon(Icons.receipt_long),
                          label: const Text('+ Factura'),
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppTheme.successColor,
                          ),
                        ),
                      ),
                    ],
                  ),
                  Container(
                    margin: const EdgeInsets.only(top: 16),
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      gradient: const LinearGradient(
                        colors: [Color(0xFFDBEAFE), Color(0xFFE0F2FE)],
                      ),
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: const Row(
                      children: [
                        Text('⏰', style: TextStyle(fontSize: 16)),
                        SizedBox(width: 8),
                        Expanded(
                          child: Text(
                            'Los recordatorios se procesan automáticamente cada 2 minutos',
                            style: TextStyle(
                              color: Color(0xFF1E40AF),
                              fontSize: 13,
                              fontWeight: FontWeight.w500,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 24),
                  GridView.count(
                    crossAxisCount: 2,
                    shrinkWrap: true,
                    physics: const NeverScrollableScrollPhysics(),
                    mainAxisSpacing: 12,
                    crossAxisSpacing: 12,
                    childAspectRatio: 1.5,
                    children: [
                      StatCard(
                        icon: '📄',
                        label: 'Total Facturas',
                        value: state.total.toString(),
                        iconBackground: const Color(0xFFE0F2FE),
                      ),
                      StatCard(
                        icon: '🕐',
                        label: 'Pendientes',
                        value: state.pendienteCount.toString(),
                        iconBackground: const Color(0xFFF0FDF4),
                      ),
                      StatCard(
                        icon: '📋',
                        label: 'Primer Recordatorio',
                        value: state.primerRecordatorioCount.toString(),
                        iconBackground: const Color(0xFFDBEAFE),
                      ),
                      StatCard(
                        icon: '⚠️',
                        label: 'Segundo Recordatorio',
                        value: state.segundoRecordatorioCount.toString(),
                        iconBackground: const Color(0xFFFEF3C7),
                      ),
                      StatCard(
                        icon: '⛔',
                        label: 'Desactivados',
                        value: state.desactivadoCount.toString(),
                        iconBackground: const Color(0xFFFEE2E2),
                      ),
                    ],
                  ),
                  const SizedBox(height: 24),
                  const Text(
                    'Últimas Facturas',
                    style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.w600,
                      color: AppTheme.textPrimary,
                    ),
                  ),
                  const SizedBox(height: 12),
                  if (state.invoices.isEmpty)
                    const Center(
                      child: Padding(
                        padding: EdgeInsets.all(32),
                        child: Text(
                          'No hay facturas disponibles.\nCrea un cliente y una factura para comenzar.',
                          textAlign: TextAlign.center,
                          style: TextStyle(color: AppTheme.textSecondary),
                        ),
                      ),
                    )
                  else
                    ...state.invoices.take(5).map((invoice) => InvoiceCard(invoice: invoice)),
                ],
              ),
            ),
          );
        }

        return Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Text('Presiona el botón para cargar datos'),
              const SizedBox(height: 16),
              ElevatedButton(
                onPressed: () {
                  context.read<InvoiceBloc>().add(LoadInvoices());
                },
                child: const Text('Cargar'),
              ),
            ],
          ),
        );
      },
    );
  }
}
