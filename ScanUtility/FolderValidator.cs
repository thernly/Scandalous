namespace ScanUtility
{
    public static class FolderValidator
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidPathChars();
        private static readonly string[] ReservedNames =
        [
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        ];

        /// <summary>
        /// Validates the specified folder name.
        /// </summary>
        /// <param name="folderName">The name of the folder to validate.</param>
        /// <returns>
        /// A (<see cref="bool"/> isValid, <see cref="string"/> errorMessage) tuple.
        /// <list type="bullet">
        /// <item><description><c>isValid</c> is <c>true</c> if the folder name is valid; otherwise, <c>false</c>.</description></item>
        /// <item><description><c>errorMessage</c> contains the error message if validation failed; otherwise, an empty string.</description></item>
        /// </list>
        /// </returns>
        public static (bool isValid, string errorMessage) IsValid(string? folderName)
        {
            string errorMessage;

            if (string.IsNullOrWhiteSpace(folderName))
            {
                errorMessage = "Folder name cannot be null, empty, or consist only of white-space characters.";
                return (false, errorMessage);
            }

            // Check for invalid characters
            if (folderName.Any(c => InvalidFileNameChars.Contains(c)))
            {
                errorMessage = $"Folder name '{folderName}' contains invalid characters. Invalid characters are: {string.Join(" ", InvalidFileNameChars.Where(c => c != Path.DirectorySeparatorChar && c != Path.AltDirectorySeparatorChar).Distinct())}";
                return (false, errorMessage);
            }

            // Check for reserved names (comparison should be case-insensitive)
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(folderName);
            if (ReservedNames.Contains(nameWithoutExtension.ToUpperInvariant()))
            {
                errorMessage = $"Folder name '{folderName}' is a reserved system name.";
                return (false, errorMessage);
            }

            // Check if folder name ends with a space or a period
            if (folderName.EndsWith(' ') || folderName.EndsWith('.'))
            {
                errorMessage = $"Folder name '{folderName}' cannot end with a space or a period.";
                return (false, errorMessage);
            }

            // Check for path traversal attempts (e.g., "..", "./")
            // This is a basic check; more sophisticated path normalization/validation might be needed depending on context.
            if (folderName.Contains("..") || folderName.Contains("./") || folderName.Contains(".\\"))
            {
                // Allow "." as a segment if it's the only thing (represents current directory)
                // but not as part of a multi-segment name like "foo/./bar" if we are validating single folder names.
                // For a single folder name, "." or ".." are invalid.
                if (folderName == "." || folderName == "..")
                {
                     errorMessage = $"Folder name '{folderName}' cannot be '.' or '..'.";
                     return (false, errorMessage);
                }
                // More specific check for ".." as a segment
                string[] segments = folderName.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);
                if (segments.Any(s => s == "..")) {
                    errorMessage = $"Folder name '{folderName}' contains path traversal sequences ('..').";
                    return (false, errorMessage);
                }
            }
            
            // Check for path length (this validates the segment length, not the full path)
            // Windows API typically has a MAX_PATH limit of 260 characters for the full path.
            // A single folder name (segment) is usually limited by file system (e.g. 255 chars for NTFS).
            if (folderName.Length > 240) // A conservative limit for a single segment
            {
                errorMessage = $"Folder name '{folderName}' is too long. Maximum length for a folder name segment is typically around 240 characters.";
                return (false, errorMessage);
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Validates the specified folder name and throws an ArgumentException if it's invalid.
        /// </summary>
        /// <param name="folder">The name of the folder to validate.</param>
        /// <exception cref="ArgumentException">Thrown if the folder name is invalid.</exception>
        public static void Validate(string? folder)
        {
            var (isValid, errorMessage) = IsValid(folder);
            if (!isValid)
            {
                throw new ArgumentException(errorMessage, nameof(folder));
            }
        }
    }
}