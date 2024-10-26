using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace FISTNESSGYM.Components.Pages.Store
{
    public partial class OrderItems
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

        protected IEnumerable<FISTNESSGYM.Models.database.OrderItem> orderItems;

        protected RadzenDataGrid<FISTNESSGYM.Models.database.OrderItem> grid0;
        protected bool isEdit = true;
        protected override async Task OnInitializedAsync()
        {
            orderItems = await databaseService.GetOrderItems(new Query { Expand = "Order,Product" });

            ordersForOrderId = await databaseService.GetOrders();

            productsForProductId = await databaseService.GetProducts();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            isEdit = false;
            orderItem = new FISTNESSGYM.Models.database.OrderItem();
        }

        protected async Task EditRow(FISTNESSGYM.Models.database.OrderItem args)
        {
            isEdit = true;
            orderItem = args;
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.OrderItem orderItem)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await databaseService.DeleteOrderItem(orderItem.Id);

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
                    Detail = $"Unable to delete OrderItem"
                });
            }
        }
        protected bool errorVisible;
        protected FISTNESSGYM.Models.database.OrderItem orderItem;

        protected IEnumerable<FISTNESSGYM.Models.database.Order> ordersForOrderId;

        protected IEnumerable<FISTNESSGYM.Models.database.Product> productsForProductId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                var result = isEdit ? await databaseService.UpdateOrderItem(orderItem.Id, orderItem) : await databaseService.CreateOrderItem(orderItem);

            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {

        }
    }
}