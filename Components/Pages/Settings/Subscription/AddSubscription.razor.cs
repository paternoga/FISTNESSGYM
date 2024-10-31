using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace FISTNESSGYM.Components.Pages.Settings.Subscription
{
    public partial class AddSubscription
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

        protected override async Task OnInitializedAsync()
        {
            subscription = new FISTNESSGYM.Models.database.Subscription();

            aspNetUsersForUserId = await databaseService.GetAspNetUsers();

            subscriptionStatusesForSubscriptionStatusId = await databaseService.GetSubscriptionStatuses();
        }
        protected bool errorVisible;
        protected FISTNESSGYM.Models.database.Subscription subscription;

        protected IEnumerable<FISTNESSGYM.Models.database.AspNetUser> aspNetUsersForUserId;

        protected IEnumerable<FISTNESSGYM.Models.database.SubscriptionStatus> subscriptionStatusesForSubscriptionStatusId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await databaseService.CreateSubscription(subscription);
                DialogService.Close(subscription);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}