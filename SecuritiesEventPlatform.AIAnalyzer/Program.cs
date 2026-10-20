using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecuritiesEventPlatform.AIAnalyzer;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AIAssessmentConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost");

        cfg.ReceiveEndpoint("ai-risk-assessment", e =>
        {
            e.ConfigureConsumer<AIAssessmentConsumer>(context);

            e.UseMessageRetry(r =>
                r.Interval(3, TimeSpan.FromSeconds(5)));
        });
    });
});

var app = builder.Build();

Console.WriteLine("AI Risk Assessment Service started...");

await app.RunAsync();