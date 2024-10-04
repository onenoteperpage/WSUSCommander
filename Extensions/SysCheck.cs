using System;
using System.Net.NetworkInformation;

namespace WSUSCommander.Extensions
{
    internal class SysCheck
    {
        public static bool IsMachineReachable(string machineName)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(machineName, 3000); // Timeout of 3000 ms (3 seconds)
                    if (reply.Status == IPStatus.Success)
                    {
                        CLogger.Info($"Pint to '{machineName}' successful.");
                        return true;
                    }
                }
            }
            catch (PingException)
            {
                // Ping failed due to network issues or invalid machine name
                CLogger.Debug($"Ping to '{machineName}' failed due to network issues or invalid machine name");
                return false;
            }
            catch (Exception ex)
            {
                CLogger.Error($"An error occurred: {ex.Message}");
                return false;
            }

            CLogger.Warn($"Unknown 'false' for ping to '{machineName}'.");
            return false;
        }
    }
}
