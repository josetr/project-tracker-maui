namespace ProjectTracker;

using Microsoft.EntityFrameworkCore;

public class EFOptions
{
    public static DbContextOptionsBuilder Get(DbContextOptionsBuilder optionBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("TASKTRACKER_SQL_CONNECTION_STRING");

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            optionBuilder.UseSqlServer(connectionString);
        }
        else
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(directory, "ProjectTracker.db");
            optionBuilder.UseSqlite($"Filename={path}");
        }

        return optionBuilder;
    }
}
