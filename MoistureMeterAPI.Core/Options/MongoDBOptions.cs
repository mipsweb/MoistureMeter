namespace MoistureMeterAPI.Core.Options
{
    public class MongoDBOptions
    {
        public const string MongoDbOption = "MongoDbOption";

        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string GetConnectionString()
        {
            return $"mongodb://{Username}:{Password}@{Server}/";
        }
    }
}
