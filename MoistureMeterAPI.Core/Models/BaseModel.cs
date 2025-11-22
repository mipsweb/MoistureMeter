using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace MoistureMeterAPI.Core.Models
{
    public class BaseModel
    {
        [BsonId]
        [JsonIgnore]
        public ObjectId Id { get; set; }
    }
}
