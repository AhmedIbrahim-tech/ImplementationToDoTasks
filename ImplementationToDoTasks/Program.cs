using ImplementationToDoTasks.Data;
using ImplementationToDoTasks.Middleware;
using ImplementationToDoTasks.Persistence;
using ImplementationToDoTasks.Repository;
using ImplementationToDoTasks.Services;
using ImplementationToDoTasks.Services.CreateAccount;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the DbContext, UnitOfWork, and repositories
builder.Services.AddScoped<AppDbContext>(provider => 
new AppDbContext(builder.Configuration.GetConnectionString("DefaultLiveConnection")));


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ClientRepository, ClientRepository>();

builder.Services.AddScoped<ICreateAccountClientServices, CreateAccountClientServices>();
builder.Services.AddScoped<INameReplacerService,NameReplacerService>();
builder.Services.AddScoped<IReplaceFileRenameServices, ReplaceFileRenameServices>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


// Use custom error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<LoadingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
