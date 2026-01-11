using Foodie.Business.Authorization.Handlers;
using Foodie.Business.Authorization.Requirements;
using Foodie.Business.Repositories.Implementations;
using Foodie.Business.Repositories.Interfaces;
using Foodie.Business.Services.Implementations;
using Foodie.Business.Services.Interfaces;
using Foodie.Data.Persistance;
using Foodie.Data.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IRestaurantService, RestaurantService>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<IMenuService, MenuService>();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RestaurantAccessPolicy", policy =>
    {
        policy.Requirements.Add(new RestaurantManagementAccessRequirement());
    });
builder.Services.AddScoped<IAuthorizationHandler, RestaurantManagementAccessHandler>();

var app = builder.Build();
// Seed data
using IServiceScope scope = app.Services.CreateScope();
IServiceProvider serviceProvider = scope.ServiceProvider;

await DatabaseSeeder.SeedAsync(serviceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
