import 'package:get_it/get_it.dart';
import 'package:reminder_flow_mobile/infrastructure/datasources/api_client.dart';
import 'package:reminder_flow_mobile/infrastructure/repositories/invoice_repository_impl.dart';
import 'package:reminder_flow_mobile/infrastructure/repositories/client_repository_impl.dart';
import 'package:reminder_flow_mobile/domain/ports/invoice_repository.dart';
import 'package:reminder_flow_mobile/domain/ports/client_repository.dart';
import 'package:reminder_flow_mobile/application/bloc/invoice_bloc.dart';
import 'package:reminder_flow_mobile/application/bloc/client_bloc.dart';

final getIt = GetIt.instance;

void configureDependencies() {
  getIt.registerLazySingleton<ApiClient>(() => ApiClient());

  getIt.registerLazySingleton<InvoiceRepository>(
    () => InvoiceRepositoryImpl(getIt<ApiClient>()),
  );

  getIt.registerLazySingleton<ClientRepository>(
    () => ClientRepositoryImpl(getIt<ApiClient>()),
  );

  getIt.registerFactory<InvoiceBloc>(
    () => InvoiceBloc(getIt<InvoiceRepository>()),
  );

  getIt.registerFactory<ClientBloc>(
    () => ClientBloc(getIt<ClientRepository>()),
  );
}
