using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LingoAppNet8.Models
{
    public class UserSettings
    {
        public UserSettings()
        {
            AvatarPath = "default.png";
            NotificationsEnabled = true;
            SoundEnabled = true;
            TTSSpeed = 0;
            TTSVolume = 100;
            ThemeMode = "Light";
        }

        [Key]
        public int SettingsId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public string AvatarPath { get; set; }

        public bool NotificationsEnabled { get; set; }

        public bool SoundEnabled { get; set; }

        public int TTSSpeed { get; set; } // -10 to 10

        public int TTSVolume { get; set; } // 0 to 100

        public string ThemeMode { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}

