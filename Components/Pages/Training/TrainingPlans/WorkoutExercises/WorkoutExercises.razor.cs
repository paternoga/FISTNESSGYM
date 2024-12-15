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
            string trainerEmail = Security.User?.Email;

            if (AuthorizationService.IsAdmin || AuthorizationService.IsWorker)
            {
                // Admin lub pracownik widzi wszystko
                workoutExercises = await databaseService.GetWorkoutExercises(new Query { Expand = "Exercise,WorkoutPlan" });
            }
            else if (AuthorizationService.IsTrainer && !string.IsNullOrEmpty(trainerEmail))
            {
                // Trener widzi tylko swoje ćwiczenia
                workoutExercises = await databaseService.GetWorkoutExercises(new Query
                {
                    Filter = $@"i => i.WorkoutPlan.InstructorEmail == @0",
                    FilterParameters = new object[] { trainerEmail },
                    Expand = "Exercise,WorkoutPlan"
                });
            }
            else if (AuthorizationService.IsClient)
            {
                string userId = Security.User?.Id;

                if (!string.IsNullOrEmpty(userId))
                {
                    // Klient widzi tylko ćwiczenia przypisane do swoich planów treningowych
                    workoutExercises = await databaseService.GetWorkoutExercisesForUser(userId);
                }
            }
        }


        protected async Task AddButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddWorkoutExercise>("Add WorkoutExercise", null);

            if (result != null) // Sprawdzamy, czy dodanie �wiczenia si� powiod�o
            {
                if (AuthorizationService.IsClient)
                {
                    // Ponownie za�aduj �wiczenia klienta
                    workoutExercises = await databaseService.GetWorkoutExercisesForUser(Security.User?.Id);
                    StateHasChanged(); // Od�wie�enie widoku
                }
                else
                {
                    // Od�wie� grid dla administratora
                    await grid0.Reload();
                }
            }
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
                        if (AuthorizationService.IsClient)
                        {
                            workoutExercises = await databaseService.GetWorkoutExercisesForUser(Security.User?.Id);
                            StateHasChanged(); 
                        }
                        else
                        {
                            await grid0.Reload(); 
                        }
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