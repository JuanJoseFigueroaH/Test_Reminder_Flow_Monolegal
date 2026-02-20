import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:reminder_flow_mobile/infrastructure/di/injection.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/client_bloc.dart';
import 'package:reminder_flow_mobile/ui/pages/home_page.dart';
import 'package:reminder_flow_mobile/ui/theme/app_theme.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  configureDependencies();
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        BlocProvider(create: (_) => getIt<InvoiceBloc>()),
        BlocProvider(create: (_) => getIt<ClientBloc>()),
      ],
      child: MaterialApp(
        title: 'Reminder Flow',
        debugShowCheckedModeBanner: false,
        theme: AppTheme.lightTheme,
        home: const HomePage(),
      ),
    );
  }
}
