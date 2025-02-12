using System.Net;
using System.Net.Mail;

namespace Authentication__ASP.Net_Core__.Services
{
    public class EmailVerificationService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailVerificationService(IConfiguration configuration , IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task SendEmailAsync(string toEmail, string FirstName, string verificationToken)
        {

            // Load email template from file
            var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "Templates", "EmailTemplate.html");
            var emailBody = await File.ReadAllTextAsync(templatePath);


            // Replace placeholders with dynamic data
            emailBody = emailBody.Replace("{{FirstName}}", FirstName);


            // Generate the verification URL
            var verificationUrl = $"https://localhost:7005/api/auth/verify-email?token={verificationToken}";
            emailBody = emailBody.Replace("{{VerificationUrl}}", verificationUrl);

            // Replace the user email placeholder
            emailBody = emailBody.Replace("{{UserEmail}}", toEmail);

            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = "EMAIL VERIFICATION.",
                Body = emailBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
