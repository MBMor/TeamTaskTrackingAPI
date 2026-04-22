

namespace TeamTaskTracking.Domain.Projects;

public sealed class TaskItem
{
    public Guid Id {get; private set;}
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? MyProperty { get; private set; }
    public bool IsCompleted { get; private set; }
    public  DateTime CreateAtUtc { get; private set; }

    private TaskItem()
    {            
    }

    public TaskItem(Guid projectId, string title, string? description)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("ProjectId is required.", nameof(projectId));

        Id = Guid.NewGuid();
        ProjectId = projectId;
        Title = title;
        Description = description;
        CreateAtUtc = DateTime.UtcNow;
    }
    
    public void Update(string title, string? description)
    {
        SetTitle(title);
        SetDescription(description);
    }

    public void MarkCompleted()
    {
        IsCompleted = true;
    }
    public void MarkOpen()
    {
        IsCompleted = false;
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Task title is required.", nameof(title));

        if (title.Length > 150)
            throw new ArgumentException("Supported length for project name is 150 characters and less", 
                nameof(title));

        Title = title.Trim();
    }

    private void SetDescription(string? description)
    {
        if (description?.Length > 1500)
            throw new ArgumentException("Supported length for description is 1500 characters and less", 
                nameof(description));

        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
