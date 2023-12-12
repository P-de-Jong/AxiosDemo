// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string API_KEY = "";
const string DATA_SET = "";

// Create a tracer provider with the OTLP exporter
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("my-service-name"))
    .AddSource("my-source-name")
    .AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
        otlpOptions.Endpoint = new Uri("https://api.axiom.co/v1/traces");
        otlpOptions.Headers = $"Authorization=Bearer {API_KEY},X-Axiom-Dataset={DATA_SET}";
    })
    .Build();

// Get a tracer
var tracer = tracerProvider.GetTracer("my-source-name");

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddOpenTelemetry(logging =>
    {
        logging
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("my-service-name"))
            .AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                otlpOptions.Endpoint = new Uri("https://api.axiom.co/v1/logs");
                otlpOptions.Headers = $"Authorization=Bearer {API_KEY},X-Axiom-Dataset={DATA_SET}";
            });
    });
});

var logger = loggerFactory.CreateLogger<Program>();

logger.LogInformation("Hello World!");

// Start a span and do some work
using (var span = tracer.StartActiveSpan("my-span-name"))
{
    try
    {
        // Your work here
        // Simulating work with a delay
        Thread.Sleep(1000);

        // Optionally, you can add events or attributes onto your span
        span.AddEvent("An event in my span");
    }
    catch (Exception ex)
    {
        // If there's an error, record the exception into the span
        span.RecordException(ex);
        throw; // rethrow the exception
    }
    finally
    {
        // End the span
        span.End();
    }
}

Console.ReadLine();
