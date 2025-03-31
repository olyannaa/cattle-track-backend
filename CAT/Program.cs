using CAT.EF;
using CAT.EF.DAL;
using CAT.Services;
using CAT.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<YandexS3Service>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<ICSVService, CSVService>();

builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("PostgresDB");
builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.UseCors(builder =>
     builder.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod()
         .WithExposedHeaders("Content-Disposition")
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