using TaskManager.Application.Exceptions;
using TaskManager.Application.Validation;

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
}
