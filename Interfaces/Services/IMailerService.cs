using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Interfaces.Services
{
    public interface IMailerService
    {
        Task SendResetPasswordEmail(string emailTo, string fullname, string resetPasswordUrl, string locale);
    }
}