# Reminder Flow Mobile

Flutter mobile app for the Invoice Reminder Flow system.

## Running the App

```bash
flutter pub get
flutter run
```

## Running Tests

```bash
flutter test
```

## API Configuration

The app connects to the backend API at `http://10.0.2.2:8000/api/v1` (Android emulator localhost).

For physical devices, update the base URL in `lib/infrastructure/datasources/api_client.dart`.
