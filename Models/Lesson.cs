using System.Collections.Generic;

namespace LingoAppNet8.Models
{
    public class Lesson
    {
        public int LessonId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int XPReward { get; set; }
        public string Content { get; set; }

        public virtual ICollection<UserProgress> UserProgresses { get; set; }
    }
}

