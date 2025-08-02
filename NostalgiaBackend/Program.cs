using Microsoft.EntityFrameworkCore;
using NostalgiaBackend;
using NostalgiaBackend.Services;
using Shared;
using Shared.Database;
using System.Text.Json.Serialization;

_ = new FeroxArchiver.Load();
_ = new HoverthArchiver.Load();

var builder = WebApplication.CreateBuilder(args);

var assemblies = AppDomain.CurrentDomain.GetAssemblies();

builder.Services.AddHostedService<AppConsole>();
builder.Services.AddDbContext<PostContext>();

foreach (var assembly in assemblies)
{
    var sharedTypes = assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && t.Namespace != null && (t.Namespace.Contains("FeroxArchiver") || t.Namespace.Contains("HoverthArchiver")));

    foreach (var type in sharedTypes)
    {
        if (typeof(IConsoleApplication).IsAssignableFrom(type))
            builder.Services.AddSingleton(type);
    }
}

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<PostContext>();
    context.Database.Migrate();
    DbInitializer.Initialize(context);
}

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
