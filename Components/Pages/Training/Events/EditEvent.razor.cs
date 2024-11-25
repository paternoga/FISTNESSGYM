using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using FISTNESSGYM.Models.database;

namespace FISTNESSGYM.Components.Pages.Training.Events
{
    public partial class EditEvent
    {
        [Inject] protected NotificationService NotificationService { get; set; }

        [Inject] protected DialogService DialogService { get; set; }
        [Inject] protected databaseService databaseService { get; set; }

        [Parameter] public int Id { get; set; } // Event ID

        protected Event _event; // Event object being edited
        protected bool errorVisible; // Display error messages if needed

        protected List<AspNetUser> registeredUsers = new List<AspNetUser>(); // List of users registered to the event
        protected string selectedUserId; // Selected user for removal

        protected override async Task OnInitializedAsync()
        {
            // Load event details
            _event = await databaseService.GetEventById(Id);

            // Load users registered to this event
            registeredUsers = await databaseService.GetUsersRegisteredForEventAsync(Id);
        }

        protected async Task FormSubmit()
        {
            try
            {
                await databaseService.UpdateEvent(Id, _event);
                DialogService.Close(_event);
            }
            catch (Exception)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        protected async Task RemoveUserFromEvent()
        {
            if (!string.IsNullOrEmpty(selectedUserId))
            {
                var confirmed = await DialogService.Confirm("Czy na pewno chcesz wypisaæ tego u¿ytkownika?", "Potwierdzenie wypisu", new ConfirmOptions { OkButtonText = "Tak", CancelButtonText = "Nie" });

                if (confirmed == true)
                {
                    var userToRemove = registeredUsers.Find(u => u.Id == selectedUserId);
                    if (userToRemove != null)
                    {
                        string userEmail = userToRemove.Email;

                       
                        await databaseService.DeleteReservationAsync(Id, selectedUserId);

                        
                        if (_event.Participants > 0)
                        {
                            _event.Participants -= 1; 
                        }

                        registeredUsers.Remove(userToRemove);
                        selectedUserId = null;

                        NotificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = $"U¿ytkownik {userEmail} wypisany",
                            Detail = "U¿ytkownik zosta³ pomyœlnie wypisany z wydarzenia."
                        });

                        await databaseService.UpdateEvent(Id, _event);
                        // Dodanie powiadomienia dla u¿ytkownika w bazie danych
                        var notification = new Notification
                        {
                            UserId = userToRemove.Id,
                            Title = $"Administarator wypisa³ Ciê z zajêæ: {_event.EventName}",
                            CreatedAt = DateTime.UtcNow
                        };
                        await databaseService.CreateNotificationAsync(notification);
                    }
                }
            }
        }

    }
}
