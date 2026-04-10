namespace JiraCopyProject_My.Models
{
    public class AccountStat
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Role { get; set; }
        public int TasksCreatedCount { get; set; }   // как заказчик
        public int TasksAssignedCount { get; set; }  // как исполнитель
    }
}