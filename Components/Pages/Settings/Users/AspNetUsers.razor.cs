using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using FISTNESSGYM.Components.Pages.Training.TrainingPlans.Exercises;

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

        [Inject]
        protected SecurityService Security { get; set; }

        protected IEnumerable<FISTNESSGYM.Models.database.AspNetUser> aspNetUsers;

        protected RadzenDataGrid<FISTNESSGYM.Models.database.AspNetUser> grid0;

        protected string search = "";

        protected override async Task OnInitializedAsync()
        {
            if (AuthorizationService.IsWorker || AuthorizationService.IsAdmin)
            {
                aspNetUsers = await databaseService.GetAspNetUsers();
            }
            else
            {
                navigationManager.NavigateTo("/");
            }

        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await grid0.GoToPage(0);

            aspNetUsers = await databaseService.GetAspNetUsers(new Query { Filter = $@"i => i.Email.Contains(@0) || i.UserName.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddAspNetUser>("Dodaj u�ytkownika", null);
            await grid0.Reload();
        }

        protected async Task EditRow(FISTNESSGYM.Models.database.AspNetUser args)
        {
            await DialogService.OpenAsync<EditAspNetUser>("Edytuj u�ytkownika", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.AspNetUser aspNetUser)
        {
            try
            {
                if (await DialogService.Confirm("Jeste� pewny, �e chcesz usun�� ten rekord?") == true)
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