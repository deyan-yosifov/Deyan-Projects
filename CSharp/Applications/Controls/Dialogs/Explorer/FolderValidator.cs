using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Deyo.Controls.Dialogs.Explorer
{
    public class FolderValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string path = value as string;

            if (FolderValidator.IsValid(path))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, "Not existing folder!");
            }
        }

        public static bool IsValid(string path)
        {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }
    }
}
