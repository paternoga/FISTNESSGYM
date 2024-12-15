using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace FISTNESSGYM.Components.Pages.Training.TrainingPlans.WorkoutPlans
{
    public partial class AddWorkoutPlan
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
            workoutPlan = new FISTNESSGYM.Models.database.WorkoutPlan();

            aspNetUsersForUserId = await databaseService.GetAspNetUsers();
        }
        protected bool errorVisible;
        protected FISTNESSGYM.Models.database.WorkoutPlan workoutPlan;

        protected IEnumerable<FISTNESSGYM.Models.database.AspNetUser> aspNetUsersForUserId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                if (AuthorizationService.IsTrainer)
                {
                    workoutPlan.InstructorEmail = Security.User?.Email;
                }

                workoutPlan.CreatedDate = DateTime.Now;

                await databaseService.CreateWorkoutPlan(workoutPlan);
                DialogService.Close(workoutPlan);
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
        protected async Task FormSubmitForClient()
        {
            try
            {
                // Ustawiamy UserId na aktualnego u¿ytkownika i datê utworzenia
                workoutPlan.UserId = Security.User?.Id;
                workoutPlan.CreatedDate = DateTime.Now;
                workoutPlan.InstructorEmail = "";

                await databaseService.CreateWorkoutPlan(workoutPlan);
                DialogService.Close(workoutPlan);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }
    }
}