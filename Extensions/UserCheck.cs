using System;
using System.Security.Principal;

namespace WSUSCommander.Extensions
{
    internal class UserCheck
    {
        public static bool IsUserAdministrator()
        {
            try
            {
                // Get the current user's Windows identity
                WindowsIdentity identity = WindowsIdentity.GetCurrent();

                // Create a WindowsPrincipal object to represent the user
                WindowsPrincipal principal = new WindowsPrincipal(identity);

                // Get the SID for the local Administrators group
                SecurityIdentifier adminGroupSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);

                if (principal.IsInRole(adminGroupSid))
                {
                    CLogger.Debug("User is a member of Administrators group on local machine.");
                }
                else
                {
                    CLogger.Debug("User is NOT a member of Administrators group on local machine.");
                }

                // Check if the current user is in the Administrators group
                return principal.IsInRole(adminGroupSid);
            }
            catch (UnauthorizedAccessException)
            {
                CLogger.Error("Access denied to access user security groups.");
                return false;
            }
            catch (Exception ex)
            {
                CLogger.Error($"An error occurred: {ex.Message}");
                return false;
            }
        }
    }
}
