using Microsoft.Extensions.Options;
using MoistureMeterAPI.Core.Models;
using MoistureMeterAPI.Core.Options;
using MoistureMeterAPI.Core.Services.Interfaces;
using MQTTnet;
using MQTTnet.Formatter;
using Newtonsoft.Json;
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
        IMoistureMeterService _moistureMeterService;

        public MqttBrokerService(ILogger<MqttBrokerService> logger, IOptions<MqttOptions> options, IMoistureMeterService moistureMeterService)
        {
            _logger = logger;
            _moistureMeterService = moistureMeterService;

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

                MoistureMeterPayload? moistureMeterPayload = JsonConvert.DeserializeObject<MoistureMeterPayload>(message);
                if (moistureMeterPayload != null)
                {
                    try
                    {
                        _logger.LogInformation($"Received moisture meter reading: {moistureMeterPayload.Value.Measure}");
                
                        var timestamp = DateTimeOffset.FromUnixTimeSeconds(moistureMeterPayload.Value.TS).UtcDateTime;

                        var moistureMeterReading = new MoistureMeterReading
                        {
                            Timestamp = timestamp,
                            Measure = moistureMeterPayload.Value.Measure
                        };


                        await _moistureMeterService.Insert(moistureMeterReading);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error logging moisture meter reading");
                    }
                }


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

    public struct MoistureMeterPayload
    {
        public int TS { get; set; }
        public float Measure { get; set; }
    }
}
