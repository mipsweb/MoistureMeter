using MoistureMeterAPI.BackgroundService;
using MoistureMeterAPI.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection(MqttOptions.MqttOption));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHostedService<MqttBrokerService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
