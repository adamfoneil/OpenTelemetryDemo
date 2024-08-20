using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryDemo.Components;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddOpenTelemetry().ConfigureResource(resource =>
	resource.AddService("BlazorApp").AddAttributes(new Dictionary<string, object>()
	{
		["service.version"] = Assembly.GetExecutingAssembly().GetName().Version!.ToString(),
		["environment"] = builder.Environment.EnvironmentName
	})).WithTracing(trace =>
		trace
			.AddAspNetCoreInstrumentation()
			.AddHttpClientInstrumentation()
			.AddEntityFrameworkCoreInstrumentation()
			.AddConsoleExporter()
			.AddOtlpExporter(options =>
			{
				options.Endpoint = new Uri("http://jaeger:4317");
				options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;				
			}));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
