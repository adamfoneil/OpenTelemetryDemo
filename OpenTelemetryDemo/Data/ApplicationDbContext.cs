using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OpenTelemetryDemo.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<ToDo> ToDos { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
	}
}

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	private static IConfiguration Config => new ConfigurationBuilder()
		.AddJsonFile("appsettings.json", optional: false)
		.Build();

	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		var connectionString = Config.GetConnectionString("Default") ?? throw new Exception("Connection string 'Default' not found");
		optionsBuilder.UseSqlServer(connectionString);

		return new ApplicationDbContext(optionsBuilder.Options);
	}
}