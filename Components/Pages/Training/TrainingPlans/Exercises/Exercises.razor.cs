using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace FISTNESSGYM.Components.Pages.Training.TrainingPlans.Exercises
{
    public partial class Exercises
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

        protected IEnumerable<FISTNESSGYM.Models.database.Exercise> exercises;

        protected RadzenDataGrid<FISTNESSGYM.Models.database.Exercise> grid0;

        protected string search = "";

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            exercises = await databaseService.GetExercises(new Query { Filter = $@"i => i.Name.Contains(@0) || i.Description.Contains(@0) || i.Category.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            exercises = await databaseService.GetExercises(new Query { Filter = $@"i => i.Name.Contains(@0) || i.Description.Contains(@0) || i.Category.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddExercise>("Dodaj �wiczenie", null);
            await grid0.Reload();
        }

        protected async Task EditRow(FISTNESSGYM.Models.database.Exercise args)
        {
            await DialogService.OpenAsync<EditExercise>("Edytuj �wiczenie", new Dictionary<string, object> { {"Id", args.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, FISTNESSGYM.Models.database.Exercise exercise)
        {
            try
            {
                if (await DialogService.Confirm("Jeste� pewny, �e chcesz usun�� ten rekord?") == true)
                {
                    var deleteResult = await databaseService.DeleteExercise(exercise.Id);

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
                    Detail = $"Unable to delete Exercise"
                });
            }
        }
    }
}