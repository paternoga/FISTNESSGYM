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

namespace FISTNESSGYM.Components.Pages.Settings.Subscription
{
    public partial class Subscriptions
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

        protected IEnumerable<FISTNESSGYM.Models.database.Subscription> subscriptions;

        protected RadzenDataGrid<FISTNESSGYM.Models.database.Subscription> grid0;

        protected string search = "";
        [Inject]
        AuthorizationService AuthorizationService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!(AuthorizationService.IsAdmin || AuthorizationService.IsWorker))
            {
                NavigationManager.NavigateTo("/");
            }
            subscriptions = await databaseService.GetSubscriptions(new Query { Expand = "AspNetUser,SubscriptionStatus" });
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            subscriptions = await databaseService.GetSubscriptions(new Query { Filter = $@"i => i.AspNetUser.UserName.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddSubscription>("Dodaj Subskrypcjê", null);
            await grid0.Reload();
        }

        protected async Task EditRow(FISTNESSGYM.Models.database.Subscription args)
        {
            await DialogService.OpenAsync<EditSubscription>("Edytuj Subskrypcjê", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.Subscription subscription)
        {
            try
            {
                if (await DialogService.Confirm("Jesteœ pewny, ¿e chcesz usun¹æ ten rekord?") == true)
                {
                    var deleteResult = await databaseService.DeleteSubscription(subscription.Id);

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
                    Detail = $"Unable to delete Subscription"
                });
            }
        }
    }
}