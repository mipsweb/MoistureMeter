[![Docker Image CI](https://github.com/mipsweb/MoistureMeter/actions/workflows/docker-image.yml/badge.svg)](https://github.com/mipsweb/MoistureMeter/actions/workflows/docker-image.yml)

# MoistureMeterAPI

MoistureMeterAPI is a small .NET 10 web service that listens for MQTT messages containing moisture sensor readings, persists them to MongoDB, and exposes an ASP.NET Core API surface (OpenAPI/Swagger enabled in development). It includes a background MQTT client, a domain service layer, and a MongoDB repository.

## Key components
- `MoistureMeterAPI.BackgroundService.MqttBrokerService` — background worker that connects to an MQTT broker, subscribes to a topic and converts incoming messages into domain readings.
- `MoistureMeterAPI.Core.Services.MoistureMeterService` — business/service layer that coordinates persistence.
- `MoistureMeterAPI.Core.Repository.MoistureMeterRepository` — MongoDB-backed repository writing to the `reading` collection.
- `MoistureMeterAPI.Core.Models.MoistureMeterReading` — model persisted to MongoDB (`Measure: float`, `Timestamp: DateTimeOffset`).
- Serilog configured for file logging (`logs/log-.json`) and OpenAPI enabled in development.

## Configuration
Primary configuration lives in `appsettings.json` (and `appsettings.Development.json` for dev overrides). Relevant sections:
- `MqttOption` — `TcpServer`, `Username`, `Password`, `TopicFilter`.
- `MongoDbOption` — `Server`, `DatabaseName`, `Username`, `Password`.
- `Serilog` — logging sinks and levels.

The repository builds the MongoDB connection string via `MongoDBOptions.GetConnectionString()` which produces:
`mongodb://{Username}:{Password}@{Server}/`

Note: include host:port in `Server` if your MongoDB is not on the default port (e.g. `dbhost:27017`).

## Expected MQTT payload
Incoming MQTT messages are JSON deserialized into the shape:
{
  "TS": 1700000000,    // Unix epoch seconds
  "Measure": 12.34     // float moisture value
}

The service converts `TS` to a `DateTimeOffset` and stores the measurement.

## Build & run
Requirements: .NET 10 SDK.

From the repository root:
- Build: `dotnet build`
- Run: `dotnet run --project MoistureMeterAPI`

In Visual Studio, open the solution and use __Debug > Start Debugging__ or __Debug > Start Without Debugging__.

## Docker
The project includes a `Dockerfile` for building and running the service.
Build and run (example):
- Build: `docker build -t moisturemeterapi .`
- Run: `docker run -e ASPNETCORE_ENVIRONMENT=Production -p 8080:8080 moisturemeterapi`

Ensure container environment or mounted `appsettings.json` provide valid MQTT and MongoDB connection info.

## Development notes
- OpenAPI is mapped only when `IWebHostEnvironment.IsDevelopment()` is true.
- Serilog reads configuration from `appsettings.json` and writes JSON logs to `logs/log-.json`.
- The MongoDB collection used is named `reading` and `AssignIdOnInsert = true` is set for inserts.

## Security
- Do not commit production credentials. Prefer environment variables or secret stores for production credentials.
- Validate MQTT broker access and MongoDB authentication before deploying.

## Troubleshooting
- If MQTT connection fails, check `MqttOption.TcpServer`, credentials and network reachability.
- If MongoDB writes fail, confirm `MongoDbOption.Server`, credentials and firewall/port settings.
