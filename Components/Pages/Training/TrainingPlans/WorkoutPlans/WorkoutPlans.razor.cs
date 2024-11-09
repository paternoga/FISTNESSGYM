using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using FISTNESSGYM.Services;
using Radzen;
using Radzen.Blazor;
using Microsoft.AspNetCore.Authentication;
using FISTNESSGYM.Models.database;
using Microsoft.IdentityModel.Tokens;

namespace FISTNESSGYM.Components.Pages.Training.TrainingPlans.WorkoutPlans
{
    public partial class WorkoutPlans
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

        protected IEnumerable<FISTNESSGYM.Models.database.WorkoutPlan> workoutPlans;

        protected RadzenDataGrid<FISTNESSGYM.Models.database.WorkoutPlan> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            workoutPlans = await databaseService.GetWorkoutPlans(new Query { Filter = $@"i => i.Name.Contains(@0) || i.Description.Contains(@0) || i.UserId.Contains(@0)", FilterParameters = new object[] { search }, Expand = "AspNetUser" });
        }
        protected override async Task OnInitializedAsync()
        {
            if (AuthorizationService.IsAdmin || AuthorizationService.IsTrainer || AuthorizationService.IsWorker)
            {
                workoutPlans = await databaseService.GetWorkoutPlans(new Query { Filter = $@"i => i.Name.Contains(@0) || i.Description.Contains(@0) || i.UserId.Contains(@0)", FilterParameters = new object[] { search }, Expand = "AspNetUser" });
            }
            else if(AuthorizationService.IsClient) 
            {
                string userId = Security.User?.Id;
                if (!string.IsNullOrEmpty(userId))
                {
                    workoutPlans = (await databaseService.GetWorkoutPlansForUser(userId)).ToList() ?? new List<WorkoutPlan>();
                }
            }

            
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            if (AuthorizationService.IsTrainer || AuthorizationService.IsAdmin || AuthorizationService.IsWorker)
            {
                await DialogService.OpenAsync<AddWorkoutPlan>("Add WorkoutPlan", null);
                await grid0.Reload(); 
            }
            else if (AuthorizationService.IsClient)
            {
                var result = await DialogService.OpenAsync<AddWorkoutPlan>("Add WorkoutPlan", null);

                if (result != null) 
                {
                    await LoadClientWorkoutPlans(); 
                }
            }
        }


        protected async Task EditRow(FISTNESSGYM.Models.database.WorkoutPlan args)
        {
            await DialogService.OpenAsync<EditWorkoutPlan>("Edit WorkoutPlan", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.WorkoutPlan workoutPlan)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await databaseService.DeleteWorkoutPlan(workoutPlan.Id);

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
                    Detail = $"Unable to delete WorkoutPlan"
                });
            }
        }
        private async Task LoadClientWorkoutPlans()
        {
            string userId = Security.User?.Id; 

            if (!string.IsNullOrEmpty(userId))
            {

                workoutPlans = (await databaseService.GetWorkoutPlansForUser(userId)).ToList() ?? new List<WorkoutPlan>();
            }
            else
            {
                workoutPlans = new List<WorkoutPlan>();
            }

            StateHasChanged(); 
        }
        private async Task DeleteClientWorkoutPlan(int planId)
        {
            var confirmed = await DialogService.Confirm("Czy na pewno chcesz usun¹æ ten plan treningowy?", "Potwierdzenie",
                                                         new ConfirmOptions { OkButtonText = "Tak", CancelButtonText = "Nie" });

            if (confirmed == true)
            {
                try
                {
                    var deleteResult = await databaseService.DeleteWorkoutPlan(planId);

                    if (deleteResult != null)
                    {
                        await LoadClientWorkoutPlans(); 
                        NotificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = "Plan usuniêty",
                            Detail = "Plan treningowy zosta³ pomyœlnie usuniêty."
                        });
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "B³¹d",
                        Detail = "Nie uda³o siê usun¹æ planu treningowego."
                    });
                }
            }
        }
        protected async Task EditClientWorkoutPlan(int workoutPlanId)
        {
            var result = await DialogService.OpenAsync<EditWorkoutPlan>("Edytuj Plan",
                new Dictionary<string, object> { { "Id", workoutPlanId } });
            if (result != null)
            {
                await LoadClientWorkoutPlans();
            }
        }


    }
}