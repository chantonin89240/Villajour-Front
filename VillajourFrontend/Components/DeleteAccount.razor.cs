using Microsoft.AspNetCore.Components;
using Radzen;

namespace VillajourFrontend.Components
{
    public partial class DeleteAccount
    {
        [Inject]
        protected NotificationService NotificationService { get; set; }

        protected bool Confirmed;

        protected void Confirmer()
        {
            if (Confirmed)
            {
                Confirmed = false;
            }
            else
            {
                Confirmed = true;
            }
            DialogService.Close();

            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Duration = 4000,
                Summary = "Votre compte sera supprimé dans 30 jours",
                Detail = ""
            });
        }

        protected void Annuler()
        {
            DialogService.Close();
        }
    }
}
