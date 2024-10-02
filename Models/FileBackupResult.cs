using System;

namespace WSUSCommander.Models;

public class FileBackupResult
{
        /// <summary>
        /// The name or IP address of the remote server.
        /// </summary>
        public required string ServerName { get; set; }

        /// <summary>
        /// The original UNC file path.
        /// </summary>
        public required string OriginalFilePath { get; set; }

        /// <summary>
        /// The backup directory path where the file was copied.
        /// </summary>
        public required string BackupDirectory { get; set; }

        /// <summary>
        /// The full path to the backed-up file.
        /// </summary>
        public required string BackupFilePath { get; set; }

        /// <summary>
        /// Indicates whether the original file existed on the remote machine.
        /// </summary>
        public bool FileExisted { get; set; }

        /// <summary>
        /// Indicates whether the backup operation was successful.
        /// </summary>
        public bool BackupSuccessful { get; set; }

        /// <summary>
        /// Indicates whether the original file's contents were cleared.
        /// </summary>
        public bool FileCleared { get; set; }
}