

using System.Xml.Linq;

namespace TeamTaskTracking.Domain.Projects;

public sealed class Project
{
    public Guid Id { get; private set; }
    public Guid OwnerUserId { get; private set;  }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private readonly List<TaskItem> _tasks = new();
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    private Project()
    {
    }

    public Project(Guid ownerUserId, string name, string? description)
    {
        Id = Guid.NewGuid();
        OwnerUserId = ownerUserId;
        SetName(name);
        SetDescription(description);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string? description)
    {
        SetName(name);
        SetDescription(description);
    }

    private void SetDescription(string? description)
    {
        if (description?.Length > 1500)
            throw new ArgumentException("Supported length for description is 1500 characters and less",
                nameof(description));
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name is required.", nameof(name));

        if(name.Length > 150)
            throw new ArgumentException("Supported length for project name is 150 characters and less",
                nameof(name));

        Name = name.Trim();
    }
}
