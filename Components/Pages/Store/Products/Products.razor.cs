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

namespace FISTNESSGYM.Components.Pages.Store.Products
{
    public partial class Products
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

        protected IEnumerable<FISTNESSGYM.Models.database.Product> products;

        protected RadzenDataGrid<FISTNESSGYM.Models.database.Product> grid0;

        [Inject]
        protected SecurityService Security { get; set; }
        protected string search = "";

        protected override async Task OnInitializedAsync()
        {
            products = await databaseService.GetProducts(new Query { Expand = "ProductCategory" });
        }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await grid0.GoToPage(0);

            products = await databaseService.GetProducts(new Query { Filter = $@"i => i.Name.Contains(@0) || i.Description.Contains(@0) || i.Category.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddProduct>("Dodaj produkt", null);
            await grid0.Reload();
        }

        protected async Task EditRow(FISTNESSGYM.Models.database.Product args)
        {
            await DialogService.OpenAsync<EditProduct>("Edytuj Produkt", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.Product product)
        {
            try
            {
                if (await DialogService.Confirm("Jesteœ pewny, ¿e chcesz usun¹æ ten rekord?") == true)
                {
                    var deleteResult = await databaseService.DeleteProduct(product.Id);

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
                    Detail = $"Unable to delete Product"
                });
            }
        }
    }
}