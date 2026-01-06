using System;

namespace LingoAppNet8.Models
{
    public class UserProgress
    {
        public int UserProgressId { get; set; }
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public bool IsCompleted { get; set; }
        public int Score { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? LastAttemptDate { get; set; }

        public virtual User User { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
}

