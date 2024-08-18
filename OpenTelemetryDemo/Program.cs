using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryDemo.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddOpenTelemetry().WithTracing(trace =>
	trace.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BlazorApp"))
	.AddAspNetCoreInstrumentation()
	.AddHttpClientInstrumentation()
	.AddEntityFrameworkCoreInstrumentation()
	.AddOtlpExporter(options => options.Endpoint = new Uri("http://jaeger:4317")));

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
