using System;
using System.IO;
using WSUSCommander.Models;

namespace WSUSCommander.Services;

public static class RemoteFileManager
{
    /// <summary>
    /// Checks if a specified file exists on a remote machine via UNC path, backs it up to a designated temporary directory,
    /// and optionally clears the contents of the original file.
    /// </summary>
    /// <param name="serverName">The hostname or IP address of the remote machine.</param>
    /// <param name="uncFilePath">The UNC path to the file on the remote machine (e.g., \\Server\Share\Path\To\File.txt).</param>
    /// <param name="tempDir">The temporary directory path where backups will be stored.</param>
    /// <param name="folder">The subfolder within the temporary directory to create for storing the backup.</param>
    /// <param name="emptyFile">A boolean indicating whether to clear the contents of the original file after backing it up.</param>
    /// <returns>
    /// A <see cref="FileBackupResult"/> object containing details about the backup operation.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when any of the required parameters are null or empty.</exception>
    /// <exception cref="IOException">Thrown when file operations fail.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the application lacks necessary permissions.</exception>
    public static FileBackupResult BackupAndClearFile(string serverName, string uncFilePath, string tempDir, string folder, bool emptyFile)
    {
        // Validate parameters
        if (string.IsNullOrWhiteSpace(serverName))
            throw new ArgumentException("Server name cannot be null or empty.", nameof(serverName));

        if (string.IsNullOrWhiteSpace(uncFilePath))
            throw new ArgumentException("UNC file path cannot be null or empty.", nameof(uncFilePath));

        if (string.IsNullOrWhiteSpace(tempDir))
            throw new ArgumentException("Temporary directory path cannot be null or empty.", nameof(tempDir));

        if (string.IsNullOrWhiteSpace(folder))
            throw new ArgumentException("Folder name cannot be null or empty.", nameof(folder));

        // Combine tempDir and folder to create backup directory path
        string backupDir = Path.Combine(tempDir, folder);

        // Ensure backup directory exists
        try
        {
            Directory.CreateDirectory(backupDir);
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to create backup directory '{backupDir}': {ex.Message}", ex);
        }

        // Initialize result
        FileBackupResult result = new FileBackupResult
        {
            ServerName = serverName,
            OriginalFilePath = uncFilePath,
            BackupDirectory = backupDir,
            BackupFilePath = string.Empty,
            FileExisted = false,
            BackupSuccessful = false,
            FileCleared = false
        };

        // Check if the file exists
        bool fileExists;
        try
        {
            fileExists = File.Exists(uncFilePath);
            result.FileExisted = fileExists;
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to check existence of file '{uncFilePath}': {ex.Message}", ex);
        }

        if (!fileExists)
        {
            return result; // File does not exist; nothing to do
        }

        // Get the file name
        string fileName = Path.GetFileName(uncFilePath);
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("Invalid UNC file path provided.", nameof(uncFilePath));
        }

        // Define backup file path
        string backupFilePath = Path.Combine(backupDir, fileName);
        result.BackupFilePath = backupFilePath;

        // Attempt to copy the file
        try
        {
            File.Copy(uncFilePath, backupFilePath, overwrite: true);
            result.BackupSuccessful = true;
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to backup file from '{uncFilePath}' to '{backupFilePath}': {ex.Message}", ex);
        }

        // Optionally, clear the contents of the original file
        if (emptyFile)
        {
            try
            {
                // Truncate the file by setting its length to zero
                using (FileStream fs = new FileStream(uncFilePath, FileMode.Truncate))
                {
                    // No need to write anything; opening with FileMode.Truncate sets length to zero
                }
                result.FileCleared = true;
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to clear the contents of file '{uncFilePath}': {ex.Message}", ex);
            }
        }

        return result;
    }
}

