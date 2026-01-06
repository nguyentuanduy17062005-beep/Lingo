using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingoAppNet8.Models
{
    public class SpeakingSentence
    {
        [Key]
        public int Id { get; set; }
        
        public required string EnglishText { get; set; }
        
        public required string VietnameseTranslation { get; set; }
        
        public required string Category { get; set; } // "Daily", "Business", "Travel", etc.
        
        public required string Level { get; set; } // "Easy", "Medium", "Hard"
        
        public string? PhoneticTranscription { get; set; } // IPA notation
    }

    public class SpeakingResult
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public int SentenceId { get; set; }
        
        public double AccuracyScore { get; set; } // 0-100
        
        public double FluencyScore { get; set; } // 0-100
        
        public double CompletenessScore { get; set; } // 0-100
        
        public double PronunciationScore { get; set; } // Overall score 0-100
        
        public required string RecognizedText { get; set; }
        
        public DateTime PracticeDate { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        
        [ForeignKey("SentenceId")]
        public SpeakingSentence Sentence { get; set; } = null!;
    }
}
