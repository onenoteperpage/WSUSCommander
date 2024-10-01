using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.Configuration;

namespace WSUSCommander;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IConfiguration Configuration { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
{
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

        // Display settings in a message box
        MessageBox.Show($"Setting1: {setting1}\nSetting2: {setting2}", "WSUS Settings");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

}

