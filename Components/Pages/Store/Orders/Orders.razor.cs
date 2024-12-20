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
using FISTNESSGYM.Models.database;

namespace FISTNESSGYM.Components.Pages.Store.Orders
{
    public partial class Orders
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
        protected AuthorizationService AuthorizationService { get; set; }



        protected IEnumerable<FISTNESSGYM.Models.database.Order> orders;

        protected RadzenDataGrid<FISTNESSGYM.Models.database.Order> grid0;
        protected bool isEdit = true;
        protected override async Task OnInitializedAsync()
        {
            if (AuthorizationService.IsAdmin  || AuthorizationService.IsWorker)
            {
                orders = await databaseService.GetOrders(new Query { Expand = "AspNetUser,OrderStatus" });
            }
            else if (AuthorizationService.IsClient)
            {
                string userId = Security.User?.Id;

                if (!string.IsNullOrEmpty(userId))
                {
                    orders = await databaseService.GetOrdersForUser(userId);
                }
            }
            else if (AuthorizationService.IsTrainer)
            {
                NavigationManager.NavigateTo("/");
            }

                aspNetUsersForUserId = await databaseService.GetAspNetUsers();
            orderStatusesForOrderStatusId = await databaseService.GetOrderStatuses();
        }


        protected async Task AddButtonClick(MouseEventArgs args)
        {
            isEdit = false;
            order = new FISTNESSGYM.Models.database.Order();
        }

        protected async Task EditRow(FISTNESSGYM.Models.database.Order args)
        {
            isEdit = true;
            order = args;
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.Order order)
        {
            try
            {
                if (await DialogService.Confirm("Jeste� pewny, �e chcesz usun�� ten rekord?") == true)
                {
                    var deleteResult = await databaseService.DeleteOrder(order.Id);

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
                    Detail = $"Unable to delete Order"
                });
            }
        }
        protected bool errorVisible;
        protected FISTNESSGYM.Models.database.Order order;

        protected IEnumerable<FISTNESSGYM.Models.database.AspNetUser> aspNetUsersForUserId;

        protected IEnumerable<FISTNESSGYM.Models.database.OrderStatus> orderStatusesForOrderStatusId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                var result = isEdit ? await databaseService.UpdateOrder(order.Id, order) : await databaseService.CreateOrder(order);

                if (isEdit)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Zam�wienie zaktualizowane",
                        Detail = "Zam�wienie zosta�o pomy�lnie zaktualizowane."
                    });
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Zam�wienie dodane",
                        Detail = "Nowe zam�wienie zosta�o pomy�lnie dodane."
                    });
                }

            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {

        }

        protected async Task PayOrder(FISTNESSGYM.Models.database.Order order)
        {
            try
            {
                var success = await databaseService.PayOrder(order.Id); // Upewnij si�, �e PayOrder zwraca wynik

                if (success)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Sukces",
                        Detail = "Zam�wienie zosta�o op�acone"
                    });

                    var notification = new Notification
                    {
                        UserId = Security.User?.Id,
                        Title = $"Twoje zam�wienie zosta�o pomy�lnie op�acone.",
                        CreatedAt = DateTime.UtcNow.AddHours(1)
                    };
                    await databaseService.CreateNotificationAsync(notification);

                    await grid0.Reload(); // Od�wie� siatk� danych, aby pokaza� zmieniony status
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Warning,
                        Summary = "B��d",
                        Detail = "Nie uda�o si� op�aci� zam�wienia"
                    });
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "B��d",
                    Detail = $"Nie uda�o si� op�aci� zam�wienia: {ex.Message}"
                });
            }
        }

    }
}