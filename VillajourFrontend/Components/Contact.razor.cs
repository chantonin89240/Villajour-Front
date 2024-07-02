using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace VillajourFrontend.Components
{
    public partial class Contact
    {
        [Inject]
        protected NotificationService NotificationService { get; set; }

        private EmailModel emailModel = new EmailModel();

        protected bool Confirmed;

        protected async Task Confirmer()
        {
            DialogService.Close();

            try
            {
                var smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("villajour@outlook.com", "xygpu5-kurhyH-xugnar"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("villajour@outlook.com"),
                    Subject = emailModel.Sujet,
                    Body = emailModel.Message,
                    IsBodyHtml = false,
                };

                mailMessage.To.Add("villajour@outlook.com");

                await smtpClient.SendMailAsync(mailMessage);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Duration = 4000,
                    Summary = "Votre mail a bien été envoyé",
                    Detail = ""
                });
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Duration = 4000,
                    Summary = ex.Message,
                    Detail = ""
                });
            }

        }

        protected void Annuler()
        {
            DialogService.Close();
        }

        public class EmailModel
        {
            public string Nom { get; set; }
            public string Email { get; set; }
            public string Sujet { get; set; }
            public string Message { get; set; }
        }
    }
}
