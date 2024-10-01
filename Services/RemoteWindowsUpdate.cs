using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace WSUSCommander.Services;

public static class RemoteWindowsUpdate
{
    /// <summary>
    /// Retrieves the number of available Windows Updates on a remote machine.
    /// </summary>
    /// <param name="remoteComputer">The hostname or IP address of the remote machine.</param>
    /// <returns>The count of available Windows Updates.</returns>
    /// <exception cref="ArgumentException">Thrown when the remoteComputer parameter is null or empty.</exception>
    /// <exception cref="Exception">Thrown when there is an error retrieving the update count.</exception>
    public static int Count(string remoteComputer)
    {
        if (string.IsNullOrWhiteSpace(remoteComputer))
            throw new ArgumentException("Remote computer name cannot be null or empty.", nameof(remoteComputer));

        // Define the PowerShell script to execute
        string script = @"
            Import-Module PSWindowsUpdate
            $updates = Get-WindowsUpdate -MicrosoftUpdate -AcceptAll -IgnoreReboot | Where-Object {$_.IsInstalled -eq $false}
            $updates.Count
        ";

        // Set up the connection information using the current user's credentials
        WSManConnectionInfo connectionInfo = new WSManConnectionInfo(
            new Uri($"http://{remoteComputer}:5985/wsman")
        );
        connectionInfo.ShellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
        connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;
        connectionInfo.OperationTimeout = 4 * 60 * 1000; // 4 minutes

        // Handle untrusted certificates if using HTTPS
        connectionInfo.SkipCACheck = true;
        connectionInfo.SkipCNCheck = true;
        connectionInfo.SkipRevocationCheck = true;

        using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
        {
            try
            {
                runspace.Open();

                using (PowerShell ps = PowerShell.Create())
                {
                    ps.Runspace = runspace;
                    ps.AddScript(script);

                    // Invoke the script
                    var results = ps.Invoke();

                    // Check for PowerShell errors
                    if (ps.Streams.Error.Count > 0)
                    {
                        string errorMessages = string.Join(Environment.NewLine, ps.Streams.Error);
                        throw new Exception($"PowerShell Error: {errorMessages}");
                    }

                    // Parse and return the result
                    if (results.Count > 0 && int.TryParse(results[0].ToString(), out int count))
                    {
                        return count;
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve the update count.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving update count from '{remoteComputer}': {ex.Message}", ex);
            }
            finally
            {
                if (runspace.RunspaceStateInfo.State == RunspaceState.Opened)
                {
                    runspace.Close();
                }
            }
        }
    }
}
