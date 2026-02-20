# Reminder Flow Backend

Backend API for managing invoice reminders and client notifications.

## Running with Docker

```bash
docker-compose up --build
```

## API Endpoints

### Health
- `GET /health` - Health check

### Clients
- `GET /api/v1/clients` - Get all clients
- `GET /api/v1/clients/{id}` - Get client by ID
- `POST /api/v1/clients` - Create client
- `PUT /api/v1/clients/{id}` - Update client
- `DELETE /api/v1/clients/{id}` - Delete client

### Invoices
- `GET /api/v1/invoices` - Get all invoices
- `GET /api/v1/invoices/{id}` - Get invoice by ID
- `GET /api/v1/invoices/client/{client_id}` - Get invoices by client
- `GET /api/v1/invoices/status/{statuses}` - Get invoices by status
- `POST /api/v1/invoices` - Create invoice
- `DELETE /api/v1/invoices/{id}` - Delete invoice

### Reminders
- `POST /api/v1/reminders/process` - Process all pending reminders

## Running Tests

```bash
pytest
```

## Swagger Documentation

Access Swagger UI at: `http://localhost:8000/docs`
