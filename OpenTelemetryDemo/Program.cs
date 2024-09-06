using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryDemo;
using OpenTelemetryDemo.Components;
using OpenTelemetryDemo.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddControllers();

builder.Services.AddOpenTelemetry().ConfigureResource(resource =>
	resource.AddService("BlazorApp").AddAttributes(new Dictionary<string, object>()
	{
		["service.version"] = Assembly.GetExecutingAssembly().GetName().Version!.ToString(),
		["environment"] = builder.Environment.EnvironmentName
	})).WithTracing(trace => trace
		.AddAspNetCoreInstrumentation()
		.AddHttpClientInstrumentation()
		.AddEntityFrameworkCoreInstrumentation()
		.AddConsoleExporter()
		.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317")));

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(connectionString).EnableSensitiveDataLogging().EnableDetailedErrors());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.MapControllers();

// for debugging purposes
app.MapGet("/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
{
	var routes = endpointSources
		.SelectMany(source => source.Endpoints)
		.OfType<RouteEndpoint>()
		.Select(endpoint =>
		{
			var methods = endpoint.Metadata.OfType<HttpMethodMetadata>().SelectMany(m => m.HttpMethods) ?? [];
			return new
			{				
				Route = endpoint.RoutePattern.RawText,
				Methods = string.Join(", ", methods)
			};
		});

	return Results.Ok(routes);
});


app.Run();
