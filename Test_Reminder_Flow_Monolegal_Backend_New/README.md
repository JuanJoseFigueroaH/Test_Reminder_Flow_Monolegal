# Reminder Flow Backend (.NET Core)

Backend API para gestión de recordatorios de facturas desarrollado en C# y .NET Core.

## Ejecución

### Con Docker
```bash
docker-compose up --build
```
API disponible en: http://localhost:8000
Swagger: http://localhost:8000/docs

### Local
```bash
dotnet run --project src/ReminderFlow.Api
```

## Tests
```bash
dotnet test
```

## API Endpoints

### Health
- `GET /health` - Health check

### Clients
- `GET /api/v1/clients` - Obtener todos los clientes
- `GET /api/v1/clients/{id}` - Obtener cliente por ID
- `POST /api/v1/clients` - Crear cliente
- `PUT /api/v1/clients/{id}` - Actualizar cliente
- `DELETE /api/v1/clients/{id}` - Eliminar cliente

### Invoices
- `GET /api/v1/invoices` - Obtener todas las facturas
- `GET /api/v1/invoices/{id}` - Obtener factura por ID
- `GET /api/v1/invoices/client/{clientId}` - Obtener facturas por cliente
- `GET /api/v1/invoices/status/{statuses}` - Obtener facturas por estado
- `POST /api/v1/invoices` - Crear factura
- `DELETE /api/v1/invoices/{id}` - Eliminar factura

### Reminders
- `POST /api/v1/reminders/process` - Procesar todos los recordatorios pendientes
