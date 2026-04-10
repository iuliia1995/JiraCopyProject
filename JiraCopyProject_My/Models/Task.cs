namespace JiraCopyProject_My.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DueDate { get; set; }
        public int? TeamId { get; set; }
        public int? AssigneeId { get; set; }
        public int CreatorId { get; set; }
        public int? ParentTaskId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
