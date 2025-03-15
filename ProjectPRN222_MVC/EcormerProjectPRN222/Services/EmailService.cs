using MailKit.Net.Smtp;
using MimeKit;
namespace EcormerProjectPRN222.Services
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _senderEmail;
        private readonly string _senderPassword;

        // Inject IConfiguration để lấy thông tin từ appsettings.json
        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["EmailSettings:SmtpServer"];
            _port = int.Parse(configuration["EmailSettings:Port"]);
            _senderEmail = configuration["EmailSettings:SenderEmail"];
            _senderPassword = configuration["EmailSettings:SenderPassword"];
        }

        // Phương thức gửi email
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Admin", _senderEmail)); // Địa chỉ người gửi
            message.To.Add(new MailboxAddress("", toEmail)); // Địa chỉ người nhận
            message.Subject = subject;

            // Nội dung email
            message.Body = new TextPart("html") { Text = body };

            // Kết nối tới server SMTP và gửi email
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_senderEmail, _senderPassword); // Xác thực người gửi
                await client.SendAsync(message); // Gửi email
                await client.DisconnectAsync(true); // Ngắt kết nối
            }
        }
    }
}
