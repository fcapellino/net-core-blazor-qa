using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAppQA.Infrastructure.Domain
{
    [Table("Votes")]
    public sealed class Vote
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Answer")]
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }

        [Required]
        public bool Upvote { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
