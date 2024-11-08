namespace FISTNESSGYM.Models.Database.ModelsDTO
{
    public class WeatherDataDTO
    {
        public string Name { get; set; }
        public MainDTO Main { get; set; }
        public WeatherDTO[] Weather { get; set; }
    }
}
