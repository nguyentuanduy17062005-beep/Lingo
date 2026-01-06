using System;

namespace LingoAppNet8.Models
{
    public class UserAchievement
    {
        public int UserAchievementId { get; set; }
        public int UserId { get; set; }
        public int AchievementId { get; set; }
        public DateTime UnlockedDate { get; set; }

        public virtual User User { get; set; }
        public virtual Achievement Achievement { get; set; }
    }
}

