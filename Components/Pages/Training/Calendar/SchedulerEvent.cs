namespace FISTNESSGYM.Components.Pages.Calendar
{
    public class SchedulerEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string InstructorName { get; set; } 
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int MaxParticipants { get; set; } 
        public int Participants { get; set; } 
    }
}
