using CAT.EF;
using CAT.Events;
using CAT.Filters;
using CAT.Services;
using CAT.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var basePath = AppContext.BaseDirectory;

    var xmlPath = Path.Combine(basePath, "CATAPI.xml");
    options.IncludeXmlComments(xmlPath);
});
builder.Services.AddCors();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<YandexS3Service>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<ICSVService, CSVService>();
builder.Services.AddScoped<IGroupService, GroupService>();

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IAuthService, CookiesAuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICSVService, CSVService>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddSingleton<CustomCookieAuthenticationEvents>();
builder.Services.AddScoped<OrgValidationFilter>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => 
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.SameSite = SameSiteMode.Strict;

        options.EventsType = typeof(CustomCookieAuthenticationEvents);
    });

builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("PostgresDB");
builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseNpgsql(connectionString)
               .UseLoggerFactory(LoggerFactory.Create(builder =>
                   builder.AddFilter(level => level >= LogLevel.Warning))));


var app = builder.Build();

app.UseCors(builder =>
     builder.WithOrigins("http://localhost:3000")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .WithExposedHeaders("Content-Disposition")
         .AllowCredentials()
);

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();