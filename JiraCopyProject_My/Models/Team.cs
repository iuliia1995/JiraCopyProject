namespace JiraCopyProject_My.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? TeamLeadId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}