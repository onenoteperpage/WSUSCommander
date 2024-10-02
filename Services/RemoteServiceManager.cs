using System;
using System.ComponentModel;
using System.Management;
using System.ServiceProcess;
using TimeoutException = System.TimeoutException;

namespace WSUSCommander.Services;

/// <summary>
/// Provides functionalities to manage Windows services on remote machines.
/// </summary>
public static class RemoteServiceManager
{
    /// <summary>
    /// Checks if a specified service is running on a remote machine and attempts to stop it if it exists.
    /// </summary>
    /// <param name="serverName">The hostname or IP address of the remote machine.</param>
    /// <param name="serviceName">The name of the service to check and stop.</param>
    /// <returns>
    /// <c>true</c> if the service does not exist or exists and was successfully stopped;
    /// <c>false</c> if the service exists but couldn't be stopped.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="serverName"/> or <paramref name="serviceName"/> is null or empty.</exception>
    public static bool CheckAndStopService(string serverName, string serviceName)
    {
        // Validate input parameters
        if (string.IsNullOrWhiteSpace(serverName))
            throw new ArgumentException("Server name cannot be null or empty.", nameof(serverName));

        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));

        try
        {
            // Initialize the ServiceController for the specified service on the remote machine
            using (ServiceController sc = new ServiceController(serviceName, serverName))
            {
                // Attempt to retrieve the current status of the service
                ServiceControllerStatus status = sc.Status;

                // If the service is not stopped or stop pending, attempt to stop it
                if (status != ServiceControllerStatus.Stopped && status != ServiceControllerStatus.StopPending)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    return true; // Service was successfully stopped
                }
                else
                {
                    // Service is already stopped
                    return true;
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            // Handle scenarios where the service does not exist
            if (ex.InnerException is Win32Exception win32Ex && win32Ex.NativeErrorCode == 1060)
            {
                // ERROR_SERVICE_DOES_NOT_EXIST
                return true; // Service does not exist
            }
            else
            {
                // Other InvalidOperationExceptions indicate issues accessing the service
                return false;
            }
        }
        catch (TimeoutException)
        {
            // Thrown if the service did not stop within the specified timeout
            return false;
        }
        catch (Exception)
        {
            // Catch-all for any other exceptions that may occur
            return false;
        }
    }


    /// <summary>
    /// Checks if a specified service is running on a remote machine and attempts to start it if necessary.
    /// </summary>
    /// <param name="serverName">The hostname or IP address of the remote machine.</param>
    /// <param name="serviceName">The name of the service to check and start.</param>
    /// <returns>
    /// <c>true</c> if the service does not exist, or exists and is not set to "Automatic", or was successfully started;
    /// <c>false</c> if the service exists, is set to "Automatic", but could not be started.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="serverName"/> or <paramref name="serviceName"/> is null or empty.</exception>
    public static bool CheckAndStartService(string serverName, string serviceName)
    {
        // Validate input parameters
        if (string.IsNullOrWhiteSpace(serverName))
            throw new ArgumentException("Server name cannot be null or empty.", nameof(serverName));

        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));

        try
        {
            // Use WMI to query the service on the remote machine
            string query = $"SELECT * FROM Win32_Service WHERE Name = '{serviceName}'";
            ManagementScope scope = new ManagementScope($"\\\\{serverName}\\root\\cimv2");
            scope.Connect();

            ObjectQuery objectQuery = new ObjectQuery(query);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, objectQuery);
            ManagementObjectCollection services = searcher.Get();

            if (services.Count == 0)
            {
                // Service does not exist
                return true;
            }

            // Assuming service names are unique, take the first result
            foreach (ManagementObject service in services)
            {
                string startMode = service["StartMode"]?.ToString() ?? string.Empty;

                if (!string.Equals(startMode, "Automatic", StringComparison.OrdinalIgnoreCase))
                {
                    // Service exists but StartupType is not "Automatic"
                    return true;
                }

                // Service is set to "Automatic", attempt to start it
                // Use ServiceController to manage the service
                using (ServiceController sc = new ServiceController(serviceName, serverName))
                {
                    if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.StartPending)
                    {
                        // Service is already running or in the process of starting
                        return true;
                    }

                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));

                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        // Service was successfully started
                        return true;
                    }
                    else
                    {
                        // Service exists and was set to "Automatic" but could not be started
                        return false;
                    }
                }
            }

            // Default return true if no conditions met (should not reach here)
            return true;
        }
        catch (ManagementException ex)
        {
            // Handle WMI-specific exceptions
            Console.WriteLine($"WMI Error: {ex.Message}");
            return false;
        }
        catch (System.TimeoutException)
        {
            // Thrown if the service did not start within the specified timeout
            return false;
        }
        catch (Exception)
        {
            // Catch-all for any other exceptions that may occur
            return false;
        }
    }
}
