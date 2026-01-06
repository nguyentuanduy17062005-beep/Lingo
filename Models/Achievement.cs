using System.Collections.Generic;

namespace LingoAppNet8.Models
{
    public class Achievement
    {
        public int AchievementId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int RequiredValue { get; set; }
        public string Type { get; set; }

        public virtual ICollection<UserAchievement> UserAchievements { get; set; }
    }
}

