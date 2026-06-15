using BlazorApp.Components;
using BlazorApp.Data;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using BlazorApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMudServices()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

//identity and security services
builder.Services.AddIdentityCore<User>(options => {
    //password rules (optional)
    })
    .AddRoles<IdentityRole<int>>() //enables rolebased authorisation
    .AddEntityFrameworkStores<ApplicationDbContext>() //save data via db context
    .AddSignInManager() //managing user login sessions
    .AddDefaultTokenProviders(); //tokens for password reset or email confirmations

builder.Services.AddCascadingAuthenticationState(); //cascading state tracking

builder.Services.AddAuthentication(options => {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("PostgreSQLLocalHost");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope()) {
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var roles = configuration.GetSection("Roles").Get<string[]>();

    if (roles is not null) {
        foreach (var role in roles) {
            if (!await roleManager.RoleExistsAsync(role)) {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }
    }
}

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
