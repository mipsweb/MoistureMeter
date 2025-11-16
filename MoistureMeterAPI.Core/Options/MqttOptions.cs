namespace MoistureMeterAPI.Core.Options
{
    public class MqttOptions
    {
        public const string MqttOption = "MqttOption";

        public string TcpServer { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TopicFilter { get; set; }
    }
}
