namespace MoistureMeterAPI.Controllers.Models
{
    public class MoistureMeterReadingDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Gets or sets the measurement value associated with this instance.
        /// </summary>
        public float Measure { get; set; }
        
        /// <summary>
        /// Gets or sets the timestamp indicating when the event occurred.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }
    }
}
