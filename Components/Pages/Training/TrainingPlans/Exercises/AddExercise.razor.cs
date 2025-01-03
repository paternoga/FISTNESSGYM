using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace FISTNESSGYM.Components.Pages.Training.TrainingPlans.Exercises
{
    public partial class AddExercise
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
            if (AuthorizationService.IsWorker || AuthorizationService.IsTrainer || AuthorizationService.IsAdmin)
            {
                exercise = new Models.database.Exercise();
            }
            else
            {
                NavigationManager.NavigateTo("/");
            }    
        }
        protected bool errorVisible;
        protected Models.database.Exercise exercise;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await databaseService.CreateExercise(exercise);
                DialogService.Close(exercise);
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