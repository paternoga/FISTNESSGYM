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
    public partial class ApplicationUsers
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

        protected IEnumerable<FISTNESSGYM.Models.ApplicationUser> users;
        protected RadzenDataGrid<FISTNESSGYM.Models.ApplicationUser> grid0;
        protected string error;
        protected bool errorVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (AuthorizationService.IsWorker || AuthorizationService.IsAdmin)
            {
                users = await Security.GetUsers();
            }
            else
            {
                navigationManager.NavigateTo("/");
            }
                
        }

        protected async Task AddClick()
        {
            await DialogService.OpenAsync<AddApplicationUser>("Dodaj u�ytkownika");

            users = await Security.GetUsers();
        }

        protected async Task RowSelect(FISTNESSGYM.Models.ApplicationUser user)
        {
            await DialogService.OpenAsync<EditApplicationUser>("Edytuj u�ytkownika", new Dictionary<string, object>{ {"Id", user.Id} });

            users = await Security.GetUsers();
        }

        protected async Task DeleteClick(FISTNESSGYM.Models.ApplicationUser user)
        {
            try
            {
                if (await DialogService.Confirm("Jeste� pewny, �e chcesz usun�� ten rekord?") == true)
                {
                    await Security.DeleteUser($"{user.Id}");

                    users = await Security.GetUsers();
                }
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }
    }
}