﻿using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.Configuration;
using WSUSCommander.Extensions;

namespace WSUSCommander;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IConfiguration? Configuration { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            // Build the configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // Initialize Logger
            CLogger.Initialize(Configuration);
            CLogger.Info("Logger initialized");

            // Access settings
            var setting1 = Configuration["WSUSSettings:Setting1"];
            var setting2 = Configuration["WSUSSettings:Setting2"];

            CLogger.Info($"Loaded WSUS Settings: Setting1={setting1}, Setting2={setting2}");

            // Display settings in a message box
            MessageBox.Show($"Setting1: {setting1}\nSetting2: {setting2}", "WSUS Settings");
        }
        catch (Exception ex)
        {
            // Since Logger might not be initialized, use MessageBox
            MessageBox.Show($"Error during startup: {ex.Message}\n{ex.StackTrace}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(); // Ensure the application shuts down if startup fails
        }

        base.OnStartup(e);

        // Store arguments in dictionary
        Dictionary<string, string> arguments = new Dictionary<string, string>();

        // Get cmdline arguments
        string[] args = Environment.GetCommandLineArgs();
        for (int index = 1; index < args.Length; index += 2)
        {
            string arg = args[index].Replace("--", "");
            if (index + 1 < args.Length)
            {
                arguments.Add(arg, args[index + 1]);
            }
        }

        // Store the arguments globally
        Application.Current.Properties["Arguments"] = arguments;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        CLogger.Info("WSUS Command has been shutdown.");
        base.OnExit(e);
    }
}
