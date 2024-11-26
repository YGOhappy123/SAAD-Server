using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using HandlebarsDotNet;
using milktea_server.Interfaces.Services;

namespace milktea_server.Services
{
    public class MailerService : IMailerService
    {
        private readonly IConfiguration _configuration;

        public MailerService(IConfiguration configuration)
        {
            _configuration = configuration;

            Handlebars.RegisterHelper(
                "eq",
                (writer, context, parameters) =>
                {
                    if (parameters.Length != 2)
                    {
                        throw new ArgumentException("The 'eq' helper expects exactly two arguments.");
                    }

                    var isEqual = parameters[0]?.ToString() == parameters[1]?.ToString();
                    writer.WriteSafeString(isEqual.ToString().ToLower());
                }
            );
        }

        private async Task SendEmail(string emailTo, string emailSubject, string emailBody)
        {
            var smtpClient = new SmtpClient()
            {
                Host = _configuration["Smtp:Host"]!,
                Port = int.Parse(_configuration["Smtp:Port"]!),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:Username"]!),
                Subject = emailSubject,
                Body = emailBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(emailTo);
            await smtpClient.SendMailAsync(mailMessage);
        }

        private static string GenerateTemplate(string templateName, object templateData)
        {
            var templatePath = Path.Combine("Templates", templateName);
            var templateContent = File.ReadAllText(templatePath);
            var template = Handlebars.Compile(templateContent);
            return template(templateData);
        }

        public async Task SendResetPasswordEmail(string emailTo, string fullname, string resetPasswordUrl, string locale)
        {
            string title = "Pit Milk Tea - Đặt lại mật khẩu";
            string body = GenerateTemplate(
                "ForgotPassword.hbs",
                new
                {
                    Fullname = fullname,
                    ResetPasswordUrl = resetPasswordUrl,
                    Locale = locale,
                }
            );

            await SendEmail(emailTo, title, body);
        }

        public async Task SendGoogleRegistrationSuccessEmail(
            string emailTo,
            string fullname,
            string username,
            string password,
            string changePasswordUrl,
            string locale
        )
        {
            string title = "Pit Milk Tea - Đăng ký thành công";
            string body = GenerateTemplate(
                "GoogleRegistrationSuccess.hbs",
                new
                {
                    Fullname = fullname,
                    Username = username,
                    Password = password,
                    ChangePasswordUrl = changePasswordUrl,
                    Locale = locale,
                }
            );

            await SendEmail(emailTo, title, body);
        }

        public async Task SendOrderConfirmationEmail(
            string emailTo,
            string fullname,
            string orderId,
            string manageOrdersUrl,
            string acceptedAt,
            string locale
        )
        {
            string title = "Pit Milk Tea - Xác nhận đơn hàng";
            string body = GenerateTemplate(
                "OrderConfirmation.hbs",
                new
                {
                    Fullname = fullname,
                    OrderId = orderId,
                    ManageOrdersUrl = manageOrdersUrl,
                    AcceptedAt = acceptedAt,
                    Locale = locale,
                }
            );

            await SendEmail(emailTo, title, body);
        }
    }
}
