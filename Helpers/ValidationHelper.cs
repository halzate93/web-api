using System.Text.RegularExpressions;

namespace UserManagement.Helpers;

public static class ValidationHelper
{
    private static readonly Regex NameRegex = new Regex(@"^[a-zA-Z\s]{2,50}$", RegexOptions.Compiled);
    private static readonly Regex UsernameRegex = new Regex(@"^[a-zA-Z0-9_-]{3,20}$", RegexOptions.Compiled);
    private static readonly Regex EmailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

    public static (bool isValid, string errorMessage) ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return (false, "Name is required");
        }

        if (ContainsScriptTag(name))
        {
            return (false, "Name contains invalid content");
        }

        if (!NameRegex.IsMatch(name))
        {
            return (false, "Name must be 2-50 characters and contain only letters and spaces");
        }

        return (true, string.Empty);
    }

    public static (bool isValid, string errorMessage) ValidateUsername(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return (false, "Username is required");
        }

        if (ContainsScriptTag(username))
        {
            return (false, "Username contains invalid content");
        }

        if (!UsernameRegex.IsMatch(username))
        {
            return (false, "Username must be 3-20 characters and contain only letters, numbers, hyphens, and underscores");
        }

        return (true, string.Empty);
    }

    public static (bool isValid, string errorMessage) ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return (false, "Email is required");
        }

        if (ContainsScriptTag(email))
        {
            return (false, "Email contains invalid content");
        }

        if (!EmailRegex.IsMatch(email))
        {
            return (false, "Email must be in a valid format");
        }

        return (true, string.Empty);
    }

    private static bool ContainsScriptTag(string input)
    {
        return input.Contains("<script>", StringComparison.OrdinalIgnoreCase) ||
               input.Contains("</script>", StringComparison.OrdinalIgnoreCase);
    }
}
