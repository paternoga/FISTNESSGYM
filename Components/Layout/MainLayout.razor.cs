using System.Net.Http;
using FISTNESSGYM.Models.database;
using FISTNESSGYM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace FISTNESSGYM.Components.Layout
{
    public partial class MainLayout
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
        protected SecurityService SecurityService { get; set; } 

        [Inject]
        protected SecurityService Security { get; set; }
        [Inject]
        protected databaseService DatabaseService { get; set; }


        private bool sidebarExpanded = true;
        private bool isDarkMode = true;

        private bool isUser;
        private bool isClient;
        private bool isWorker;
        private bool isTrainer;
        private bool isAdmin;
        private string currentUserId;

        private string GetLogoPath()
        {
            return isDarkMode ? "images/logo.png" : "images/logo2.png";
        }

        void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
        }

        protected void ProfileMenuClick(RadzenProfileMenuItem args)
        {
            if (args.Value == "Logout")
            {
                Security.Logout();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            // Check if the user is authenticated and fetch their role
            if (SecurityService.User != null && SecurityService.IsAuthenticated())
            {
                // Check if the user is in the "Client" role
                isUser = SecurityService.IsInRole("U¿ytkownik");
                isClient = SecurityService.IsInRole("Klient");
                isWorker = SecurityService.IsInRole("Pracownik");
                isTrainer = SecurityService.IsInRole("Trener");
                isAdmin = SecurityService.IsInRole("Administrator");
                // Retrieve the logged-in user's ID
                currentUserId = SecurityService.User.Id;

            }



        }

        private bool showNotificationsPanel = false;
        private List<Notification> notifications = new();


        private async Task ToggleNotificationsPanel()
        {
            showNotificationsPanel = !showNotificationsPanel;

            if (showNotificationsPanel)
            {
                // £adowanie powiadomieñ tylko przy pierwszym otwarciu panelu
                await LoadNotifications();
            }
        }


        private async Task LoadNotifications()
        {
            if (!string.IsNullOrEmpty(currentUserId))
            {
                // Pobierz powiadomienia u¿ytkownika z serwisu bazy danych
                notifications = await DatabaseService.GetNotificationsForUserAsync(currentUserId);
            }
        }

        private async Task RemoveNotification(Notification notification)
        {
            if (notification != null)
            {
                // Usuñ powiadomienie z bazy danych
                await DatabaseService.DeleteNotificationAsync(notification.Id);

                // Usuñ powiadomienie z listy w interfejsie
                notifications.Remove(notification);

                // Zaktualizuj interfejs
                StateHasChanged();
            }
        }

    }
}
