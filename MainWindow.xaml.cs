﻿using System.Diagnostics;
using System.Security.RightsManagement;
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
using WSUSCommander.Models;
using WSUSCommander.Windows;

namespace WSUSCommander;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static bool IsAdmin { get; set; } = false;  // Static value for the admin group
    public string LogFilePath { get; set; } = CLogger.GetLogFilePath() ?? System.IO.Path.Combine("Logs", $"{DateTime.Now.ToString("yyyyMMdd")}-appsettings-json-missing-wsuscommander.log");
    public List<ServerGroup> ServerGroups { get; set; }
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
            // Convert the dictionary keys to a list of ServerGroup objects
            ServerGroups = serverGroups.Keys
                                       .Select(key => new ServerGroup { Name = key, IsChecked = false })
                                       .ToList();
        }
        else
        {
            // Handle case where the ServerGroups key does not exist or is invalid
            ServerGroups = new List<ServerGroup>
            {
                new ServerGroup { Name = "The", IsChecked = false },
                new ServerGroup { Name = "servers.txt", IsChecked = false },
                new ServerGroup { Name = "file", IsChecked = false },
                new ServerGroup { Name = "is", IsChecked = false },
                new ServerGroup { Name = "missing", IsChecked = false }
            };
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
        return;
    }

    private async void btn_ExecuteProcess_Click(object sender, RoutedEventArgs e)
    {
        List<string> listOfTargetServers = new();

        // Retrieve the dictionary from Application.Current.Properties
        if (Application.Current.Properties["ServerGroups"] is Dictionary<string, List<string>> serverGroupsDictionary)
        {
            // Get the checked items from the UI
            var checkedGroups = ServerGroups.Where(sg => sg.IsChecked).ToList();

            if (checkedGroups.Any())
            {
                // Iterate through the checked groups and find their corresponding values in the dictionary
                foreach (var group in checkedGroups)
                {
                    // Check if the dictionary contains the group name as a key
                    if (serverGroupsDictionary.TryGetValue(group.Name, out List<string>? groupValues))
                    {
                        // Log the group name and the values associated with it
                        CLogger.Info($"Checked group: {group.Name}");
                        foreach (var value in groupValues)
                        {
                            CLogger.Info($"  -> Contains machine: [{value}]");
                            listOfTargetServers.Add(value);
                        }
                    }
                    else
                    {
                        CLogger.Warn($"Group '{group.Name}' was checked but not found in the dictionary.");
                    }
                }
            }
            else
            {
                CLogger.Warn("No groups were checked.");
            }
        }
        else
        {
            CLogger.Error("The ServerGroups dictionary is not available in Application.Current.Properties.");
            return;
        }


        // Ping each server
        ProgressWindow progressWindow = new ProgressWindow();
        progressWindow.SetWindowTitle("Pinging Servers");
        progressWindow.Show();

        for (int i = 0;  i < listOfTargetServers.Count; i++)
        {
            if (progressWindow.IsCancelled)
            {
                MessageBox.Show("Operation cancelled");
                break;
            }

            // Test connection to each server
            await SysCheck.IsMachineReachableAsync(listOfTargetServers[i]);

            // Update progress (percentage)
            double percentage = (double)i / listOfTargetServers.Count * 100;
            progressWindow.UpdateProgress(percentage);

            // Update iteration text
            progressWindow.UpdateIterationText($"Processing {i} of {listOfTargetServers.Count}");

            await Task.Delay(100);
        }
        progressWindow.Close();
    }

}