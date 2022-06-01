namespace ProjectTracker;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class TaskTrackerDbContext : DbContext
{
    public TaskTrackerDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; } = default!;
    public DbSet<ProjectTask> Tasks { get; set; } = default!;
    public DbSet<TaskTimeEntry> TaskEntries { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>();
        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.Property(e => e.Duration).HasConversion(new TimeSpanToTicksConverter());
        });
    }
}
