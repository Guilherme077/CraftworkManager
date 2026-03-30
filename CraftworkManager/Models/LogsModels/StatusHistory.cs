namespace CraftworkManager.Models.LogsModels
{
    public class StatusHistory
    {
        public int Id { get; set; }
        public DateTime ChangedOn { get; set; }
        public string? ChangedByUserId { get; set; }
    }
}
