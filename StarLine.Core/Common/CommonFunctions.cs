using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace StarLine.Core.Common
{
    public static class CommonFunctions
    {
        private static readonly Random _random = new Random();
        public static IEnumerable<SelectListItem> GetEnumSelectList<TEnum>()
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToInt32(e).ToString(),
                    Text = e.GetType()
                             .GetMember(e.ToString() ?? "")
                             .First()
                             .GetCustomAttribute<DisplayAttribute>()?
                             .Name ?? e.ToString()
                });
        }

        public static string GetDisplayName<TEnum>(int value) where TEnum : Enum
        {
            var enumValue = (TEnum)(object)value;
            var memberInfo = typeof(TEnum).GetMember(enumValue.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                var displayAttr = memberInfo.GetCustomAttribute<DisplayAttribute>();
                if (displayAttr != null)
                    return displayAttr.Name ?? "";
            }

            return enumValue.ToString(); // fallback to enum name
        }
        public static string GenerateRandomPassword(int length)
        {
            const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%&*_-?";

            // Ensure the password contains at least one character from each category
            var password = new StringBuilder();

            // Add at least one uppercase letter
            password.Append(upperCaseChars[_random.Next(upperCaseChars.Length)]);

            // Add at least one lowercase letter
            password.Append(lowerCaseChars[_random.Next(lowerCaseChars.Length)]);

            // Add at least one digit
            password.Append(digits[_random.Next(digits.Length)]);

            // Add at least one special character
            password.Append(specialChars[_random.Next(specialChars.Length)]);

            // Fill the rest of the password with random characters from all sets
            var allChars = upperCaseChars + lowerCaseChars + digits + specialChars;
            for (int i = password.Length; i < length; i++)
            {
                password.Append(allChars[_random.Next(allChars.Length)]);
            }

            // Shuffle the characters to ensure randomness
            return new string(password.ToString().OrderBy(c => _random.Next()).ToArray());
        }
        public static async Task<string> ReplacePlaceholders(string filePath, object model)
        {
            var htmlContent = await File.ReadAllTextAsync(filePath);

            var modelProperties = model.GetType().GetProperties();

            foreach (var property in modelProperties)
            {
                // Example: Replace {Model.PropertyName} with actual property value
                var placeholder = "{" + property.Name + "}";
                var value = property.GetValue(model)?.ToString() ?? string.Empty;
                htmlContent = htmlContent.Replace(placeholder, value);
            }

            return htmlContent;
        }
    }
}