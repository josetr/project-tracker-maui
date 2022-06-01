namespace ProjectTracker;

using System.Linq;
using System.Threading.Tasks;

public sealed class ProjectService
{
    private readonly IUnitOfWork dbContext;
    private readonly TaskEvents events;

    public ProjectService(IUnitOfWork dbContext, TaskEvents events)
    {
        this.dbContext = dbContext;
        this.events = events;
    }

    public Task InitializeAsync()
    {
        return dbContext.EnsureCreatedAsync();
    }

    public async Task<Project[]> GetProjectsAsync()
    {
        return (await dbContext.Projects.GetAllAsync()).ToArray();
    }

    public async Task AddProjectAsync(Project project)
    {
        await dbContext.Projects.AddAsync(project);
    }

    public async Task RemoveProjectAsync(string id)
    {
        if (!await ProjectExistsAsync(id))
            return;

        await dbContext.Projects.RemoveAsync(id);
    }

    public async Task<bool> ProjectExistsAsync(string id)
    {
        return await dbContext.Projects.GetAsync(id) != null;
    }

    public async Task UpdateProjectAsync(Project project, string initialId)
    {
        var newId = project.Id;
        if (initialId == project.Id)
        {
            await dbContext.Projects.UpdateAsync(project);
            return;
        }

        project.Id = initialId;
        await dbContext.Projects.RemoveAsync(project.Id);
        project.Id = newId;
        await dbContext.Projects.AddAsync(project);
    }
}
