class ApiResponse<T> {
  final int statusCode;
  final String message;
  final T? data;

  ApiResponse({
    required this.statusCode,
    required this.message,
    this.data,
  });

  factory ApiResponse.fromJson(
    Map<String, dynamic> json,
    T Function(Map<String, dynamic>)? fromJsonT,
  ) {
    return ApiResponse(
      statusCode: json['status_code'] ?? 0,
      message: json['message'] ?? '',
      data: json['data'] != null && fromJsonT != null
          ? fromJsonT(json['data'])
          : null,
    );
  }

  bool get isSuccess => statusCode >= 200 && statusCode < 300;
}
