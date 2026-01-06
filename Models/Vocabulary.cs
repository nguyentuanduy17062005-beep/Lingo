using System;
using System.ComponentModel.DataAnnotations;

namespace LingoAppNet8.Models
{
    public class Vocabulary
    {
        [Key]
        public int VocabularyId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Word { get; set; }
        
        [MaxLength(500)]
        public string Definition { get; set; }
        
        [MaxLength(200)]
        public string Example { get; set; }
        
        [MaxLength(50)]
        public string Category { get; set; }
        
        public int UserId { get; set; }
        public virtual User User { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }
}

