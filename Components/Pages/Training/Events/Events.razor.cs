using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using FISTNESSGYM.Models.database;
using Radzen.Blazor;

namespace FISTNESSGYM.Components.Pages.Training.Events
{
    public partial class Events
    {
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public DialogService DialogService { get; set; }
        [Inject] public NotificationService NotificationService { get; set; }
        [Inject] public databaseService databaseService { get; set; }
        [Inject] public SecurityService Security { get; set; }

        protected IEnumerable<Event> events;
        protected RadzenDataGrid<Event> grid0;
        private bool isEmployee;
        private bool isClient;
        private string currentUserId;

        protected override async Task OnInitializedAsync()
        {
            // Determine user role
            isEmployee = Security.IsInRole("Pracownik");
            isClient = Security.IsInRole("Klient");
            currentUserId = Security.User?.Id;

            // Load appropriate events based on role
            if (isEmployee)
            {
                events = await databaseService.GetEvents(); // All events for employee
            }
            else if (isClient)
            {
                events = await databaseService.GetUserRegisteredEventsAsync(currentUserId); // Registered events for client
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            if (isEmployee) 
            {
                await DialogService.OpenAsync<AddEvent>("Add Event", null);
                await grid0.Reload();
            }

        }

        protected async Task EditRow(Event args)
        {
            if (isEmployee)
            {
                await DialogService.OpenAsync<EditEvent>("Edit Event", new Dictionary<string, object> { { "Id", args.Id } });
            }
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Event _event)
        {
            try
            {
                var confirmOptions = new ConfirmOptions
                {
                    OkButtonText = "Tak",
                    CancelButtonText = "Nie"
                };

                if (await DialogService.Confirm("Czy na pewno chcesz usun¹æ ten rekord?", "Potwierdzenie", confirmOptions) == true)
                {
                    var deleteResult = await databaseService.DeleteEvent(_event.Id);
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
                    Summary = "B³¹d",
                    Detail = "Nie uda³o siê usun¹æ wydarzenia"
                });
            }
        }
        private async Task OnUnregisterClick(MouseEventArgs args, Event _event)
        {
            var confirmed = await DialogService.Confirm("Czy na pewno chcesz siê wypisaæ z tego wydarzenia?", "Potwierdzenie wypisu",
                new ConfirmOptions { OkButtonText = "Tak", CancelButtonText = "Nie" });

            if (confirmed == true)
            {
                try
                {
                    await databaseService.DeleteReservationAsync(_event.Id, currentUserId);

                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Wypisano",
                        Detail = "Zosta³eœ wypisany z wydarzenia."
                    });


                   if (isClient)
                    {
                        events = await databaseService.GetUserRegisteredEventsAsync(currentUserId);
                    }
                    await grid0.Reload();
                }
                catch (Exception ex)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "B³¹d",
                        Detail = "Nie uda³o siê wypisaæ z wydarzenia."
                    });
                }
            }
        }


    }
}
