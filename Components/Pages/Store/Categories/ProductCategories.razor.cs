using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace FISTNESSGYM.Components.Pages.Store.Categories
{
    public partial class ProductCategories
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

        protected IEnumerable<FISTNESSGYM.Models.database.ProductCategory> productCategories;

        protected RadzenDataGrid<FISTNESSGYM.Models.database.ProductCategory> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnInitializedAsync()
        {
            productCategories = await databaseService.GetProductCategories();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddProductCategory>("Add ProductCategory", null);
            await grid0.Reload();
        }

        protected async Task EditRow(FISTNESSGYM.Models.database.ProductCategory args)
        {
            await DialogService.OpenAsync<EditProductCategory>("Edit ProductCategory", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.ProductCategory productCategory)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await databaseService.DeleteProductCategory(productCategory.Id);

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
                    Detail = $"Unable to delete ProductCategory"
                });
            }
        }
    }
}