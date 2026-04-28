using FluentAssertions;
using TeamTaskTracking.Domain.Projects;

namespace TeamTaskTracking.UnitTests.Projects;

public sealed class TaskItemTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateTaskItem()
    {
        var projectId = Guid.NewGuid();

        var task = new TaskItem(projectId, " Test task ", " Description ");

        task.Id.Should().NotBeEmpty();
        task.ProjectId.Should().Be(projectId);
        task.Title.Should().Be("Test task");
        task.Description.Should().Be("Description");
        task.IsCompleted.Should().BeFalse();
        task.CreateAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_WithEmptyProjectId_ShouldThrowArgumentException()
    {
        var act = () => new TaskItem(Guid.Empty, "Task", null);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidTitle_ShouldThrowArgumentException(string title)
    {
        var act = () => new TaskItem(Guid.NewGuid(), title, null);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithTooLongTitle_ShouldThrowArgumentException()
    {
        var act = () => new TaskItem(Guid.NewGuid(), new string('a', 151), null);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithTooLongDescription_ShouldThrowArgumentException()
    {
        var act = () => new TaskItem(Guid.NewGuid(), "Task", new string('a', 1501));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MarkCompleted_ShouldSetIsCompletedToTrue()
    {
        var task = new TaskItem(Guid.NewGuid(), "Task", null);

        task.MarkCompleted();

        task.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void MarkOpen_ShouldSetIsCompletedToFalse()
    {
        var task = new TaskItem(Guid.NewGuid(), "Task", null);
        task.MarkCompleted();

        task.MarkOpen();

        task.IsCompleted.Should().BeFalse();
    }
}