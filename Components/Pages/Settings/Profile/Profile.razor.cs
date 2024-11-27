using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.WebRequestMethods;

namespace FISTNESSGYM.Components.Pages.Settings.Profile
{
    public partial class Profile
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


        protected string oldPassword = "";
        protected string newPassword = "";
        protected string confirmPassword = "";

        protected string newUsername = "";
        protected string currentUsername = "";

        protected FISTNESSGYM.Models.ApplicationUser user;

        protected string error;
        protected bool errorVisible;
        protected bool successVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            user = await Security.GetUserById($"{Security.User.Id}");
            currentUsername = user.UserName;    
        }

        private async Task ChangeUsername()
        {
            try
            {
                await Security.ChangeUsername(newUsername); 
                successVisible = true;
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }

            StateHasChanged();

        }

        private async Task ChangePassword()
        {
            try
            {
                await Security.ChangePassword(oldPassword, newPassword); 
                successVisible = true;
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }

            StateHasChanged();
        }
    }
}