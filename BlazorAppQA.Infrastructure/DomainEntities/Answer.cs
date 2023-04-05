using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAppQA.Infrastructure.Domain
{
    [Table("Answers")]
    public sealed class Answer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [Required, MaxLength(1600)]
        public string Description { get; set; }

        [Required]
        public bool BestAnswer { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [InverseProperty("Answer")]
        public ICollection<Vote> AnswerVotes { get; set; }
    }
}
