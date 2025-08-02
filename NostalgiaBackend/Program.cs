using NostalgiaBackend;
using Shared;

_ = new FeroxArchiver.Load();
_ = new HoverthArchiver.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<AppConsole>();

var assemblies = AppDomain.CurrentDomain.GetAssemblies();

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization(); 

app.MapControllers();

app.Run();
