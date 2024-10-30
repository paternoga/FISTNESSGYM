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
    public partial class AspNetUsers
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
        public databaseService databaseService { get; set; }

        protected IEnumerable<FISTNESSGYM.Models.database.AspNetUser> aspNetUsers;

        protected RadzenDataGrid<FISTNESSGYM.Models.database.AspNetUser> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            aspNetUsers = await databaseService.GetAspNetUsers();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddAspNetUser>("Add AspNetUser", null);
            await grid0.Reload();
        }

        protected async Task EditRow(FISTNESSGYM.Models.database.AspNetUser args)
        {
            await DialogService.OpenAsync<EditAspNetUser>("Edit AspNetUser", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.AspNetUser aspNetUser)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await databaseService.DeleteAspNetUser(aspNetUser.Id);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete AspNetUser"
                });
            }
        }
    }
}