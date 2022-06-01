using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProjectTracker;
using ProjectTracker.Blazor.WASM;
using TG.Blazor.IndexedDB;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
CommonStartup.AddServices(builder.Services);
builder.Services.AddIndexedDB(dbStore =>
{
    dbStore.DbName = "ProjectTracker";
    dbStore.Version = 5;

    dbStore.Stores.Add(new StoreSchema
    {
        Name = nameof(Project),
        PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id" }
    });

    dbStore.Stores.Add(new StoreSchema
    {
        Name = nameof(ProjectTask),
        PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = true }
    });

    dbStore.Stores.Add(new StoreSchema
    {
        Name = nameof(TaskTimeEntry),
        PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = true }
    });
});

builder.Services.AddScoped<IUnitOfWork, IndexedDbUnitOfWork>();
builder.Services.AddSingleton<INotificationManager, EmptyNotificationManager>();

await builder.Build().RunAsync();
