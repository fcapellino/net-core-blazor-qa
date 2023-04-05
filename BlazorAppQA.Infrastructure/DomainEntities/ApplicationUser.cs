using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BlazorAppQA.Infrastructure.Domain
{
    [Table("AspNetUsers")]
    public sealed class ApplicationUser : IdentityUser<int>
    {
        public ApplicationUser()
        {
        }

        [Required]
        public string Base64AvatarImage { get; set; }

        [Required, MaxLength(800)]
        public string Biography { get; set; }

        [Required, MaxLength(250)]
        public string LinkedinProfileUrl { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

        [InverseProperty("User")]
        public ICollection<Question> UserQuestions { get; set; }

        [InverseProperty("User")]
        public ICollection<Answer> UserAnswers { get; set; }

        [InverseProperty("User")]
        public ICollection<Vote> UserVotes { get; set; }
    }
}
