using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagementSystem.Service
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAbsentEmail(string toEmail, string employeeName, DateTime absentDate)
        {
            // 🔍 Read config safely
            var host = _config["Email:SmtpHost"];
            var portStr = _config["Email:SmtpPort"];
            var username = _config["Email:Username"];
            var password = _config["Email:Password"];
            var from = _config["Email:From"];

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(portStr) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(from))
            {
                throw new Exception("❌ Email configuration is missing in appsettings.json");
            }

            using var smtp = new SmtpClient(host)
            {
                Port = int.Parse(portStr),
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(from),
                Subject = $"Absent Notification – {absentDate:dd MMM yyyy}",
                Body = $@"
Hello {employeeName},

Our records show that you were marked ABSENT on {absentDate:dd-MMM-yyyy}.

If you believe this is incorrect, please contact HR as soon as possible.

Regards,
Employee Management System
",
                IsBodyHtml = false
            };

            mail.To.Add(toEmail);

           
            await smtp.SendMailAsync(mail);
        }

        // Send email when leave is approved or rejected
        public async Task SendLeaveStatusEmail(
            string toEmail,
            string employeeName,
            DateOnly leaveDate,
            bool isApproved)
        {
            var host = _config["Email:SmtpHost"];
            var portStr = _config["Email:SmtpPort"];
            var username = _config["Email:Username"];
            var password = _config["Email:Password"];
            var from = _config["Email:From"];

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(portStr) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(from))
            {
                throw new Exception("Email configuration is missing.");
            }

            using var smtp = new SmtpClient(host)
            {
                Port = int.Parse(portStr),
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)
            };

            var status = isApproved ? "approved" : "rejected";

            using var mail = new MailMessage
            {
                From = new MailAddress(from),
                Subject = $"Leave Request {status.ToUpper()}",
                Body = $@"Hello {employeeName},

Your leave request for {leaveDate:dd-MMM-yyyy} has been {status}.

Regards,
Employee Management System",
                IsBodyHtml = false
            };

            mail.To.Add(toEmail);
            await smtp.SendMailAsync(mail);
        }
    }
}
