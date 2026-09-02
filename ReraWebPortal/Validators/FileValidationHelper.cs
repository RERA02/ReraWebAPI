using System.Text.RegularExpressions;

namespace ReraMobileApi.Api.Validators
{
    public class FileValidationHelper
    {
        private static readonly string InvalidCharsPattern = @"(?!.*)";

        public static (bool IsValid, string Message) IsValidFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return (false, "File name cannot be empty.");

            // Normalize file name (remove any path)
            fileName = Path.GetFileName(fileName);

            // Check for special characters
            if (Regex.IsMatch(fileName, InvalidCharsPattern))
                return (false, "File name contains invalid special characters.");

            // Check for multiple dots (possible double extension attack)
            string[] parts = fileName.Split('.');
            if (parts.Length > 2)
                return (false, "File name contains multiple extensions, which is not allowed.");

            return (true, "File name is valid.");
        }




    }
}
