using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using ReminderFlow.Domain.Ports;
using ReminderFlow.Infrastructure.Settings;

namespace ReminderFlow.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> SendReminderEmailAsync(
        string toEmail,
        string clientName,
        string invoiceNumber,
        decimal amount,
        string currentStatus,
        string newStatus)
    {
        try
        {
            _logger.LogInformation("Enviando email de recordatorio a {Email}", toEmail);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(clientName, toEmail));
            message.Subject = $"Recordatorio de Factura {invoiceNumber} - Cambio de Estado";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildEmailTemplate(clientName, invoiceNumber, amount, currentStatus, newStatus)
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email enviado exitosamente a {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar email a {Email}", toEmail);
            return false;
        }
    }

    private static string BuildEmailTemplate(
        string clientName,
        string invoiceNumber,
        decimal amount,
        string currentStatus,
        string newStatus)
    {
        var statusColor = newStatus switch
        {
            "primerrecordatorio" => "#3B82F6",
            "segundorecordatorio" => "#F59E0B",
            "desactivado" => "#EF4444",
            _ => "#10B981"
        };

        var statusMessage = newStatus switch
        {
            "primerrecordatorio" => "Su factura ha pasado a Primer Recordatorio",
            "segundorecordatorio" => "Su factura ha pasado a Segundo Recordatorio",
            "desactivado" => "Su factura ha sido Desactivada por falta de pago",
            _ => "Estado actualizado"
        };

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: {statusColor}; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .footer {{ background-color: #333; color: white; padding: 15px; text-align: center; border-radius: 0 0 8px 8px; }}
        .amount {{ font-size: 24px; font-weight: bold; color: {statusColor}; }}
        .status-badge {{ display: inline-block; padding: 8px 16px; background-color: {statusColor}; color: white; border-radius: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Recordatorio de Factura</h1>
        </div>
        <div class='content'>
            <p>Estimado/a <strong>{clientName}</strong>,</p>
            <p>{statusMessage}</p>
            <p><strong>Detalles de la factura:</strong></p>
            <ul>
                <li>Número de Factura: <strong>{invoiceNumber}</strong></li>
                <li>Monto: <span class='amount'>${amount:N2}</span></li>
                <li>Estado anterior: {currentStatus}</li>
                <li>Nuevo estado: <span class='status-badge'>{newStatus}</span></li>
            </ul>
            <p>Por favor, realice el pago a la brevedad posible para evitar inconvenientes.</p>
        </div>
        <div class='footer'>
            <p>Este es un mensaje automático del Sistema de Recordatorios</p>
        </div>
    </div>
</body>
</html>";
    }
}
