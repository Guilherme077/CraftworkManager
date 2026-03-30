using System.ComponentModel.DataAnnotations;

namespace CraftworkManager.Models.LogsModels
{
    public class StatusHistory
    {
        public int Id { get; set; }
        public DateTime ChangedOn { get; set; }
        public string? ChangedByUserId { get; set; }
        public ModificationType ModificationType { get; set; } = ModificationType.StatusChanged;

    }
    public enum ModificationType
    {
        [Display(Name = "Status Alterado")]
        StatusChanged,
        [Display(Name = "Criado")]
        Created,
        [Display(Name = "Deletado")]
        Deleted,
    }
}
