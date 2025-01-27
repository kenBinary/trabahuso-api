using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using trabahuso_api.Helpers;
using trabahuso_api.Interfaces;
using trabahuso_api.Middlewares;
using trabahuso_api.Models;
using trabahuso_api.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<GlobalErrorHandler>();

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
});

string? origin = builder.Configuration.GetValue<string>("Origin");
builder.Services.Configure<TursoDatabaseSettings>(
    builder.Configuration.GetSection(TursoDatabaseSettings.TursoDatabase));

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCorsPolicy", policy =>
    {
        policy.WithOrigins("*")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("ProductionCorsPolicy", policy =>
    {
        policy.WithOrigins(origin ?? "")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<ITechSkillRepository, TechSkillRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ISalaryRepository, SalaryRepository>();
builder.Services.AddScoped<ISqliteQueryCompiler, SqliteQueryCompiler>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("fixed-by-ip", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));
});

var app = builder.Build();

app.UseMiddleware<GlobalErrorHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevelopmentCorsPolicy");

}
else if (app.Environment.IsStaging())
{
    app.UseHsts();
    app.UseRateLimiter();
    Console.WriteLine("Application starting in Staging mode");
}
else
{
    var appUrl = app.Configuration.GetValue<string>("ASPNETCORE_URLS");
    app.UseHsts();
    app.UseRateLimiter();
    app.UseCors("DevelopmentCorsPolicy");
    Console.WriteLine("Application starting in Production mode");
    Console.WriteLine($"Application running on the url: {appUrl}");
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    await next.Invoke();
});

app.MapControllers().RequireRateLimiting("fixed-by-ip");
app.UseStatusCodePages();
app.UseRouteLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.Run();
