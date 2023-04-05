using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAppQA.Infrastructure.Domain
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(35)]
        public string Name { get; set; }

        [InverseProperty("Category")]
        public ICollection<Question> Questions { get; set; }
    }
}
