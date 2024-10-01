using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace WSUSCommander;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IConfiguration Configuration { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Initialize Serilog first
        InitializeLogging();

        Log.Information("Application Starting Up");

        base.OnStartup(e);

        try
        {
            // Build the configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // Access settings
            var setting1 = Configuration["WSUSSettings:Setting1"];
            var setting2 = Configuration["WSUSSettings:Setting2"];

            Log.Information("Loaded WSUS Settings: Setting1={Setting1}, Setting2={Setting2}", setting1, setting2);

            // Display settings in a message box
            MessageBox.Show($"Setting1: {setting1}\nSetting2: {setting2}", "WSUS Settings");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error loading settings");
            MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Application Shutting Down");
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private void InitializeLogging()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        try
        {
            Log.Information("Logger initialized");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing logger: {ex.Message}", "Logging Error", MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }
}
