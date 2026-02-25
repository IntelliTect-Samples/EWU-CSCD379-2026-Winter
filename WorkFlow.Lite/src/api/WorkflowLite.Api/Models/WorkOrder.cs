namespace WorkflowLite.Api.Models;

public enum WorkOrderStatus { New, InProgress, Blocked, Done }
public enum Priority { Low, Medium, High, Urgent }

public class WorkOrder
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.New;
    public Priority Priority { get; set; } = Priority.Medium;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedByUserId { get; set; } = "";

    public List<WorkOrderComment> Comments { get; set; } = new();
}
