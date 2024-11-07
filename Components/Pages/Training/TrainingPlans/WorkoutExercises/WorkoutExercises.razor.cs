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
    public partial class WorkoutExercises
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

        protected IEnumerable<FISTNESSGYM.Models.database.WorkoutExercise> workoutExercises;

        protected RadzenDataGrid<FISTNESSGYM.Models.database.WorkoutExercise> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            workoutExercises = await databaseService.GetWorkoutExercises(new Query { Filter = $@"i => i.WorkoutPlan.Name.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            workoutExercises = await databaseService.GetWorkoutExercises(new Query { Expand = "Exercise,WorkoutPlan" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddWorkoutExercise>("Add WorkoutExercise", null);
            await grid0.Reload();
        }

        protected async Task EditRow(FISTNESSGYM.Models.database.WorkoutExercise args)
        {
            await DialogService.OpenAsync<EditWorkoutExercise>("Edit WorkoutExercise", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.WorkoutExercise workoutExercise)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await databaseService.DeleteWorkoutExercise(workoutExercise.Id);

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
                    Summary = $"Error",
                    Detail = $"Unable to delete WorkoutExercise"
                });
            }
        }
    }
}