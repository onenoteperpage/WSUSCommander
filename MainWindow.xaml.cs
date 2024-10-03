using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WSUSCommander.Extensions;
using WSUSCommander.Windows;

namespace WSUSCommander;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static bool IsAdmin { get; set; } = false;  // Static value for the admin group
    public string LogFilePath { get; set; } = Application.Current.Properties["LogFileName"]?.ToString() ?? System.IO.Path.Combine("Logs", $"{DateTime.Now.ToString("yyyyMMdd")}-appsettings-json-missing-wsuscommander.log");
    public List<string> ServerGroups { get; set; }
    public string CustomText { get; set; } = Application.Current.Properties["LoggedOnUsersMessage"]?.ToString() ?? "Replace this text with a message to send any users that are logged on to target machines.";

    public MainWindow()
    {
        InitializeComponent();

        // Check if the user is an admin using the UserCheck class
        IsAdmin = UserCheck.IsUserAdministrator();

        // execute command line arguments first, then exit?
        if (Application.Current.Properties.Contains("Arguments"))
        {
            if (Application.Current.Properties["Arguments"] is Dictionary<string, string> arguments)
            {
                if (arguments.ContainsKey("message"))
                {
                    txt_TitleText.Text = arguments["message"];
                }
            }
        }

        // Retrieve the dictionary of ServerGroups from Application.Current.Properties
        if (Application.Current.Properties["ServerGroups"] is Dictionary<string, List<string>> serverGroups)
        {
            // Get the keys from the dictionary and assign them to the ServerGroups property
            ServerGroups = serverGroups.Keys.ToList();
        }
        else
        {
            // Handle case where the ServerGroups key does not exist or is invalid
            ServerGroups = new List<string>() { "The", "servers.txt", "file", "is", "missing" }; // Initialize empty list or handle accordingly
        }

        // Set the data context to this window for easy binding
        DataContext = this;

        // log: app started
        CLogger.Info($"WSUS Commander started");
        

    }

    private async void StartAction_Click(object sender, RoutedEventArgs e)
    {
        // Create and show the progress window
        ProgressWindow progressWindow = new ProgressWindow();
        progressWindow.Show();

        // Simulate a long-running action with sub-activities
        int totalSubActivities = 10;

        for (int i = 0; i < totalSubActivities; i++)
        {
            // Simulate each sub-activity taking time (e.g., processing, loading, etc.)
            await Task.Delay(500); // Simulate time delay for a sub-task

            // Calculate percentage completion
            int percentage = ((i + 1) * 100) / totalSubActivities;

            // Update the progress bar
            progressWindow.UpdateProgress(percentage);
        }

        // Close the progress window once completed
        progressWindow.Close();
    }

    private void btn_ShowHelp_Click(object sender, RoutedEventArgs e)
    {
        HelpWindow helpWindow = new HelpWindow();
        helpWindow.Show();
    }

    private void btn_CloseApp_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void btn_ExecuteProcess_Click(object sender, RoutedEventArgs e)
    {

    }
}