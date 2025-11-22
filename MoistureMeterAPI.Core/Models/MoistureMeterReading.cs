namespace MoistureMeterAPI.Core.Models
{
    public class MoistureMeterReading : BaseModel
    {
        public float Measure { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
