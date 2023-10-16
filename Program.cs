// See https://aka.ms/new-console-template for more information

using OpenTelemetry;
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
        otlpOptions.Endpoint = new Uri("https://api.axiom.co");
        otlpOptions.Headers = $"Authorization=Bearer {API_KEY},X-Axiom-Dataset={DATA_SET}";
    })
    .Build();

// Get a tracer
var tracer = tracerProvider.GetTracer("my-source-name");

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
