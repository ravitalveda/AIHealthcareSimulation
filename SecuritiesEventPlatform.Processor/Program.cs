using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecuritiesEventPlatform.Processor;

var builder = Host.CreateApplicationBuilder(args);

ConfigureMessaging(builder.Services);

var app = builder.Build();

Console.WriteLine("=================================================");
Console.WriteLine("Securities Event Processing Service");
Console.WriteLine("Queue : incident-processing");
Console.WriteLine("Broker: RabbitMQ");
Console.WriteLine("=================================================");

await app.RunAsync();

static void ConfigureMessaging(IServiceCollection services)
{
    services.AddMassTransit(x =>
    {
        x.AddConsumer<IncidentProcessingConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("localhost");

            cfg.ReceiveEndpoint("incident-processing", endpoint =>
            {
                endpoint.ConfigureConsumer<IncidentProcessingConsumer>(context);

                endpoint.UseMessageRetry(r =>
                    r.Interval(3, TimeSpan.FromSeconds(5)));
            });
        });
    });
}