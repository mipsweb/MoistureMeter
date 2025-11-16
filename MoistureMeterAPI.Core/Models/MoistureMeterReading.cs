namespace MoistureMeterAPI.Core.Models
{
    public class MoistureMeterReading
    {
        public float Measure { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
