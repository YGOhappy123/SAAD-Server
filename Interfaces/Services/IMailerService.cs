using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Interfaces.Services
{
    public interface IMailerService
    {
        Task SendResetPasswordEmail(string emailTo, string fullname, string resetPasswordUrl, string locale);
        Task SendGoogleRegistrationSuccessEmail(
            string emailTo,
            string fullname,
            string username,
            string password,
            string changePasswordUrl,
            string locale
        );

        Task SendOrderConfirmationEmail(
            string emailTo,
            string fullname,
            string orderId,
            string manageOrdersUrl,
            string acceptedAt,
            string locale
        );
    }
}
