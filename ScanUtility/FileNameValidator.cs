namespace ScanUtility
{
    public static class FileNameValidator
    {
        // These are characters that cannot be used in a file name.
        // Path.GetInvalidFileNameChars() typically includes \, /, :, *, ?, ", <, >, |
        private static readonly char[] InvalidFileNameCharsInternal = Path.GetInvalidFileNameChars();

        // Reserved file names in Windows. These names are not allowed for files or folders,
        // regardless of extension, as the OS treats them specially.
        private static readonly string[] ReservedNamesInternal =
        [
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        ];

        /// <summary>
        /// Validates the specified file name.
        /// </summary>
        /// <param name="name">The file name to validate.</param>
        /// <param name="isBaseNameOnly">
        /// True if the <paramref name="name"/> is a base file name (without extension) and should not contain periods;
        /// False if the <paramref name="name"/> is a full file name and can include an extension.
        /// </param>
        /// <returns>
        /// A (<see cref="bool"/> isValid, <see cref="string"/> errorMessage) tuple.
        /// <list type="bullet">
        /// <item><description><c>isValid</c> is <c>true</c> if the name is valid; otherwise, <c>false</c>.</description></item>
        /// <item><description><c>errorMessage</c> contains the error message if validation failed; otherwise, an empty string.</description></item>
        /// </list>
        /// </returns>
        public static (bool isValid, string errorMessage) IsValid(string? name, bool isBaseNameOnly = false)
        {
            string displayName = isBaseNameOnly ? "Base file name" : "File name";

            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, $"{displayName} cannot be null, empty, or consist only of white-space characters.");
            }

            // Rule: Cannot end with a space or a period.
            if (name.EndsWith(' ') || name.EndsWith('.'))
            {
                return (false, $"{displayName} '{name}' cannot end with a space or a period.");
            }

            // Rule: Cannot be "." or ".."
            // These are directory navigation symbols and not valid file/folder names.
            if (name == "." || name == "..")
            {
                return (false, $"{displayName} '{name}' cannot be '.' or '..'.");
            }

            // Rule: Check for invalid characters.
            foreach (char c in name)
            {
                if (InvalidFileNameCharsInternal.Contains(c))
                {
                    return (false, $"{displayName} '{name}' contains an invalid character: '{c}'.");
                }
            }

            string nameToCheckForReserved;
            if (isBaseNameOnly)
            {
                // Rule: Base file name should not contain '.' (extension separator).
                if (name.Contains('.'))
                {
                    return (false, $"Base file name '{name}' should not contain an extension separator ('.').");
                }
                nameToCheckForReserved = name;
            }
            else // Full filename (can have an extension)
            {
                nameToCheckForReserved = Path.GetFileNameWithoutExtension(name);
                // If name is ".config", nameToCheckForReserved is empty. This is acceptable.
                // If name is "archive.tar.gz", nameToCheckForReserved is "archive.tar".
                // Reserved names are checked against the part before the (final) extension.
            }

            // Rule: Check for reserved names (case-insensitive).
            // Applies to the name part before the extension (e.g., "CON" in "CON.txt").
            if (!string.IsNullOrEmpty(nameToCheckForReserved) &&
                ReservedNamesInternal.Any(rn => rn.Equals(nameToCheckForReserved, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, $"The name component '{nameToCheckForReserved}' (from '{name}') is a reserved system name.");
            }

            // Rule: Length check.
            // The maximum length for a file name component (name + extension) on NTFS is 255 characters.
            if (name.Length > 255)
            {
                return (false, $"{displayName} '{name}' is too long. Maximum length for a file name component is 255 characters.");
            }
            
            // Rule: Base name cannot be effectively empty if it's a base name.
            // (Covered by IsNullOrWhiteSpace for the initial 'name' string).
            // For full filenames, an empty base (e.g. from ".foo") is allowed.

            return (true, string.Empty);
        }

        /// <summary>
        /// Validates the specified file name and throws an ArgumentException if it's invalid.
        /// </summary>
        /// <param name="name">The file name to validate.</param>
        /// <param name="isBaseNameOnly">
        /// True if the <paramref name="name"/> is a base file name (without extension);
        /// False if the <paramref name="name"/> is a full file name.
        /// </param>
        /// <exception cref="ArgumentException">Thrown if the file name is invalid.</exception>
        public static void Validate(string? name, bool isBaseNameOnly = false)
        {
            var (isValid, errorMessage) = IsValid(name, isBaseNameOnly);
            if (!isValid)
            {
                // Determine parameter name for the exception based on context
                string paramName = isBaseNameOnly ? "baseFileName" : "fileName";
                // It's often helpful for the ArgumentException to know which parameter was invalid.
                // However, if this method is called with a general 'name', 'name' might be more appropriate.
                // For now, using a context-specific one.
                throw new ArgumentException(errorMessage, paramName);
            }
        }
    }
}
