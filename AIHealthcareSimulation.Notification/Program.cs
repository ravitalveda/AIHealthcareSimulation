using MassTransit;
using Microsoft.Extensions.Hosting;
using AIHealthcareSimulation.Notification;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TrainingNotificationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost");

        cfg.ReceiveEndpoint("simulation-training-feedback", e =>
        {
            e.ConfigureConsumer<TrainingNotificationConsumer>(context);
        });
    });
});

var app = builder.Build();

Console.WriteLine("Simulation Training Feedback Service started...");

await app.RunAsync();