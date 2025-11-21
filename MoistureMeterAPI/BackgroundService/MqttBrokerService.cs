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
    /// <summary>
    /// Provides a background service that connects to an MQTT broker, subscribes to a specified topic, and processes
    /// incoming moisture meter readings.
    /// </summary>
    /// <remarks>MqttBrokerService manages the lifecycle of an MQTT client, including connecting to the
    /// broker, subscribing to topics, and handling incoming messages. Received messages are deserialized and stored
    /// using the provided moisture meter service. This service is intended to run as a hosted background service within
    /// an ASP.NET Core or .NET application. Ensure that valid MQTT configuration options are supplied when registering
    /// this service.</remarks>
    public class MqttBrokerService : Microsoft.Extensions.Hosting.BackgroundService
    {
        ILogger<MqttBrokerService> _logger;

        IMqttClient _mqttClient;
        MqttClientOptions _mqttClientOptions;
        MqttClientFactory _mqttFactory;
        MqttClientSubscribeOptions _mqttClientSubscribeOptions;
        IMoistureMeterService _moistureMeterService;

        /// <summary>
        /// Initializes a new instance of the MqttBrokerService class with the specified logger, MQTT options, and
        /// moisture meter service dependencies.
        /// </summary>
        /// <remarks>This constructor sets up the MQTT client and subscribes to the specified topic.
        /// Incoming MQTT messages are deserialized and processed as moisture meter readings, which are then stored
        /// using the provided moisture meter service. Ensure that the options parameter contains valid MQTT
        /// configuration values before instantiating this service.</remarks>
        /// <param name="logger">The logger used to record operational and error information for the MQTT broker service.</param>
        /// <param name="options">The configuration options for the MQTT broker, including server address, credentials, and topic filter.</param>
        /// <param name="moistureMeterService">The service used to store and manage moisture meter readings received from MQTT messages.</param>
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

        /// <summary>
        /// Executes the background service operation to connect to the MQTT broker and subscribe to the configured
        /// topics.
        /// </summary>
        /// <remarks>If the connection to the MQTT broker or the subscription to topics fails, the method
        /// logs the error and disconnects from the broker. This method is typically not called directly; it is invoked
        /// by the host when the background service starts.</remarks>
        /// <param name="stoppingToken">A cancellation token that can be used to signal the request to stop the background operation.</param>
        /// <returns>A task that represents the asynchronous execution of the background service.</returns>
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
