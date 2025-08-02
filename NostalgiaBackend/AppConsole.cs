using FeroxArchiver;
using HoverthArchiver;

namespace NostalgiaBackend
{
    public class AppConsole(ILogger<AppConsole> logger) : IHostedService
    {
        private readonly ILogger<AppConsole> _logger = logger;
        private readonly FeroxInput _feroxConsole = new();
        private readonly HoverthInput _hoverthConsole = new();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Console service started.");

            while (true)
            {
                Console.WriteLine("Enter command (type 'exit' to quit):");
                var input = Console.ReadLine();
                var inputArray = input.Split();
                var command = inputArray.FirstOrDefault().ToLowerInvariant();

                switch (command)
                {
                    case "exit":
                        _logger.LogInformation("Exiting console service.");
                        break;
                    case "ferox":
                        _logger.LogInformation("Handling Ferox command.");
                        _feroxConsole.HandleInput([.. inputArray.Skip(1)]);
                        break;
                    case "hoverth":
                        _logger.LogInformation("Handling Hoverth command.");
                        _hoverthConsole.HandleInput([.. inputArray.Skip(1)]);
                        break;
                    default:
                        _logger.LogWarning("Unknown command: {input}", input);
                        break;
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Console service stopped.");
            return Task.CompletedTask;
        }
    }
}
