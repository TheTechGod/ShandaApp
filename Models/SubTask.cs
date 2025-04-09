using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShandaApp.Models.Enums;

namespace ShandaApp.Models
{
    public class SubTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        public SubTaskStatus Status { get; set; } = SubTaskStatus.Pending;

        // Required foreign key to parent
        public int ToDoItemId { get; set; }

        // Navigation property (no longer required)
        [ForeignKey("ToDoItemId")]
        public ToDoItem? ToDoItem { get; set; } // Made nullable
    }
}
