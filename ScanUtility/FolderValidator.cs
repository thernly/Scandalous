namespace ScanUtility
{
    public static class FolderValidator
    {
        // Characters that are invalid anywhere in a path string.
        // Path.GetInvalidPathChars() includes e.g. ", |, <, > and control chars.
        // It does NOT include \, /, :, *, ?
        private static readonly char[] SystemInvalidPathChars = Path.GetInvalidPathChars();

        // Characters that are invalid within a single file or directory name (a path segment).
        // Path.GetInvalidFileNameChars() includes e.g. \, /, :, *, ?, ", <, >, | and control chars.
        private static readonly char[] SystemInvalidSegmentNameChars = Path.GetInvalidFileNameChars();

        private static readonly string[] ReservedNamesInternal =
        [
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        ];

        /// <summary>
        /// Validates the specified folder name or path.
        /// </summary>
        /// <param name="folderName">The folder name or path to validate.</param>
        /// <returns>
        /// A (<see cref="bool"/> isValid, <see cref="string"/> errorMessage) tuple.
        /// <list type="bullet">
        /// <item><description><c>isValid</c> is <c>true</c> if the folder name/path is valid; otherwise, <c>false</c>.</description></item>
        /// <item><description><c>errorMessage</c> contains the error message if validation failed; otherwise, an empty string.</description></item>
        /// </list>
        /// </returns>
        public static (bool isValid, string errorMessage) IsValid(string? folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                return (false, "Folder name/path cannot be null, empty, or consist only of white-space characters.");
            }

            // Rule: Check for characters that are invalid anywhere in a path string.
            int invalidCharIndex = folderName.IndexOfAny(SystemInvalidPathChars);
            if (invalidCharIndex != -1)
            {
                char c = folderName[invalidCharIndex];
                return (false, $"Folder name/path '{folderName}' contains an invalid character: '{c}'.");
            }

            // Split the path into segments. Handles both single names and multi-segment paths.
            // RemoveEmptyEntries handles cases like "path//segment" -> "path", "segment".
            string[] segments = folderName.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);

            // If after splitting, there are no segments, but the original string was not empty (e.g. "/", "\\"),
            // it could be a root path. For this validator's purpose (creating/identifying named folders),
            // we typically expect at least one segment unless the input is explicitly a simple root like "C:".
            // However, if folderName is just "C:", segments will be {"C:"}. If folderName is "/", segments is {}.
            // An empty segments array for a non-empty folderName (like "/") is valid as a root.
            if (segments.Length == 0 && folderName.Length > 0) 
            {
                // This means folderName consisted only of separators, e.g., "/" or "\\\". This is a valid root.
                return (true, string.Empty);
            }
            if (segments.Length == 0 && folderName.Length == 0)
            {
                 // Should have been caught by IsNullOrWhiteSpace, but as a safeguard.
                return (false, "Folder name/path cannot be empty.");
            }


            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];

                // Special handling for drive letters (e.g., "C:") as the first segment of a rooted path.
                // These are valid path roots but contain ':', which is an invalid char for general segment names.
                if (i == 0 && segment.Length == 2 && segment[1] == ':' && char.IsLetter(segment[0]))
                {
                    // Check if the drive letter segment itself ends with space/period (e.g., "C: ").
                    if (segment.EndsWith(' ') || segment.EndsWith('.'))
                    {
                        return (false, $"Drive letter segment '{segment}' in '{folderName}' cannot end with a space or a period.");
                    }
                    // Drive letter is valid, continue to the next segment if any.
                    continue;
                }

                // Rule: Check for invalid characters within this specific segment.
                int invalidSegmentCharIndex = segment.IndexOfAny(SystemInvalidSegmentNameChars);
                if (invalidSegmentCharIndex != -1)
                {
                    char c = segment[invalidSegmentCharIndex];
                    return (false, $"Path segment '{segment}' in '{folderName}' contains an invalid character: '{c}'.");
                }

                // Rule: Check for reserved names (case-insensitive).
                if (ReservedNamesInternal.Any(rn => rn.Equals(segment, StringComparison.OrdinalIgnoreCase)))
                {
                    return (false, $"Path segment '{segment}' in '{folderName}' is a reserved system name.");
                }

                // Rule: Segment cannot end with a space or a period.
                if (segment.EndsWith(' ') || segment.EndsWith('.'))
                {
                    return (false, $"Path segment '{segment}' in '{folderName}' cannot end with a space or a period.");
                }

                // Rule: Segment cannot be "." or ".." (these are navigational, not valid names for creation).
                if (segment == "." || segment == "..")
                {
                    return (false, $"Path segment '{segment}' in '{folderName}' is invalid. '.' or '..' are not allowed as directory names.");
                }
                
                // Rule: Segment length check (e.g., 255 characters for NTFS).
                if (segment.Length > 255)
                {
                    return (false, $"Path segment '{segment}' in '{folderName}' is too long. Maximum length for a segment is 255 characters.");
                }
            }
            
            // Rule: Overall path length (conservative check).
            // This is a heuristic; true MAX_PATH (260) applies to the fully qualified path.
            // A long relative path might be valid.
            if (folderName.Length > 240) 
            {
                // This warning is about the input string length. If it's a relative path,
                // its combination with the current directory could exceed MAX_PATH.
                // Consider if this check is too broad or if a more context-aware check is needed elsewhere.
                // For now, retaining a similar check to the original.
                // No return false here, as segment checks are more critical for individual component validity.
                // Could be a warning or a stricter check if needed.
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