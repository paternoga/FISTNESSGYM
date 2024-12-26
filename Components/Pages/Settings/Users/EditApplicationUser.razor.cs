using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace FISTNESSGYM.Components.Pages.Settings.Users
{
    public partial class EditApplicationUser
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

        protected IEnumerable<FISTNESSGYM.Models.ApplicationRole> roles;
        protected FISTNESSGYM.Models.ApplicationUser user;
        protected IEnumerable<string> userRoles;
        protected string error;
        protected bool errorVisible;

        [Parameter]
        public string Id { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (AuthorizationService.IsWorker || AuthorizationService.IsAdmin)
            {
                user = await Security.GetUserById($"{Id}");
                userRoles = user.Roles.Select(role => role.Id);

                if (AuthorizationService.IsWorker)
                {
                    // Pracownik widzi tylko wybrane role
                    var allowedRoles = new List<string> { "Trener", "U¿ytkownik", "Klient" };
                    roles = (await Security.GetRoles()).Where(role => allowedRoles.Contains(role.Name)).ToList();
                }
                else if (AuthorizationService.IsAdmin)
                {
                    // Administrator widzi wszystkie role
                    roles = await Security.GetRoles();
                }
            }
            else
            {
                navigationManager.NavigateTo("/");
            }
        }


        protected async Task FormSubmit(FISTNESSGYM.Models.ApplicationUser user)
        {
            try
            {
                if (userRoles.Count() > 1)
                {
                    error = "Nie mo¿esz nadaæ kilku ról dla jednej osoby.";
                    errorVisible = true;
                    return;
                }

                // Przypisz jedn¹ rolê
                user.Roles = roles.Where(role => userRoles.Contains(role.Id)).ToList();

                await Security.UpdateUser($"{Id}", user);
                DialogService.Close(null);
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }


        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }
    }
}