using ProjectTracker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<TaskTrackerDbContext>(options => EFOptions.Get(options));
builder.Services.AddSingleton<INotificationManager, EmptyNotificationManager>();
builder.Services.AddScoped<IUnitOfWork, EFUnitOfWork>();
CommonStartup.AddServices(builder.Services);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
