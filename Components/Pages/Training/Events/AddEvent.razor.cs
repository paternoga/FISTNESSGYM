using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace FISTNESSGYM.Components.Pages.Training.Events
{
    public partial class AddEvent
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

        protected bool errorVisible;
        protected FISTNESSGYM.Models.database.Event _event;
        List<string> instructorEmails = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            _event = new FISTNESSGYM.Models.database.Event();
            // Wczytujemy emaile instruktor�w od razu po za�adowaniu strony, tak jak w event-dialog
            instructorEmails = await databaseService.GetTrainerEmailsAsync();
        }

        protected async Task FormSubmit()
        {
            try
            {
                await databaseService.CreateEvent(_event);
                DialogService.Close(_event);
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