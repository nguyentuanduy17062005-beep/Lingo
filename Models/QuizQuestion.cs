namespace LingoAppNet8.Models
{
    public class QuizQuestion
    {
        public int QuestionId { get; set; }
        public string Question { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty; // A, B, C, or D
        public string Difficulty { get; set; } = "Normal"; // Easy, Normal, Hard
        public int TimeLimit { get; set; } // Seconds
        public int TenseId { get; set; }
        public TenseData? Tense { get; set; }
    }

    public class QuizResult
    {
        public int QuizResultId { get; set; }
        public int UserId { get; set; }
        public DateTime CompletedDate { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int Score { get; set; }
        public int TimeSpent { get; set; } // Total seconds
        public User? User { get; set; }
    }
}
