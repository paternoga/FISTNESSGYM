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
    public partial class EditWorkoutExercise
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

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            workoutExercise = await databaseService.GetWorkoutExerciseById(Id);

            exercisesForExerciseId = await databaseService.GetExercises();

            workoutPlansForWorkoutPlanId = await databaseService.GetWorkoutPlans();
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
                await databaseService.UpdateWorkoutExercise(Id, workoutExercise);
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