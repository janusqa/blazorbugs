using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DemoAuth.Client.Pages;
using DemoAuth.Components;
using DemoAuth.Components.Account;
using DemoAuth.Data;
using DemoAuth.Models;
using DemoAuth.Data.UnitOfWork;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = (context) =>
    {
        // this is a hack to dectect if request is a regualar request or an api/ajax request;

        if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("/api/"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }

        return Task.CompletedTask;
    };
});

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorizationBuilder();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var serverUrl = builder.Configuration.GetSection("AppUrls")["BaseServerUrl"] ?? throw new InvalidOperationException("Base Server Url 'BaseServerurl' not found.");
builder.Services.AddCors(options =>
{
    options.AddPolicy("WasmClient", builder =>
        builder
            .WithOrigins(serverUrl, "http://localhost:5264")
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS")
            .WithHeaders("Origin", "X-Requested-With", "Content-Type", "Authorization", "X-Xsrf-Token",
                "X-Forwarded-For", "X-Real-IP")
            .AllowCredentials()
    );
});

builder.Services.AddHttpClient(
    "DemoAuth",
    client => client.BaseAddress = new Uri(builder.Configuration.GetSection("AppUrls")["BaseApiUrl"]!)
)
.ConfigurePrimaryHttpMessageHandler(() =>
{
    // !!! DISABLE IN PROD. THIS IS TO BYPASS CHECKING SSL CERT AUTH FOR DEV PURPOSES !!!
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
DemoAuth.Client.CommonServices.ConfigureCommonServices(builder.Services, builder.Configuration);

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
    builder.Configuration.GetSection("SyncFusion")["ApiKey"] ?? throw new InvalidOperationException("SyncFusion Licences Key 'ApiKey' not found."));
builder.Services.AddSyncfusionBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("WasmClient");

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(DemoAuth.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
