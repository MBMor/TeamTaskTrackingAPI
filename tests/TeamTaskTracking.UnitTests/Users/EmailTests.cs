using FluentAssertions;
using TeamTaskTracking.Domain.Users;

namespace TeamTaskTracking.UnitTests.Users;

public sealed class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldNormalizeEmail()
    {
        var email = Email.Create("  TEST@Example.COM  ");

        email.Value.Should().Be("test@example.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-email")]
    [InlineData("missing-domain@")]
    [InlineData("@missing-local.com")]
    public void Create_WithInvalidEmail_ShouldThrowArgumentException(string value)
    {
        var act = () => Email.Create(value);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WhenEmailIsLongerThan254Characters_ShouldThrowArgumentException()
    {
        var value = $"{new string('a', 245)}@example.com";

        var act = () => Email.Create(value);

        act.Should().Throw<ArgumentException>();
    }
}