using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using BugTracker.Models;

namespace BugTracker.Helpers
{
    public class EmailHelper
    {
        private static string ConfiguredEmail = WebConfigurationManager.AppSettings["emailto"];

        public static async Task ComposeConfirmEmailAsync(RegisterViewModel model, string callbackUrl)
        {
            try
            {
                var senderEmail = $"Bugtracker<{ConfiguredEmail}>";
                var mailMsg = new MailMessage(senderEmail, model.Email)
                {
                    Subject = "Confirm your account",
                    Body = $"Please confirm your account by clicking <a href=\"{callbackUrl}\"> here",
                    IsBodyHtml = true
                };

                var svc = new PersonalEmail();
                await svc.SendAsync(mailMsg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
        }

        public static async Task ComposeForgotPasswordEmailAsync(ForgotPasswordViewModel model, string callbackUrl)
        {
            try
            {
                var senderEmail = $"Bugtracker<{ConfiguredEmail}>";
                var mailMsg = new MailMessage(senderEmail, model.Email)
                {
                    Subject = "Reset Password",
                    Body = $"Please reset your password by clicking <a href=\"{callbackUrl}\"> here",
                    IsBodyHtml = true
                };

                var svc = new PersonalEmail();
                await svc.SendAsync(mailMsg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.FromResult(0);
            }
        }
    }
}