using System;
using System.Management;

namespace WSUSCommander.Extensions
{
    internal class SysMessage
    {
        public static void SendMessageToUser(string machineName, string userName, string message)
        {
            // Define the command to be executed remotely
            string command = $"/C msg {userName} {message}";

            // Execute the msg command on the remote server
            ExecuteRemoteCommand(command, machineName);

            CLogger.Info($"Send message '{message}' to '{userName}' on '{machineName}'.");
        }

        private static void ExecuteRemoteCommand(string command, string machineName)
        {
            try
            {
                // Connect to the remote server using WMI
                ManagementScope scope = new ManagementScope($@"\\{machineName}\root\cimv2");
                scope.Connect();

                // Create a new WMI process to execute the command remotely
                ObjectGetOptions options = new ObjectGetOptions();
                ManagementPath path = new ManagementPath("Win32_Process");
                ManagementClass managementClass = new ManagementClass(scope, path, options);

                // Prepare parameters to pass to Win32_Process.Create method
                ManagementBaseObject inParams = managementClass.GetMethodParameters("Create");
                inParams["CommandLine"] = command;

                // Execute the command remotely
                ManagementBaseObject outParams = managementClass.InvokeMethod("Create", inParams, null);

                // Check the return code of the executed command
                uint returnCode = (uint)(outParams["ReturnValue"]);
                if (returnCode == 0)
                {
                    CLogger.Debug("Message sent successfully.");
                }
                else
                {
                    CLogger.Debug($"Error sending message. Return code: {returnCode}");
                }
            }
            catch (Exception ex)
            {
                CLogger.Debug($"An error occurred: {ex.Message}");
            }
        }
    }
}
