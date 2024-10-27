using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using FISTNESSGYM.Components.Pages.Account;

namespace FISTNESSGYM.Components.Pages.Login
{
    public partial class Login
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        protected string redirectUrl;
        protected string error;
        protected string info;
        protected bool errorVisible;
        protected bool infoVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var query = System.Web.HttpUtility.ParseQueryString(new Uri(NavigationManager.ToAbsoluteUri(NavigationManager.Uri).ToString()).Query);

            error = query.Get("error");

            info = query.Get("info");

            redirectUrl = query.Get("redirectUrl");

            errorVisible = !string.IsNullOrEmpty(error);

            infoVisible = !string.IsNullOrEmpty(info);
        }

        protected async Task Register()
        {
            var result = await DialogService.OpenAsync<RegisterApplicationUser>("Rejestracja użytkownika");

            if (result == true)
            {
                infoVisible = true;

                info = "Rejestracja powiodła się. Aby uzyskać dalsze instrukcje, sprawdź swoją pocztę e-mail.";
            }
        }

        protected async Task ResetPassword()
        {
            var result = await DialogService.OpenAsync<ResetPassword>("Reset hasła");

            if (result == true)
            {
                infoVisible = true;

                info = "Reset hasła powiódł się. Aby uzyskać dalsze instrukcje, sprawdź swoją pocztę e-mail.";
            }
        }
    }
}