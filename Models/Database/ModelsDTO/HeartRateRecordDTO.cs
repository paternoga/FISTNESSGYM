namespace FISTNESSGYM.Models.Database.ModelsDTO
{
    public class HeartRateRecordDTO
    {
        public DateTime Time { get; set; }
        public string FormattedTime { get; set; }
        public int HeartRate { get; set; }
    }
}
