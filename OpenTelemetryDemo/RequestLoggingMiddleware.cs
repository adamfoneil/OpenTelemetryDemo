namespace OpenTelemetryDemo;

public class RequestLoggingMiddleware(RequestDelegate next)
{
	private readonly RequestDelegate _next = next;

	public async Task Invoke(HttpContext context)
	{
		context.Request.EnableBuffering();

		using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
		
		var body = await reader.ReadToEndAsync();
		context.Request.Body.Position = 0; // Rewind the body stream for next middleware
			
		context.Items["RequestBody"] = body;			
		var activity = System.Diagnostics.Activity.Current;
		activity?.SetTag("http.request.body", body);	

		await _next(context);
	}
}
