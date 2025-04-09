using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShandaApp.Models.Enums; // Ensure this namespace contains the TaskStatus enum

namespace ShandaApp.Models
{
    public class ToDoItem
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; }

        [Required, StringLength(150)]
        public string Subject { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsComplete { get; set; }

        // UserId is nullable to avoid validation issues during form submission
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? Owner { get; set; }

        public DateTime? CreatedAt { get; set; }

        public List<SubTask> SubTasks { get; set; } = new();

        // Default status set to Pending
        public ToDoTaskStatus Status { get; set; } = ToDoTaskStatus.Pending;

    }
}

