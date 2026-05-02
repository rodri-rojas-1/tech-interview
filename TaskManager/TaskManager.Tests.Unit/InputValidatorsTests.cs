using TaskManager.Application.Exceptions;
using TaskManager.Application.Validation;
using TaskManager.Domain;

namespace TaskManager.Tests.Unit;

public sealed class InputValidatorsTests
{
    [Fact]
    public void NormalizeEmail_WhenNullOrWhitespace_Throws()
    {
        Assert.Throws<AppValidationException>(() => InputValidators.NormalizeEmail("  "));
    }

    [Fact]
    public void NormalizeEmail_TrimsAndLowercases()
    {
        var result = InputValidators.NormalizeEmail("  Test@Example.COM ");
        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public void EnsureValidEmail_WhenInvalid_Throws()
    {
        Assert.Throws<AppValidationException>(() =>
            InputValidators.EnsureValidEmail("not-an-email"));
    }

    [Fact]
    public void EnsurePasswordStrength_WhenTooShort_Throws()
    {
        Assert.Throws<AppValidationException>(() =>
            InputValidators.EnsurePasswordStrength("1234567"));
    }

    [Fact]
    public void EnsureTaskTitle_WhenTooLong_Throws()
    {
        var tooLong = new string('a', InputValidators.MaxTitleLength + 1);
        Assert.Throws<AppValidationException>(() =>
            InputValidators.EnsureTaskTitle(tooLong));
    }

    [Fact]
    public void EnsureDescriptionLength_WhenTooLong_Throws()
    {
        var tooLong = new string('a', InputValidators.MaxDescriptionLength + 1);
        Assert.Throws<AppValidationException>(() =>
            InputValidators.EnsureDescriptionLength(tooLong));
    }

    [Fact]
    public void EnsureTaskStatus_WhenInvalid_Throws()
    {
        Assert.Throws<AppValidationException>(() =>
            InputValidators.EnsureTaskStatus((TaskItemStatus)999));
    }
}
