using MassTransit;
using Microsoft.Extensions.Hosting;
using SecuritiesEventPlatform.Notification;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<IncidentNotificationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost");

        cfg.ReceiveEndpoint("incident-notification", e =>
        {
            e.ConfigureConsumer<IncidentNotificationConsumer>(context);
        });
    });
});

var app = builder.Build();

Console.WriteLine("Incident Notification Service started...");

await app.RunAsync();