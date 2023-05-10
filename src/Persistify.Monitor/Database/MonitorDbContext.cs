using Microsoft.EntityFrameworkCore;
using Persistify.Monitor.Database.Domain;

namespace Persistify.Monitor.Database;

public class MonitorDbContext : DbContext
{
    public MonitorDbContext(DbContextOptions<MonitorDbContext> options) : base(options)
    {
    }

    public DbSet<PipelineEvent> PipelineEvents { get; set; } = default!;
}