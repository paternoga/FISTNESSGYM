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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        [Inject]
        AuthorizationService AuthorizationService { get; set; }

        protected IEnumerable<FISTNESSGYM.Models.database.ProductCategory> productCategories;

        protected RadzenDataGrid<FISTNESSGYM.Models.database.ProductCategory> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            productCategories = await databaseService.GetProductCategories(new Query { Filter = $@"i => i.Name.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            if (!(AuthorizationService.IsAdmin || AuthorizationService.IsWorker))
            {
                NavigationManager.NavigateTo("/");
            }
            productCategories = await databaseService.GetProductCategories(new Query { Filter = $@"i => i.Name.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await grid0.InsertRow(new FISTNESSGYM.Models.database.ProductCategory());
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.ProductCategory productCategory)
        {
            try
            {
                if (await DialogService.Confirm("Jeste� pewny, �e chcesz usun�� ten rekord?") == true)
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

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await databaseService.ExportProductCategoriesToCSV(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "ProductCategories");
            }

            if (args == null || args.Value == "xlsx")
            {
                await databaseService.ExportProductCategoriesToExcel(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "ProductCategories");
            }
        }

        protected async Task GridRowUpdate(FISTNESSGYM.Models.database.ProductCategory args)
        {
            try
            {
                await databaseService.UpdateProductCategory(args.Id, args);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                      Severity = NotificationSeverity.Error,
                      Summary = $"Error",
                      Detail = $"Unable to update ProductCategory"
                });
            }
        }

        protected async Task GridRowCreate(FISTNESSGYM.Models.database.ProductCategory args)
        {
            try
            {
                await databaseService.CreateProductCategory(args);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                      Severity = NotificationSeverity.Error,
                      Summary = $"Error",
                      Detail = $"Unable to create ProductCategory"
                });
            }
            await grid0.Reload();
        }

        protected async Task EditButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.ProductCategory data)
        {
            await grid0.EditRow(data);
        }

        protected async Task SaveButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.ProductCategory data)
        {
            await grid0.UpdateRow(data);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.ProductCategory data)
        {
            grid0.CancelEditRow(data);
            await databaseService.CancelProductCategoryChanges(data);
        }
    }
}