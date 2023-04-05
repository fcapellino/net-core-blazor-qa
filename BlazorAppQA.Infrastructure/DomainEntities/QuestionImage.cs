using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAppQA.Infrastructure.Domain
{
    [Table("QuestionImages")]
    public class QuestionImage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [Required]
        public Guid FileName { get; set; }
    }
}
