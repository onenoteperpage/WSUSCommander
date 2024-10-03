// File: Extensions/FileParser.cs
using System;
using System.Collections.Generic;
using System.IO;

namespace WSUSCommander.Extensions
{
    internal class FileParser
    {
        // Method to read the servers.txt file
        public static Dictionary<string, List<string>> ReadServersTxtFile(string filePath)
        {
            var groups = new Dictionary<string, List<string>>();
            string? currentGroup = null;  // Make current group null

            foreach (var line in File.ReadLines(filePath))
            {
                var trimmedLine = line.Trim();

                // Skip empty lines
                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;

                // Check if the line defines a new group (e.g., [OTHER])
                if (trimmedLine.StartsWith('[') && trimmedLine.EndsWith(']'))
                {
                    currentGroup = trimmedLine.Trim('[', ']');
                    groups[currentGroup] = new List<string>();
                }
                // Otherwise, it's a server name under the current group
                else if (currentGroup != null)
                {
                    groups[currentGroup].Add(trimmedLine);
                }
            }

            return groups;
        }
    }
}
