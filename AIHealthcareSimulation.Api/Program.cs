using AIHealthcareSimulation.Api.Consumers;
using AIHealthcareSimulation.Api.Endpoints;
using AIHealthcareSimulation.Api.Hubs;
using MassTransit;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(
    options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
        options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "Frontend",
        policy => policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TrainingDebriefForwarder>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"] ?? "localhost");

        cfg.ReceiveEndpoint("api-training-debriefs", endpoint =>
        {
            endpoint.ConfigureConsumer<TrainingDebriefForwarder>(context);
        });
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapScenarioEndpoints();
app.MapSimulationEndpoints();
app.MapHub<TrainingResultsHub>("/hubs/training-results");

app.Run();
