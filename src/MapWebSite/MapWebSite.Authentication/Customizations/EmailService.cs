using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Authentication
{
    internal class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            var client = new SendGrid.SendGridClient(ConfigurationManager.AppSettings["ApiKey"]);

            var emailMessage = new SendGridMessage();
            emailMessage.From = new EmailAddress(ConfigurationManager.AppSettings["EmailAddress"],
                                                ConfigurationManager.AppSettings["EmailDisplayName"]);
            emailMessage.AddTo(message.Destination);
            emailMessage.Subject = message.Subject;
            emailMessage.HtmlContent = message.Body;

            await client.SendEmailAsync(emailMessage);            
        }
    }
}
