using System;
using System.ComponentModel.DataAnnotations;

namespace LingoAppNet8.Models
{
    public class GrammarRule
    {
        [Key]
        public int GrammarRuleId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Examples { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }

        public int Level { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}

