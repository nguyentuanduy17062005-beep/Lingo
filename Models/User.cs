using System;
using System.Collections.Generic;

namespace LingoAppNet8.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CurrentLevel { get; set; }
        public int TotalXP { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime? LastCheckInDate { get; set; }

        public virtual ICollection<UserProgress> UserProgresses { get; set; }
        public virtual ICollection<DailyCheckIn> DailyCheckIns { get; set; }
        public virtual ICollection<UserAchievement> UserAchievements { get; set; }
    }
}

