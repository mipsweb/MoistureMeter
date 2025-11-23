namespace MoistureMeterAPI.Controllers.Models
{
    public class MoistureMeterPaginationResult
    {
        /// <summary>
        /// Gets or sets the collection of moisture meter readings returned by the operation.
        /// </summary>
        public IList<MoistureMeterReadingDto> Result { get; set; }
        
        /// <summary>
        /// Gets or sets the number of rows in the collection.
        /// </summary>
        public int Rows { get; set; }
    }
}
