namespace OpenTelemetryDemo.Data;

public class ToDo
{
	public int Id { get; set; }
	public DateTime DateCreated { get; set; }
	public string Description { get; set; } = default!;
	public bool IsComplete { get; set; }
}
