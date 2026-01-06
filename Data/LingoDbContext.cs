using Microsoft.EntityFrameworkCore;
using LingoAppNet8.Models;

namespace LingoAppNet8.Data
{
    public class LingoDbContext : DbContext
    {
        public LingoDbContext(DbContextOptions<LingoDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }
        public DbSet<DailyCheckIn> DailyCheckIns { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<Vocabulary> Vocabularies { get; set; }
        public DbSet<GrammarRule> GrammarRules { get; set; }
        public DbSet<TenseData> TensesData { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<SpeakingSentence> SpeakingSentences { get; set; }
        public DbSet<SpeakingResult> SpeakingResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(e => e.UserId);

            modelBuilder.Entity<User>()
                .Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Lesson>()
                .HasKey(e => e.LessonId);

            modelBuilder.Entity<QuizQuestion>()
                .HasKey(e => e.QuestionId);

            modelBuilder.Entity<QuizResult>()
                .HasKey(e => e.QuizResultId);

            modelBuilder.Entity<TenseData>()
                .HasKey(e => e.TenseId);

            modelBuilder.Entity<SpeakingSentence>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<SpeakingResult>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<UserProgress>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserProgresses)
                .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UserProgress>()
                .HasOne(up => up.Lesson)
                .WithMany(l => l.UserProgresses)
                .HasForeignKey(up => up.LessonId);

            modelBuilder.Entity<DailyCheckIn>()
                .HasOne(dc => dc.User)
                .WithMany(u => u.DailyCheckIns)
                .HasForeignKey(dc => dc.UserId);

            modelBuilder.Entity<UserAchievement>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAchievements)
                .HasForeignKey(ua => ua.UserId);

            modelBuilder.Entity<UserAchievement>()
                .HasOne(ua => ua.Achievement)
                .WithMany(a => a.UserAchievements)
                .HasForeignKey(ua => ua.AchievementId);

            // Seed data
            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Lessons
            modelBuilder.Entity<Lesson>().HasData(
                new Lesson { LessonId = 1, Title = "Greetings", Description = "Learn basic greetings", Level = 1, XPReward = 10, Content = "Hello, Hi, Good morning" },
                new Lesson { LessonId = 2, Title = "Numbers 1-10", Description = "Learn numbers from 1 to 10", Level = 1, XPReward = 10, Content = "One, Two, Three..." },
                new Lesson { LessonId = 3, Title = "Colors", Description = "Learn basic colors", Level = 1, XPReward = 10, Content = "Red, Blue, Green..." },
                new Lesson { LessonId = 4, Title = "Family Members", Description = "Learn family vocabulary", Level = 2, XPReward = 15, Content = "Mother, Father, Sister..." },
                new Lesson { LessonId = 5, Title = "Days of Week", Description = "Learn days of the week", Level = 2, XPReward = 15, Content = "Monday, Tuesday..." }
            );

            // Seed Achievements
            modelBuilder.Entity<Achievement>().HasData(
                new Achievement { AchievementId = 1, Name = "First Steps", Description = "Complete your first lesson", Icon = "🎯", RequiredValue = 1, Type = "lessons" },
                new Achievement { AchievementId = 2, Name = "Week Warrior", Description = "Maintain a 7-day streak", Icon = "🔥", RequiredValue = 7, Type = "streak" },
                new Achievement { AchievementId = 3, Name = "XP Master", Description = "Earn 100 XP", Icon = "⭐", RequiredValue = 100, Type = "xp" },
                new Achievement { AchievementId = 4, Name = "Dedicated Learner", Description = "Complete 10 lessons", Icon = "📚", RequiredValue = 10, Type = "lessons" },
                new Achievement { AchievementId = 5, Name = "Streak Legend", Description = "Maintain a 30-day streak", Icon = "🏆", RequiredValue = 30, Type = "streak" }
            );

