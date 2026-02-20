import 'package:equatable/equatable.dart';

class Client extends Equatable {
  final String id;
  final String name;
  final String email;
  final String? phone;

  const Client({
    required this.id,
    required this.name,
    required this.email,
    this.phone,
  });

  factory Client.fromJson(Map<String, dynamic> json) {
    return Client(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      email: json['email'] ?? '',
      phone: json['phone'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'email': email,
      'phone': phone,
    };
  }

  @override
  List<Object?> get props => [id, name, email, phone];
}

class ClientListResponse {
  final List<Client> clients;
  final int total;

  ClientListResponse({required this.clients, required this.total});

  factory ClientListResponse.fromJson(Map<String, dynamic> json) {
    return ClientListResponse(
      clients: (json['clients'] as List)
          .map((e) => Client.fromJson(e))
          .toList(),
      total: json['total'] ?? 0,
    );
  }
}
