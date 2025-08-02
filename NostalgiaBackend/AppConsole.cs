using FeroxArchiver;
using HoverthArchiver;

namespace NostalgiaBackend
{
    public class AppConsole(ILogger<AppConsole> logger, FeroxInput feroxConsole, HoverthInput hoverthConsole, IHostApplicationLifetime hostApplicationLifetime) : IHostedService
    {
        private readonly ILogger<AppConsole> _logger = logger;
        private readonly FeroxInput _feroxConsole = feroxConsole;
        private readonly HoverthInput _hoverthConsole = hoverthConsole;
        private readonly IHostApplicationLifetime _hostApplicationLifetime = hostApplicationLifetime;
        private Task _consoleTask;
        private CancellationTokenSource _cancellationTokenSource;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Console service started.");
            _cancellationTokenSource = new CancellationTokenSource();

            _consoleTask = Task.Run(async () => {
                try
                {
                    await RunConsoleAsync(_cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception in console task");
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        private async Task RunConsoleAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("=== Nostalgia Backend Console ===");
            ShowHelp();
            Console.WriteLine("=====================================");
            Console.WriteLine("Enter command (type 'exit' to quit):");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var input = await Task.Run(Console.ReadLine, cancellationToken);

                    if (string.IsNullOrEmpty(input) || cancellationToken.IsCancellationRequested)
                        break;

                    var inputArray = input.Split();
                    var command = inputArray.FirstOrDefault()?.ToLowerInvariant();

                    switch (command)
                    {
                        case "exit":
                            _logger.LogInformation("Exiting console service.");
                            _hostApplicationLifetime.StopApplication();
                            return;
                        case "help":
                            ShowHelp();
                            break;
                        case "ferox":
                            try
                            {
                                _logger.LogInformation("Handling Ferox command.");
                                _feroxConsole.HandleInput([.. inputArray.Skip(1)]);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error executing Ferox command");
                                Console.WriteLine($"Error executing Ferox command: {ex.Message}");
                            }
                            break;
                        case "hoverth":
                            try
                            {
                                _logger.LogInformation("Handling Hoverth command.");
                                _hoverthConsole.HandleInput([.. inputArray.Skip(1)]);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error executing Hoverth command");
                                Console.WriteLine($"Error executing Hoverth command: {ex.Message}");
                            }
                            break;
                        default:
                            _logger.LogWarning("Unknown command: {input}", input);
                            Console.WriteLine($"Unknown command: {input}");
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Console operation was cancelled.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing console input");
                    Console.WriteLine($"Error processing input: {ex.Message}");
                }

                Console.WriteLine("Enter command (type 'exit' to quit):");
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("  ferox <args>   - Execute Ferox archiver commands");
            Console.WriteLine("  hoverth <args> - Execute Hoverth archiver commands");
            Console.WriteLine("  help           - Show this help message");
            Console.WriteLine("  exit           - Stop the console service");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Console service stopping.");

            try
            {
                _cancellationTokenSource?.Cancel();

                if (_consoleTask != null)
                {
                    await _consoleTask;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping console service");
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _logger.LogInformation("Console service stopped.");
            }
        }
    }
}
