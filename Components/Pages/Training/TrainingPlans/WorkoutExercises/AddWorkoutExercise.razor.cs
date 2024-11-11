using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace FISTNESSGYM.Components.Pages.Training.TrainingPlans.WorkoutExercises
{
    public partial class AddWorkoutExercise
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
            workoutExercise = new FISTNESSGYM.Models.database.WorkoutExercise();

            exercisesForExerciseId = (await databaseService.GetExercises()).ToList() ?? new List<FISTNESSGYM.Models.database.Exercise>();

            if (AuthorizationService.IsClient)
            {
                string userId = Security.User?.Id;
                if (!string.IsNullOrEmpty(userId))
                {
                    workoutPlansForWorkoutPlanId = (await databaseService.GetWorkoutPlansForUser(userId)).ToList() ?? new List<FISTNESSGYM.Models.database.WorkoutPlan>();
                }
                else
                {
                    workoutPlansForWorkoutPlanId = new List<FISTNESSGYM.Models.database.WorkoutPlan>();
                }
            }
            else if(AuthorizationService.IsWorker || AuthorizationService.IsTrainer || AuthorizationService.IsAdmin)
            {
                workoutPlansForWorkoutPlanId = (await databaseService.GetWorkoutPlans()).ToList() ?? new List<FISTNESSGYM.Models.database.WorkoutPlan>();
            }
        }
        protected bool errorVisible;
        protected FISTNESSGYM.Models.database.WorkoutExercise workoutExercise;

        protected IEnumerable<FISTNESSGYM.Models.database.Exercise> exercisesForExerciseId;

        protected IEnumerable<FISTNESSGYM.Models.database.WorkoutPlan> workoutPlansForWorkoutPlanId;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                await databaseService.CreateWorkoutExercise(workoutExercise);
                DialogService.Close(workoutExercise);
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