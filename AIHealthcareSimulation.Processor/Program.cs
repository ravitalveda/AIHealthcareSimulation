using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AIHealthcareSimulation.Processor;

var builder = Host.CreateApplicationBuilder(args);

ConfigureMessaging(builder.Services);

var app = builder.Build();

Console.WriteLine("=================================================");
Console.WriteLine("Hospital Simulation Training Processor");
Console.WriteLine("Queue : simulation-training-assessment");
Console.WriteLine("Broker: RabbitMQ");
Console.WriteLine("=================================================");

await app.RunAsync();

static void ConfigureMessaging(IServiceCollection services)
{
    services.AddMassTransit(x =>
    {
        x.AddConsumer<TrainingAssessmentConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("localhost");

            cfg.ReceiveEndpoint("simulation-training-assessment", endpoint =>
            {
                endpoint.ConfigureConsumer<TrainingAssessmentConsumer>(context);

                endpoint.UseMessageRetry(r =>
                    r.Interval(3, TimeSpan.FromSeconds(5)));
            });
        });
    });
}