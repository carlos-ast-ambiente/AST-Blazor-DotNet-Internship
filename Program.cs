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
        // options.Password.RequireDigit = false;
        // options.Password.RequiredLength = 3;
        // options.Password.RequireNonAlphanumeric = false;
        // options.Password.RequireUppercase = false;
        // options.Password.RequireLowercase = false;
    })
    .AddRoles<IdentityRole<int>>() //enables rolebased authorisation
    .AddEntityFrameworkStores<ApplicationDbContext>() //save data via db context
    .AddSignInManager() //managing user login sessions
    .AddDefaultTokenProviders(); //tokens for password reset or email confirmations

builder.Services.AddCascadingAuthenticationState(); //cascading state tracking

builder.Services.AddAuthentication(options => {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorization();

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

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapPost("/account/login", async (
    HttpContext httpContext,
    SignInManager<User> signInManager) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    Console.WriteLine($"Login attempt for: '{username}'"); 

    var result = await signInManager.PasswordSignInAsync(
        username,
        password,
        isPersistent: false,
        lockoutOnFailure: false);

    if (result.Succeeded) {
        return Results.Redirect("/");
    }

    return Results.Redirect("/login?error=1");
});


app.MapPost("/account/logout", async(SignInManager<User> signInManager) => 
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
