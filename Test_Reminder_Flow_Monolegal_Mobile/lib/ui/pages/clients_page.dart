import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/client_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/client_event.dart';
import 'package:reminder_flow_mobile/application/bloc/client_state.dart';
import 'package:reminder_flow_mobile/ui/atoms/loading_indicator.dart';
import 'package:reminder_flow_mobile/ui/molecules/client_card.dart';
import 'package:reminder_flow_mobile/ui/theme/app_theme.dart';

class ClientsPage extends StatefulWidget {
  const ClientsPage({super.key});

  @override
  State<ClientsPage> createState() => _ClientsPageState();
}

class _ClientsPageState extends State<ClientsPage> {
  @override
  void initState() {
    super.initState();
    context.read<ClientBloc>().add(LoadClients());
  }

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<ClientBloc, ClientState>(
      builder: (context, state) {
        if (state is ClientLoading) {
          return const LoadingIndicator(message: 'Cargando clientes...');
        }

        if (state is ClientLoaded) {
          if (state.clients.isEmpty) {
            return const Center(
              child: Text(
                'No hay clientes disponibles',
                style: TextStyle(color: AppTheme.textSecondary),
              ),
            );
          }

          return RefreshIndicator(
            onRefresh: () async {
              context.read<ClientBloc>().add(LoadClients());
            },
            child: ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: state.clients.length,
              itemBuilder: (context, index) {
                return ClientCard(client: state.clients[index]);
              },
            ),
          );
        }

        if (state is ClientError) {
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
                    context.read<ClientBloc>().add(LoadClients());
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
