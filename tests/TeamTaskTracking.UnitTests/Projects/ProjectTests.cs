using FluentAssertions;
using TeamTaskTracking.Domain.Projects;

namespace TeamTaskTracking.UnitTests.Projects;

public sealed class ProjectTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateProject()
    {
        var ownerUserId = Guid.NewGuid();

        var project = new Project(ownerUserId, " Test project ", " Description ");

        project.Id.Should().NotBeEmpty();
        project.OwnerUserId.Should().Be(ownerUserId);
        project.Name.Should().Be("Test project");
        project.Description.Should().Be("Description");
        project.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string name)
    {
        var act = () => new Project(Guid.NewGuid(), name, null);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithTooLongName_ShouldThrowArgumentException()
    {
        var act = () => new Project(Guid.NewGuid(), new string('a', 151), null);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithTooLongDescription_ShouldThrowArgumentException()
    {
        var act = () => new Project(Guid.NewGuid(), "Project", new string('a', 1501));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateDetails_WithValidData_ShouldUpdateProject()
    {
        var project = new Project(Guid.NewGuid(), "Old", "Old description");

        project.UpdateDetails(" New ", " New description ");

        project.Name.Should().Be("New");
        project.Description.Should().Be("New description");
    }
}