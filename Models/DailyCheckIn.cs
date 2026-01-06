using System;

namespace LingoAppNet8.Models
{
    public class DailyCheckIn
    {
        public int DailyCheckInId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public int XPEarned { get; set; }
        public int StreakCount { get; set; }

        public virtual User User { get; set; }
    }
}

