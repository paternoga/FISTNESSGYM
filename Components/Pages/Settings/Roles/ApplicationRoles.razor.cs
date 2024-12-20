using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using FISTNESSGYM.Services;

namespace FISTNESSGYM.Components.Pages.Settings.Roles
{
    public partial class ApplicationRoles
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
        [Inject]
        AuthorizationService AuthorizationService { get; set; }

        protected IEnumerable<FISTNESSGYM.Models.ApplicationRole> roles;
        protected RadzenDataGrid<FISTNESSGYM.Models.ApplicationRole> grid0;
        protected string error;
        protected bool errorVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!AuthorizationService.IsAdmin)
            {
                NavigationManager.NavigateTo("/");
            }
            roles = await Security.GetRoles();
        }

        protected async Task AddClick()
        {
            await DialogService.OpenAsync<AddApplicationRole>("Dodaj role");

            roles = await Security.GetRoles();
        }

        protected async Task DeleteClick(FISTNESSGYM.Models.ApplicationRole role)
        {
            try
            {
                if (await DialogService.Confirm("Jeste� pewny, �e chcesz usun�� ten rekord?") == true)
                {
                    await Security.DeleteRole($"{role.Id}");

                    roles = await Security.GetRoles();
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