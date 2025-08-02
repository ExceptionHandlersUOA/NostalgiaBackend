using HoverthArchiver;
using Microsoft.EntityFrameworkCore;
using NostalgiaBackend.Services;
using Shared.Database;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var assemblies = AppDomain.CurrentDomain.GetAssemblies();

builder.Services.AddDbContext<PostContext>();
builder.Services.AddSingleton<HoverthInput>();
builder.Services.AddHostedService<DbInitializer>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<PostContext>();
    context.Database.Migrate();
}

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
