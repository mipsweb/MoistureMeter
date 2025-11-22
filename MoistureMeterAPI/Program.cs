using MoistureMeterAPI.BackgroundService;
using MoistureMeterAPI.Core.Options;
using MoistureMeterAPI.Core.Repository;
using MoistureMeterAPI.Core.Repository.Interfaces;
using MoistureMeterAPI.Core.Services;
using MoistureMeterAPI.Core.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection(MqttOptions.MqttOption));
builder.Services.Configure<MongoDBOptions>(builder.Configuration.GetSection(MongoDBOptions.MongoDbOption));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHostedService<MqttBrokerService>();
builder.Services.AddTransient<IDBContext, DBContext>();
builder.Services.AddTransient<IMoistureMeterRepository, MoistureMeterRepository>();
builder.Services.AddTransient<IMoistureMeterService, MoistureMeterService>();

var app = builder.Build();
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
