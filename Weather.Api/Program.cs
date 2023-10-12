using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using Weather.Api.Authorization;
using Weather.Api.Configurations;
using Weather.Api.Data;
using Weather.Api.IdentityRoleServices;
using Weather.Api.Services;
using Weather.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Configuration.AddJsonFile("appsettings.json");
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Services.Configure<ConnectionStringOptions>(builder.Configuration.GetSection(ConnectionStringOptions.Position));
builder.Services.AddTransient<IDapperContext, DapperContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpClient<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<WeatherApiSettings>(builder.Configuration.GetSection("WeatherApiSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, MustSeeWeatherHandler>();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //.AddJwtBearer(options =>
    //{
    //    options.Authority = "https://localhost:5001";
    //    options.Audience = "weatherapi";
    //    options.TokenValidationParameters = new ()
    //    {
    //        NameClaimType = "given_name",
    //        RoleClaimType = "role",
    //        ValidTypes = new[] { "at+jwt" }
    //    };
    //});
    .AddOAuth2Introspection(options =>
    {
        options.Authority = "https://localhost:5001";
        options.ClientId = "weatherapi";
        options.ClientSecret = "apisecret";
        options.NameClaimType = "given_name";
        options.RoleClaimType = "role";
    });
builder.Services.AddAuthorization(authorizationOptions =>
{
    authorizationOptions.AddPolicy("UserCanSeeWeather",
        AuthorizationPolicies.CanSeeWeather());
    authorizationOptions.AddPolicy("ClientApplicationCanRead", policyBuilder =>
    {
        policyBuilder.RequireClaim("scope", "weatherapi.read");
    });
    authorizationOptions.AddPolicy("MustSeeWeather", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.AddRequirements(new MustSeeWeatherRequirement());
    });
});

//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddRoleManager<RoleManager<IdentityRole>>(); // Add this line to configure RoleManager

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
IdentityModelEventSource.ShowPII = true;
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
