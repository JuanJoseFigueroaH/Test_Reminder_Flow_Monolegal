import 'package:flutter_test/flutter_test.dart';
import 'package:reminder_flow_mobile/domain/entities/client.dart';

void main() {
  group('Client', () {
    test('should create Client from JSON', () {
      final json = {
        'id': '123',
        'name': 'Test Client',
        'email': 'test@email.com',
        'phone': '+57 300 123 4567',
      };

      final client = Client.fromJson(json);

      expect(client.id, '123');
      expect(client.name, 'Test Client');
      expect(client.email, 'test@email.com');
      expect(client.phone, '+57 300 123 4567');
    });

    test('should create Client from JSON without phone', () {
      final json = {
        'id': '123',
        'name': 'Test Client',
        'email': 'test@email.com',
      };

      final client = Client.fromJson(json);

      expect(client.id, '123');
      expect(client.name, 'Test Client');
      expect(client.email, 'test@email.com');
      expect(client.phone, isNull);
    });

    test('should convert Client to JSON', () {
      const client = Client(
        id: '123',
        name: 'Test Client',
        email: 'test@email.com',
        phone: '+57 300 123 4567',
      );

      final json = client.toJson();

      expect(json['id'], '123');
      expect(json['name'], 'Test Client');
      expect(json['email'], 'test@email.com');
      expect(json['phone'], '+57 300 123 4567');
    });

    test('should have correct equality', () {
      const client1 = Client(
        id: '123',
        name: 'Test Client',
        email: 'test@email.com',
      );

      const client2 = Client(
        id: '123',
        name: 'Test Client',
        email: 'test@email.com',
      );

      expect(client1, equals(client2));
    });
  });

  group('ClientListResponse', () {
    test('should create ClientListResponse from JSON', () {
      final json = {
        'clients': [
          {
            'id': '1',
            'name': 'Client 1',
            'email': 'client1@email.com',
          },
          {
            'id': '2',
            'name': 'Client 2',
            'email': 'client2@email.com',
          },
        ],
        'total': 2,
      };

      final response = ClientListResponse.fromJson(json);

      expect(response.clients.length, 2);
      expect(response.total, 2);
      expect(response.clients[0].name, 'Client 1');
      expect(response.clients[1].name, 'Client 2');
    });
  });
}
