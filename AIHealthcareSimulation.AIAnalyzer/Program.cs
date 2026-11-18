using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AIHealthcareSimulation.AIAnalyzer;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TrainingDebriefConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost");

        cfg.ReceiveEndpoint("ai-training-debrief", e =>
        {
            e.ConfigureConsumer<TrainingDebriefConsumer>(context);

            e.UseMessageRetry(r =>
                r.Interval(3, TimeSpan.FromSeconds(5)));
        });
    });
});

var app = builder.Build();

Console.WriteLine("AI Training Debrief Service started...");

await app.RunAsync();