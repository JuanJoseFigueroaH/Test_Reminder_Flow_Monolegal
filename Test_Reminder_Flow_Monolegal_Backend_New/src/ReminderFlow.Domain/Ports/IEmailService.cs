namespace ReminderFlow.Domain.Ports;

public interface IEmailService
{
    Task<bool> SendReminderEmailAsync(string toEmail, string clientName, string invoiceNumber, decimal amount, string currentStatus, string newStatus);
}
