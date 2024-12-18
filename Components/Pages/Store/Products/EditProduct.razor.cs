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

namespace FISTNESSGYM.Components.Pages.Store.Products
{
    public partial class EditProduct
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

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!(AuthorizationService.IsAdmin || AuthorizationService.IsWorker))
            {
                NavigationManager.NavigateTo("/");
            }
            product = await databaseService.GetProductById(Id);

            productCategoriesForCategoryId = await databaseService.GetProductCategories();
        }
        protected bool errorVisible;
        protected FISTNESSGYM.Models.database.Product product;

        protected IEnumerable<FISTNESSGYM.Models.database.ProductCategory> productCategoriesForCategoryId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await databaseService.UpdateProduct(Id, product);
                DialogService.Close(product);
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

        private void OnCategoryChange(object args)
        {
            if (args is int selectedCategoryId)
            {
                var selectedCategory = productCategoriesForCategoryId.FirstOrDefault(c => c.Id == selectedCategoryId);
                if (selectedCategory != null)
                {
                    product.Category = selectedCategory.Name; 
                }
            }
        }
    }
}