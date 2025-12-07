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

        // Trim before validation
        name = name.Trim();

        if (ContainsHtmlOrScript(name))
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

        // Trim before validation
        username = username.Trim();

        if (ContainsHtmlOrScript(username))
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

        // Trim before validation
        email = email.Trim();

        if (ContainsHtmlOrScript(email))
        {
            return (false, "Email contains invalid content");
        }

        if (!EmailRegex.IsMatch(email))
        {
            return (false, "Email must be in a valid format");
        }

        return (true, string.Empty);
    }

    private static bool ContainsHtmlOrScript(string input)
    {
        // Check for common XSS patterns
        var dangerousPatterns = new[]
        {
            "<script", "</script>",
            "<iframe", "</iframe>",
            "<object", "</object>",
            "<embed", "</embed>",
            "javascript:",
            "onerror=",
            "onload=",
            "onclick=",
            "onmouseover="
        };

        foreach (var pattern in dangerousPatterns)
        {
            if (input.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