            // Seed Grammar Rules
            modelBuilder.Entity<GrammarRule>().HasData(
                new GrammarRule { GrammarRuleId = 1, Title = "Present Simple Tense", Description = "Thì hiện tại đơn diễn tả sự thật hiển nhiên, thói quen", Examples = "I go to school every day.\nShe likes coffee.", Category = "Tenses", Level = 1, CreatedDate = new DateTime(2026, 1, 1) },
                new GrammarRule { GrammarRuleId = 2, Title = "Present Continuous", Description = "Thì hiện tại tiếp diễn diễn tả hành động đang xảy ra", Examples = "I am studying English now.\nThey are playing football.", Category = "Tenses", Level = 1, CreatedDate = new DateTime(2026, 1, 1) },
                new GrammarRule { GrammarRuleId = 3, Title = "Past Simple Tense", Description = "Thì quá khứ đơn diễn tả hành động đã xảy ra", Examples = "I went to school yesterday.\nShe bought a book last week.", Category = "Tenses", Level = 2, CreatedDate = new DateTime(2026, 1, 1) }
            );

            // Seed Tenses Data
            SeedTensesData(modelBuilder);

            // Seed Quiz Questions
            SeedQuizQuestions(modelBuilder);
        }

        private void SeedTensesData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TenseData>().HasData(
                new TenseData
                {
                    TenseId = 1,
                    Name = "Present Simple",
                    VietnameseName = "Thì Hiện Tại Đơn",
                    Description = "Thì hiện tại đơn diễn tả một sự thật hiển nhiên, một thói quen hoặc hành động lặp đi lặp lại ở hiện tại.",
                    Structure = "Khẳng định: S + V(s/es)\nPhủ định: S + do/does + not + V\nNghi vấn: Do/Does + S + V?",
                    Usage = "1. Diễn tả sự thật hiển nhiên, chân lý\n2. Diễn tả thói quen, hành động lặp đi lặp lại\n3. Diễn tả khả năng\n4. Nói về lịch trình, thời gian biểu",
                    Examples = "- The sun rises in the east. (Mặt trời mọc ở phía đông)\n- I go to school every day. (Tôi đi học mỗi ngày)\n- She plays tennis on weekends. (Cô ấy chơi tennis vào cuối tuần)\n- The train leaves at 9 AM. (Tàu khởi hành lúc 9 giờ sáng)",
                    TimeMarkers = "always, usually, often, sometimes, seldom, rarely, never, every day/week/month, once a week, twice a month",
                    Level = 1
                },
                new TenseData
                {
                    TenseId = 2,
                    Name = "Present Continuous",
                    VietnameseName = "Thì Hiện Tại Tiếp Diễn",
                    Description = "Thì hiện tại tiếp diễn diễn tả một hành động đang xảy ra tại thời điểm nói hoặc xung quanh thời điểm nói.",
                    Structure = "Khẳng định: S + am/is/are + V-ing\nPhủ định: S + am/is/are + not + V-ing\nNghi vấn: Am/Is/Are + S + V-ing?",
                    Usage = "1. Hành động đang xảy ra tại thời điểm nói\n2. Hành động xảy ra xung quanh thời điểm nói\n3. Kế hoạch trong tương lai gần\n4. Phàn nàn với 'always'",
                    Examples = "- I am studying English now. (Tôi đang học tiếng Anh bây giờ)\n- She is working on a project. (Cô ấy đang làm dự án)\n- They are playing football. (Họ đang chơi bóng đá)\n- We are meeting tomorrow. (Chúng tôi sẽ gặp nhau ngày mai)",
                    TimeMarkers = "now, right now, at the moment, at present, currently, today, this week/month",
                    Level = 1
                },
                new TenseData
                {
                    TenseId = 3,
                    Name = "Present Perfect",
                    VietnameseName = "Thì Hiện Tại Hoàn Thành",
                    Description = "Thì hiện tại hoàn thành diễn tả một hành động đã hoàn thành cho đến thời điểm hiện tại mà không đề cập đến thời gian cụ thể.",
                    Structure = "Khẳng định: S + have/has + V3/ed\nPhủ định: S + have/has + not + V3/ed\nNghi vấn: Have/Has + S + V3/ed?",
                    Usage = "1. Hành động đã hoàn thành nhưng không rõ thời gian\n2. Hành động bắt đầu trong quá khứ, kéo dài đến hiện tại\n3. Kinh nghiệm sống\n4. Hành động vừa mới xảy ra",
                    Examples = "- I have finished my homework. (Tôi đã hoàn thành bài tập)\n- She has lived here for 5 years. (Cô ấy đã sống ở đây 5 năm)\n- Have you ever been to Japan? (Bạn đã từng đến Nhật Bản chưa?)\n- They have just left. (Họ vừa mới rời đi)",
                    TimeMarkers = "already, yet, just, ever, never, recently, lately, so far, up to now, since, for",
                    Level = 2
                },
                new TenseData
                {
                    TenseId = 4,
                    Name = "Present Perfect Continuous",
                    VietnameseName = "Thì Hiện Tại Hoàn Thành Tiếp Diễn",
                    Description = "Thì hiện tại hoàn thành tiếp diễn nhấn mạnh tính liên tục của hành động bắt đầu trong quá khứ và vẫn đang tiếp tục.",
                    Structure = "Khẳng định: S + have/has + been + V-ing\nPhủ định: S + have/has + not + been + V-ing\nNghi vấn: Have/Has + S + been + V-ing?",
                    Usage = "1. Hành động bắt đầu trong quá khứ và vẫn đang tiếp tục\n2. Nhấn mạnh tính liên tục của hành động\n3. Hành động vừa mới kết thúc và có kết quả ở hiện tại",
                    Examples = "- I have been studying for 3 hours. (Tôi đã học được 3 tiếng rồi)\n- She has been working here since 2020. (Cô ấy đã làm việc ở đây từ 2020)\n- They have been waiting for you. (Họ đã đợi bạn)\n- It has been raining all day. (Trời mưa cả ngày rồi)",
                    TimeMarkers = "for, since, all day/week/month, how long",
                    Level = 2
                },
                new TenseData
                {
                    TenseId = 5,
                    Name = "Past Simple",
                    VietnameseName = "Thì Quá Khứ Đơn",
                    Description = "Thì quá khứ đơn diễn tả một hành động đã xảy ra và kết thúc hoàn toàn trong quá khứ.",
                    Structure = "Khẳng định: S + V2/ed\nPhủ định: S + did + not + V\nNghi vấn: Did + S + V?",
                    Usage = "1. Hành động đã hoàn tất trong quá khứ\n2. Chuỗi hành động trong quá khứ\n3. Thói quen trong quá khứ (với 'used to')",
                    Examples = "- I went to school yesterday. (Tôi đã đi học hôm qua)\n- She studied English last night. (Cô ấy học tiếng Anh tối qua)\n- They visited Paris in 2020. (Họ đã thăm Paris năm 2020)\n- He didn't come to the party. (Anh ấy không đến bữa tiệc)",
                    TimeMarkers = "yesterday, last week/month/year, ago, in + năm trong quá khứ, when, used to",
                    Level = 1
                },
                new TenseData
                {
                    TenseId = 6,
                    Name = "Past Continuous",
                    VietnameseName = "Thì Quá Khứ Tiếp Diễn",
                    Description = "Thì quá khứ tiếp diễn diễn tả một hành động đang xảy ra tại một thời điểm cụ thể trong quá khứ.",
                    Structure = "Khẳng định: S + was/were + V-ing\nPhủ định: S + was/were + not + V-ing\nNghi vấn: Was/Were + S + V-ing?",
                    Usage = "1. Hành động đang xảy ra tại thời điểm xác định trong quá khứ\n2. Hai hành động xảy ra đồng thời trong quá khứ\n3. Hành động đang xảy ra thì có hành động khác xen vào",
                    Examples = "- I was studying at 8 PM yesterday. (Tôi đang học lúc 8 giờ tối qua)\n- She was cooking when I called. (Cô ấy đang nấu ăn khi tôi gọi)\n- They were playing while we were working. (Họ đang chơi trong khi chúng tôi đang làm việc)\n- What were you doing at that time? (Bạn đang làm gì vào lúc đó?)",
                    TimeMarkers = "at + giờ + thời gian trong quá khứ, when, while, as",
                    Level = 2
                },
                new TenseData
                {
                    TenseId = 7,
                    Name = "Past Perfect",
                    VietnameseName = "Thì Quá Khứ Hoàn Thành",
                    Description = "Thì quá khứ hoàn thành diễn tả một hành động xảy ra trước một hành động khác trong quá khứ.",
                    Structure = "Khẳng định: S + had + V3/ed\nPhủ định: S + had + not + V3/ed\nNghi vấn: Had + S + V3/ed?",
                    Usage = "1. Hành động xảy ra trước hành động khác trong quá khứ\n2. Hành động hoàn thành trước thời điểm xác định trong quá khứ\n3. Câu điều kiện loại 3",
                    Examples = "- I had finished homework before she came. (Tôi đã hoàn thành bài tập trước khi cô ấy đến)\n- They had left when we arrived. (Họ đã rời đi khi chúng tôi đến)\n- She had never seen snow before she moved to Canada. (Cô ấy chưa từng thấy tuyết trước khi chuyển đến Canada)\n- If I had known, I would have told you. (Nếu tôi biết, tôi đã nói với bạn)",
                    TimeMarkers = "before, after, when, by the time, already, just, never, until",
                    Level = 3
                },
                new TenseData
                {
                    TenseId = 8,
                    Name = "Past Perfect Continuous",
                    VietnameseName = "Thì Quá Khứ Hoàn Thành Tiếp Diễn",
                    Description = "Thì quá khứ hoàn thành tiếp diễn nhấn mạnh tính liên tục của hành động xảy ra trước một thời điểm/hành động khác trong quá khứ.",
                    Structure = "Khẳng định: S + had + been + V-ing\nPhủ định: S + had + not + been + V-ing\nNghi vấn: Had + S + been + V-ing?",
                    Usage = "1. Hành động kéo dài liên tục trước thời điểm trong quá khứ\n2. Nhấn mạnh quá trình của hành động",
                    Examples = "- I had been waiting for 2 hours before he came. (Tôi đã đợi 2 tiếng trước khi anh ấy đến)\n- She had been studying all day. (Cô ấy đã học cả ngày)\n- They had been living there for 10 years. (Họ đã sống ở đó 10 năm)",
                    TimeMarkers = "for, since, before, by the time, until",
                    Level = 3
                },
                new TenseData
                {
                    TenseId = 9,
                    Name = "Future Simple",
                    VietnameseName = "Thì Tương Lai Đơn",
                    Description = "Thì tương lai đơn diễn tả một hành động sẽ xảy ra trong tương lai.",
                    Structure = "Khẳng định: S + will + V\nPhủ định: S + will + not (won't) + V\nNghi vấn: Will + S + V?",
                    Usage = "1. Quyết định tức thời tại thời điểm nói\n2. Dự đoán không có căn cứ\n3. Lời hứa, đề nghị\n4. Sự việc chắc chắn xảy ra trong tương lai",
                    Examples = "- I will help you. (Tôi sẽ giúp bạn)\n- It will rain tomorrow. (Ngày mai trời sẽ mưa)\n- She will be 20 next year. (Năm sau cô ấy sẽ 20 tuổi)\n- Will you marry me? (Bạn sẽ lấy tôi chứ?)",
                    TimeMarkers = "tomorrow, next week/month/year, in the future, soon, someday",
                    Level = 1
                },
                new TenseData
                {
                    TenseId = 10,
                    Name = "Future Continuous",
                    VietnameseName = "Thì Tương Lai Tiếp Diễn",
                    Description = "Thì tương lai tiếp diễn diễn tả một hành động sẽ đang xảy ra tại một thời điểm xác định trong tương lai.",
                    Structure = "Khẳng định: S + will + be + V-ing\nPhủ định: S + will + not + be + V-ing\nNghi vấn: Will + S + be + V-ing?",
                    Usage = "1. Hành động đang xảy ra tại thời điểm xác định trong tương lai\n2. Hành động sẽ xảy ra như một phần của kế hoạch",
                    Examples = "- I will be studying at 8 PM tomorrow. (Tôi sẽ đang học lúc 8 giờ tối mai)\n- She will be waiting for you. (Cô ấy sẽ đang đợi bạn)\n- They will be traveling next week. (Tuần sau họ sẽ đang đi du lịch)",
                    TimeMarkers = "at this time tomorrow, at + giờ + thời gian tương lai, next week/month",
                    Level = 2
                },
                new TenseData
                {
                    TenseId = 11,
                    Name = "Future Perfect",
                    VietnameseName = "Thì Tương Lai Hoàn Thành",
                    Description = "Thì tương lai hoàn thành diễn tả một hành động sẽ hoàn thành trước một thời điểm/hành động khác trong tương lai.",
                    Structure = "Khẳng định: S + will + have + V3/ed\nPhủ định: S + will + not + have + V3/ed\nNghi vấn: Will + S + have + V3/ed?",
                    Usage = "1. Hành động sẽ hoàn thành trước thời điểm trong tương lai\n2. Hành động sẽ hoàn thành trước hành động khác trong tương lai",
                    Examples = "- I will have finished by 6 PM. (Tôi sẽ hoàn thành trước 6 giờ chiều)\n- She will have graduated by next year. (Năm sau cô ấy sẽ tốt nghiệp)\n- They will have left before you arrive. (Họ sẽ rời đi trước khi bạn đến)",
                    TimeMarkers = "by, by the time, before, by next week/month/year",
                    Level = 3
                },
                new TenseData
                {
                    TenseId = 12,
                    Name = "Future Perfect Continuous",
                    VietnameseName = "Thì Tương Lai Hoàn Thành Tiếp Diễn",
                    Description = "Thì tương lai hoàn thành tiếp diễn nhấn mạnh tính liên tục của hành động sẽ hoàn thành trước một thời điểm trong tương lai.",
                    Structure = "Khẳng định: S + will + have + been + V-ing\nPhủ định: S + will + not + have + been + V-ing\nNghi vấn: Will + S + have + been + V-ing?",
                    Usage = "1. Hành động liên tục hoàn thành trước thời điểm tương lai\n2. Nhấn mạnh quá trình của hành động",
                    Examples = "- By 2025, I will have been working here for 10 years. (Đến 2025, tôi sẽ làm việc ở đây được 10 năm)\n- She will have been studying for 5 hours by then. (Lúc đó cô ấy sẽ học được 5 tiếng)\n- They will have been living there for a decade. (Họ sẽ sống ở đó được một thập kỷ)",
                    TimeMarkers = "by, by the time, for, by next week/month/year",
                    Level = 3
                }
            );
        }

        private void SeedQuizQuestions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuizQuestion>().HasData(
                // Present Simple Questions
                new QuizQuestion { QuestionId = 1, Question = "She _____ to school every day.", OptionA = "go", OptionB = "goes", OptionC = "going", OptionD = "gone", CorrectAnswer = "B", Difficulty = "Easy", TimeLimit = 10, TenseId = 1 },
                new QuizQuestion { QuestionId = 2, Question = "They _____ football on weekends.", OptionA = "play", OptionB = "plays", OptionC = "playing", OptionD = "played", CorrectAnswer = "A", Difficulty = "Easy", TimeLimit = 10, TenseId = 1 },
                new QuizQuestion { QuestionId = 3, Question = "The sun _____ in the east.", OptionA = "rise", OptionB = "rises", OptionC = "rising", OptionD = "risen", CorrectAnswer = "B", Difficulty = "Normal", TimeLimit = 15, TenseId = 1 },
                
                // Present Continuous Questions
                new QuizQuestion { QuestionId = 4, Question = "I _____ English now.", OptionA = "study", OptionB = "studies", OptionC = "am studying", OptionD = "studied", CorrectAnswer = "C", Difficulty = "Normal", TimeLimit = 15, TenseId = 2 },
                new QuizQuestion { QuestionId = 5, Question = "They _____ football at the moment.", OptionA = "play", OptionB = "are playing", OptionC = "played", OptionD = "plays", CorrectAnswer = "B", Difficulty = "Easy", TimeLimit = 10, TenseId = 2 },
                
                // Present Perfect Questions
                new QuizQuestion { QuestionId = 6, Question = "I _____ my homework.", OptionA = "have finished", OptionB = "has finished", OptionC = "finish", OptionD = "finished", CorrectAnswer = "A", Difficulty = "Normal", TimeLimit = 15, TenseId = 3 },
                new QuizQuestion { QuestionId = 7, Question = "She _____ here for 5 years.", OptionA = "live", OptionB = "has lived", OptionC = "have lived", OptionD = "living", CorrectAnswer = "B", Difficulty = "Hard", TimeLimit = 20, TenseId = 3 },
                
                // Past Simple Questions
                new QuizQuestion { QuestionId = 8, Question = "I _____ to the park yesterday.", OptionA = "go", OptionB = "goes", OptionC = "went", OptionD = "going", CorrectAnswer = "C", Difficulty = "Easy", TimeLimit = 10, TenseId = 5 },
                new QuizQuestion { QuestionId = 9, Question = "They _____ the movie last night.", OptionA = "watch", OptionB = "watched", OptionC = "watching", OptionD = "watches", CorrectAnswer = "B", Difficulty = "Normal", TimeLimit = 15, TenseId = 5 },
                
                // Past Continuous Questions
                new QuizQuestion { QuestionId = 10, Question = "I _____ TV when she called.", OptionA = "watch", OptionB = "watched", OptionC = "was watching", OptionD = "am watching", CorrectAnswer = "C", Difficulty = "Normal", TimeLimit = 15, TenseId = 6 },
                
                // Future Simple Questions
                new QuizQuestion { QuestionId = 11, Question = "I _____ help you tomorrow.", OptionA = "will", OptionB = "shall", OptionC = "would", OptionD = "can", CorrectAnswer = "A", Difficulty = "Easy", TimeLimit = 10, TenseId = 9 },
                new QuizQuestion { QuestionId = 12, Question = "It _____ rain tomorrow.", OptionA = "is", OptionB = "will", OptionC = "was", OptionD = "would", CorrectAnswer = "B", Difficulty = "Normal", TimeLimit = 15, TenseId = 9 },
                
                // Past Perfect Questions
                new QuizQuestion { QuestionId = 13, Question = "She _____ before I arrived.", OptionA = "had left", OptionB = "has left", OptionC = "left", OptionD = "leave", CorrectAnswer = "A", Difficulty = "Hard", TimeLimit = 20, TenseId = 7 },
                
                // Mixed Questions
                new QuizQuestion { QuestionId = 14, Question = "By next year, I _____ my degree.", OptionA = "will finish", OptionB = "will have finished", OptionC = "finish", OptionD = "finished", CorrectAnswer = "B", Difficulty = "Hard", TimeLimit = 20, TenseId = 11 },
                new QuizQuestion { QuestionId = 15, Question = "He _____ in this company since 2010.", OptionA = "works", OptionB = "worked", OptionC = "has worked", OptionD = "have worked", CorrectAnswer = "C", Difficulty = "Normal", TimeLimit = 15, TenseId = 3 }
            );
        }
    }
}
