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
builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

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

app.Run();
