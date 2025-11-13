using Microsoft.Extensions.Options;
using MoistureMeterAPI.Options;
using MQTTnet;
using MQTTnet.Formatter;
using System.Text;

namespace MoistureMeterAPI.BackgroundService
{
    public class MqttBrokerService : Microsoft.Extensions.Hosting.BackgroundService
    {
        ILogger<MqttBrokerService> _logger;

        IMqttClient _mqttClient;
        MqttClientOptions _mqttClientOptions;
        MqttClientFactory _mqttFactory;
        MqttClientSubscribeOptions _mqttClientSubscribeOptions;

        public MqttBrokerService(ILogger<MqttBrokerService> logger, IOptions<MqttOptions> options)
        {
            _logger = logger;

            _mqttFactory = new MqttClientFactory();

            _mqttClientOptions = _mqttFactory.CreateClientOptionsBuilder()
            .WithTcpServer(options.Value.TcpServer)
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .WithCredentials(options.Value.Username, options.Value.Password)
            .Build();

            _mqttClient = _mqttFactory.CreateMqttClient();

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                _logger.LogInformation(message);

                await Task.CompletedTask;
            };

            _mqttClientSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter(options.Value.TopicFilter).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var connectResult = await _mqttClient.ConnectAsync(_mqttClientOptions, stoppingToken);
                if (connectResult == null || connectResult.ResultCode != MqttClientConnectResultCode.Success)
                {
                    throw new Exception($"Unable to connect to MQTT server. {connectResult?.ResultCode}");
                }

                var subscribeResult = await _mqttClient.SubscribeAsync(_mqttClientSubscribeOptions, stoppingToken);
                if (subscribeResult == null || subscribeResult.Items.Count == 0)
                {
                    throw new Exception("Unable to subscribe to MQTT topic.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while connecting to the MQTT broker.");
                await _mqttClient.DisconnectAsync();
            }
        }
    }
}
