using System.Net.Mail;
using TaskManager.Application.Exceptions;
using TaskManager.Domain;

namespace TaskManager.Application.Validation;

public static class InputValidators
{
    public const int MaxTitleLength = 500;
    public const int MaxDescriptionLength = 5000;
    public const int MinPasswordLength = 8;

    public static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new AppValidationException("Email is required.");

        return email.Trim().ToLowerInvariant();
    }

    public static void EnsureValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
        }
        catch
        {
            throw new AppValidationException("Email is not valid.");
        }
    }

    public static void EnsurePasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
            throw new AppValidationException($"Password must be at least {MinPasswordLength} characters.");
    }

    public static void EnsureTaskTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new AppValidationException("Title is required.");

        if (title.Length > MaxTitleLength)
            throw new AppValidationException($"Title cannot exceed {MaxTitleLength} characters.");
    }

    public static void EnsureDescriptionLength(string? description)
    {
        if (description is not null && description.Length > MaxDescriptionLength)
            throw new AppValidationException($"Description cannot exceed {MaxDescriptionLength} characters.");
    }

    public static void EnsureTaskStatus(TaskItemStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new AppValidationException("Task status is not valid.");
    }
}
