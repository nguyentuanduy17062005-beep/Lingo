using LingoAppNet8.Data;
using LingoAppNet8.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing.Drawing2D;

namespace LingoAppNet8.Forms
{
    // Custom clickable panel
    public class ClickablePanel : Panel
    {
        public void RaiseClick()
        {
            OnClick(EventArgs.Empty);
        }
    }

    // Modern Sidebar Button with vertical indicator
    public class ModernSidebarButton : Panel
    {
        private Label lblText;
        private Panel indicatorBar;
        private Color accentColor;
        private bool isHovered = false;
        
        public ModernSidebarButton(string text, Color accent)
        {
            accentColor = accent;
            
            this.Size = new Size(240, 55);
            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;
            
            // Vertical indicator bar on left
            indicatorBar = new Panel
            {
                Width = 4,
                Height = this.Height,
                Location = new Point(0, 0),
                BackColor = accentColor,
                Visible = false
            };
            
            // Text label
            lblText = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = false,
                Size = new Size(200, this.Height),
                Location = new Point(20, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            
            this.Controls.Add(indicatorBar);
            this.Controls.Add(lblText);
            
            // Events
            this.MouseEnter += OnMouseEnterHandler;
            this.MouseLeave += OnMouseLeaveHandler;
            lblText.MouseEnter += OnMouseEnterHandler;
            lblText.MouseLeave += OnMouseLeaveHandler;
            
            // Forward click from label to panel
            lblText.Click += (s, e) => this.OnClick(e);
        }
        
        private void OnMouseEnterHandler(object? sender, EventArgs e)
        {
            isHovered = true;
            this.BackColor = Color.FromArgb(237, 242, 247); // #EDF2F7
            lblText.ForeColor = accentColor;
            lblText.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            indicatorBar.Visible = true;
        }
        
        private void OnMouseLeaveHandler(object? sender, EventArgs e)
        {
            isHovered = false;
            this.BackColor = Color.Transparent;
            lblText.ForeColor = Color.FromArgb(100, 100, 100);
            lblText.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            indicatorBar.Visible = false;
        }
        
        public new string Text
        {
            get => lblText.Text;
            set => lblText.Text = value;
        }
    }

    public partial class MainForm : Form
    {
        private User currentUser;
        private LingoDbContext dbContext;
        
        // Modern Professional UI components
        private Panel sidebarPanel = null!;
        private TableLayoutPanel mainContentPanel = null!;
        private Panel topBarPanel = null!;
        private Label lblUserName = null!;
        private Label lblStreak = null!;
        private Label lblXP = null!;
        private ProgressBar progressBar = null!;
        
        // Modern Professional Color Palette
        private readonly Color PrimaryIndigo = Color.FromArgb(46, 49, 146);        // Deep Indigo #2E3192
        private readonly Color PrimaryCyan = Color.FromArgb(27, 255, 255);         // Bright Cyan #1BFFFF
        private readonly Color PrimaryBlue = Color.FromArgb(41, 128, 185);         // Ocean Blue
        private readonly Color AccentOrange = Color.FromArgb(255, 126, 95);        // Coral Orange #FF7E5F
        private readonly Color BackgroundGray = Color.FromArgb(244, 247, 246);     // Light Gray #F4F7F6
        private readonly Color CardWhite = Color.FromArgb(255, 255, 255);          // Pure White #FFFFFF
        private readonly Color SidebarColor = Color.FromArgb(255, 255, 255);       // White Sidebar
        private readonly Color CardShadow = Color.FromArgb(20, 0, 0, 0);          // Subtle Shadow
        private readonly Color TextDark = Color.FromArgb(40, 40, 40);             // Dark Text
        private readonly Color TextGray = Color.FromArgb(100, 100, 100);          // Gray Text
        private readonly Color TextLight = Color.FromArgb(150, 150, 150);         // Light Gray Text
        
        // Secondary colors for variety
        private readonly Color SecondaryPurple = Color.FromArgb(142, 68, 173);    // Professional Purple
        private readonly Color SecondaryGreen = Color.FromArgb(39, 174, 96);      // Success Green
        private readonly Color SecondaryYellow = Color.FromArgb(241, 196, 15);    // Warning Yellow

        public MainForm(User user)
        {
            currentUser = user;
            var scope = Program.ServiceProvider!.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<LingoDbContext>();
            
            InitializeComponent();
            LoadUserInfo();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = "LingoApp - Học Tiếng Anh";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BackgroundGray;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1200, 700);

            // Create Duolingo-style layout
            CreateTopBar();
            CreateSidebar();
            CreateMainContent();

            // Add panels to form (order matters for docking)
            this.Controls.Add(mainContentPanel);
            this.Controls.Add(sidebarPanel);
            this.Controls.Add(topBarPanel);
            
            this.ResumeLayout(false);
        }

        private void CreateTopBar()
        {
            topBarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70
            };
            
            // Paint gradient background
            topBarPanel.Paint += (s, e) =>
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                
                // Draw gradient from Indigo to Cyan
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    topBarPanel.ClientRectangle,
                    PrimaryIndigo,
                    PrimaryCyan,
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, topBarPanel.ClientRectangle);
                }
            };

            var lblLogo = new Label
            {
                Text = "💼 LINGO APP",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(280, 18),
                BackColor = Color.Transparent
            };

            // Streak badge with modern style
            var streakPanel = new Panel
            {
                Size = new Size(100, 35),
                Location = new Point(520, 18),
                BackColor = Color.FromArgb(255, 255, 255, 40) // Semi-transparent white
            };
            streakPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255, 40)))
                {
                    Rectangle rect = new Rectangle(0, 0, streakPanel.Width - 1, streakPanel.Height - 1);
                    e.Graphics.FillRoundedRectangle(brush, rect, 8);
                }
            };
            lblStreak = new Label
            {
                Text = $"🔥 {currentUser.CurrentStreak}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(12, 8),
                BackColor = Color.Transparent
            };
            streakPanel.Controls.Add(lblStreak);

            // XP badge with modern style
            var xpPanel = new Panel
            {
                Size = new Size(120, 35),
                Location = new Point(640, 18),
                BackColor = Color.Transparent
            };
            xpPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255, 40)))
                {
                    Rectangle rect = new Rectangle(0, 0, xpPanel.Width - 1, xpPanel.Height - 1);
                    e.Graphics.FillRoundedRectangle(brush, rect, 8);
                }
            };
            lblXP = new Label
            {
                Text = $"⭐ {currentUser.TotalXP} XP",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(12, 8),
                BackColor = Color.Transparent
            };
            xpPanel.Controls.Add(lblXP);

            var btnProfile = new Button
            {
                Text = $"👤 {currentUser.Username}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = TextDark,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 40),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnProfile.Location = new Point(this.ClientSize.Width - 220, 15);
            btnProfile.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnProfile.FlatAppearance.BorderSize = 1;
            btnProfile.Click += (s, e) =>
            {
                MessageBox.Show($"👤 {currentUser.Username}\n📧 {currentUser.Email}\n🏆 Level {currentUser.CurrentLevel}\n⭐ {currentUser.TotalXP} XP",
                    "Thông tin tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            topBarPanel.Controls.AddRange(new Control[] { lblLogo, streakPanel, xpPanel, btnProfile });
        }

        private void CreateSidebar()
        {
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = SidebarColor
            };
            
            sidebarPanel.Paint += (s, e) =>
            {
                // Right shadow - subtle
                using (var shadowBrush = new LinearGradientBrush(
                    new Rectangle(sidebarPanel.Width - 6, 0, 6, sidebarPanel.Height),
                    Color.FromArgb(10, 0, 0, 0),
                    Color.FromArgb(0, 0, 0, 0),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(shadowBrush, sidebarPanel.Width - 6, 0, 6, sidebarPanel.Height);
                }
            };

            var yPos = 20;
            var buttonHeight = 55;
            var spacing = 15; // Increased spacing for cleaner look

            // Use outline style icons
            var btnLearn = CreateSidebarButton("📖 Học tập", PrimaryBlue, yPos);
            btnLearn.Click += (s, e) => ShowLearningContent();
            yPos += buttonHeight + spacing;

            var btnPractice = CreateSidebarButton("⚡ Luyện tập", SecondaryPurple, yPos);
            btnPractice.Click += (s, e) => ShowPracticeContent();
            yPos += buttonHeight + spacing;

            var btnReview = CreateSidebarButton("↻ Ôn tập", SecondaryGreen, yPos);
            btnReview.Click += (s, e) => ShowReviewContent();
            yPos += buttonHeight + spacing;

            var btnCheckIn = CreateSidebarButton("☐ Điểm danh", SecondaryYellow, yPos);
            btnCheckIn.Click += (s, e) =>
            {
                this.Cursor = Cursors.WaitCursor;
                var checkInForm = new DailyCheckInForm(currentUser, dbContext);
                this.Cursor = Cursors.Default;
                checkInForm.ShowDialog();
                LoadUserInfo();
            };
            yPos += buttonHeight + spacing;

            var btnTools = CreateSidebarButton("⚙ Công cụ", Color.FromArgb(120, 120, 120), yPos);
            btnTools.Click += (s, e) => ShowToolsMenu();
            yPos += buttonHeight + spacing;

            var btnAchievements = CreateSidebarButton("♦ Thành tích", AccentOrange, yPos);
            btnAchievements.Click += (s, e) =>
            {
                MessageBox.Show($"🏆 Thành tích của bạn:\n\n⭐ XP: {currentUser.TotalXP}\n🔥 Chuỗi: {currentUser.CurrentStreak} ngày\n📊 Level: {currentUser.CurrentLevel}",
                    "Thành tích", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            sidebarPanel.Controls.AddRange(new Control[] { btnLearn, btnPractice, btnReview, btnCheckIn, btnTools, btnAchievements });

            sidebarPanel.Paint += (s, e) =>
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(230, 230, 230), 2), 
                    sidebarPanel.Width - 2, 0, sidebarPanel.Width - 2, sidebarPanel.Height);
            };
        }

        private ModernSidebarButton CreateSidebarButton(string text, Color accentColor, int yPosition)
        {
            var btn = new ModernSidebarButton(text, accentColor)
            {
                Location = new Point(10, yPosition)
            };
            
            return btn;
        }

        private void CreateMainContent()
        {
            mainContentPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = BackgroundGray,
                AutoScroll = true,
                Padding = new Padding(40, 30, 40, 30),
                ColumnCount = 1,
                RowCount = 3,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            
            mainContentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainContentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainContentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainContentPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            ShowLearningContent();
        }

        private void ShowLearningContent()
        {
            mainContentPanel.SuspendLayout();
            mainContentPanel.Controls.Clear();
            mainContentPanel.RowStyles.Clear();
            
            // Title
            var lblTitle = new Label
            {
                Text = "🎓 Con đường học tập của bạn",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };

            // Progress panel - đơn giản hóa
            var progressPanel = new Panel
            {
                Width = 950,
                Height = 90,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 20)
            };
            
            var lblProgress = new Label
            {
                Text = $"Level {currentUser.CurrentLevel}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = PrimaryBlue,
                AutoSize = true,
                Location = new Point(20, 15),
                BackColor = Color.Transparent
            };

            var simpleProgressBar = new ProgressBar
            {
                Size = new Size(900, 30),
                Location = new Point(20, 50),
                Value = Math.Min(currentUser.TotalXP % 100, 100),
                Maximum = 100,
                ForeColor = PrimaryBlue,
                Style = ProgressBarStyle.Continuous
            };

            var lblXPInfo = new Label
            {
                Text = $"{currentUser.TotalXP % 100}/100 XP",
                Font = new Font("Segoe UI", 11),
                ForeColor = TextGray,
                AutoSize = true,
                Location = new Point(850, 20),
                BackColor = Color.Transparent
            };

            progressPanel.Controls.AddRange(new Control[] { lblProgress, simpleProgressBar, lblXPInfo });

            // Cards layout với nhiều nội dung hơn
            var cardsFlow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0),
                Width = 950
            };

            // Học tập cards
            var quizCard = CreateLearningCard("📝 Bài Kiểm Tra", "10 câu hỏi tổng hợp", SecondaryPurple, 10);
            quizCard.Click += QuizCard_Click;

            var tensesCard = CreateLearningCard("📖 12 Thì", "Ngữ pháp tiếng Anh", PrimaryBlue, 15);
            tensesCard.Click += TensesCard_Click;

            var speakingCard = CreateLearningCard("🎤 Speaking", "Luyện phát âm chuẩn", AccentOrange, 20);
            speakingCard.Click += SpeakingCard_Click;

            cardsFlow.Controls.AddRange(new Control[] { quizCard, tensesCard, speakingCard });

            // Tips section
            var lblTips = new Label
            {
                Text = "💡 Mẹo học tập hiệu quả",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Margin = new Padding(0, 20, 0, 15)
            };

            var tipsFlow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0, 0, 0, 20),
                Width = 950
            };

            var tip1 = CreateInfoCard("⏰ Học đều đặn", "Học 15-20 phút mỗi ngày hiệu quả hơn học 2 tiếng 1 lần", SecondaryGreen);
            var tip2 = CreateInfoCard("🎯 Đặt mục tiêu", "Đặt mục tiêu nhỏ, cụ thể để dễ theo dõi tiến độ", PrimaryBlue);
            var tip3 = CreateInfoCard("🔊 Nghe & Nói", "Luyện nghe và nói mỗi ngày để cải thiện phát âm", AccentOrange);

            tipsFlow.Controls.AddRange(new Control[] { tip1, tip2, tip3 });

            // Quote of the day with modern gradient
            var quotePanel = new Panel
            {
                Width = 950,
                Height = 100,
                BackColor = CardWhite,
                Margin = new Padding(0, 0, 0, 20)
            };
            quotePanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                // Gradient background
                using (var brush = new LinearGradientBrush(
                    quotePanel.ClientRectangle,
                    Color.FromArgb(240, 242, 255),
                    Color.FromArgb(255, 240, 245),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, quotePanel.ClientRectangle);
                }
            };

            var lblQuote = new Label
            {
                Text = "\"Ngôn ngữ là chìa khóa mở cửa thế giới\" 🌍",
                Font = new Font("Segoe UI", 16, FontStyle.Italic),
                ForeColor = PrimaryIndigo,
                AutoSize = false,
                Size = new Size(900, 40),
                Location = new Point(25, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            var lblQuoteAuthor = new Label
            {
                Text = "- Frank Smith",
                Font = new Font("Segoe UI", 12),
                ForeColor = TextGray,
                AutoSize = true,
                Location = new Point(780, 65),
                BackColor = Color.Transparent
            };

            quotePanel.Controls.AddRange(new Control[] { lblQuote, lblQuoteAuthor });

            // Quick Stats
            var statsFlow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0),
                Width = 950
            };

            var stat1 = CreateStatCard("🔥", $"{currentUser.CurrentStreak} ngày", "Chuỗi học liên tục", AccentOrange);
            var stat2 = CreateStatCard("⭐", $"{currentUser.TotalXP} XP", "Tổng điểm kinh nghiệm", SecondaryYellow);
            var stat3 = CreateStatCard("🏆", $"Level {currentUser.CurrentLevel}", "Cấp độ hiện tại", SecondaryGreen);

            statsFlow.Controls.AddRange(new Control[] { stat1, stat2, stat3 });

            cardsFlow.Controls.AddRange(new Control[] { quizCard, tensesCard, speakingCard });

            mainContentPanel.Controls.Add(lblTitle, 0, 0);
            mainContentPanel.Controls.Add(progressPanel, 0, 1);
            mainContentPanel.Controls.Add(cardsFlow, 0, 2);
            mainContentPanel.Controls.Add(lblTips, 0, 3);
            mainContentPanel.Controls.Add(tipsFlow, 0, 4);
            mainContentPanel.Controls.Add(quotePanel, 0, 5);
            mainContentPanel.Controls.Add(statsFlow, 0, 6);
            
            mainContentPanel.ResumeLayout();
        }

        // Helper method tạo info card
        private Panel CreateInfoCard(string title, string content, Color bgColor)
        {
            var card = new Panel
            {
                Size = new Size(300, 140),
                BackColor = bgColor,
                Margin = new Padding(0, 0, 15, 15)
            };

            // Use TableLayoutPanel for responsive layout
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                RowCount = 2,
                ColumnCount = 1,
                Padding = new Padding(15, 10, 15, 10)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // Title
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Content
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 5)
            };

            var lblContent = new Label
            {
                Text = content,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(240, 240, 240),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                AutoEllipsis = true,
                BackColor = Color.Transparent
            };

            layout.Controls.Add(lblTitle, 0, 0);
            layout.Controls.Add(lblContent, 0, 1);
            card.Controls.Add(layout);
            return card;
        }

        // Helper method tạo stat card
        private Panel CreateStatCard(string icon, string value, string label, Color accentColor)
        {
            var card = new Panel
            {
                Size = new Size(300, 110),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 15, 15)
            };

            // Use TableLayoutPanel for responsive layout
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                RowCount = 1,
                ColumnCount = 2,
                Padding = new Padding(15)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F)); // Icon column
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Text column
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 32),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Text container for value and label
            var textPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                RowCount = 2,
                ColumnCount = 1
            };
            textPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            textPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            textPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = accentColor,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent
            };

            var lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10),
                ForeColor = TextGray,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.Transparent
            };

            textPanel.Controls.Add(lblValue, 0, 0);
            textPanel.Controls.Add(lblLabel, 0, 1);
            
            layout.Controls.Add(lblIcon, 0, 0);
            layout.Controls.Add(textPanel, 1, 0);
            card.Controls.Add(layout);
            return card;
        }

        private void QuizCard_Click(object? sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                var quizForm = new QuizForm(currentUser, dbContext);
                this.Cursor = Cursors.Default;
                quizForm.ShowDialog();
                LoadUserInfo();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void TensesCard_Click(object? sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                var tensesForm = new TensesForm(dbContext);
                this.Cursor = Cursors.Default;
                tensesForm.ShowDialog();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void SpeakingCard_Click(object? sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                var speakingForm = new SpeakingPracticeForm(dbContext, currentUser);
                this.Cursor = Cursors.Default;
                speakingForm.ShowDialog();
                LoadUserInfo();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void MemoryGameCard_Click(object? sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                var memoryGameForm = new MemoryGameForm();
                this.Cursor = Cursors.Default;
                memoryGameForm.ShowDialog();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private Panel CreateLearningCard(string title, string description, Color color, int xpReward)
        {
            var card = new ClickablePanel
            {
                Size = new Size(300, 200),
                BackColor = CardWhite,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 15, 15)
            };
            
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                
                // Draw subtle shadow
                Rectangle shadowRect = new Rectangle(4, 4, card.Width - 4, card.Height - 4);
                using (SolidBrush shadowBrush = new SolidBrush(CardShadow))
                {
                    e.Graphics.FillRoundedRectangle(shadowBrush, shadowRect, 12);
                }
                
                // Draw card background with rounded corners
                Rectangle cardRect = new Rectangle(0, 0, card.Width - 5, card.Height - 5);
                using (SolidBrush brush = new SolidBrush(CardWhite))
                {
                    e.Graphics.FillRoundedRectangle(brush, cardRect, 12);
                }
                
                // Draw subtle border
                using (Pen pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    e.Graphics.DrawRoundedRectangle(pen, cardRect, 12);
                }
            };

            // Add colored accent bar at top
            Panel accentBar = new Panel
            {
                Height = 4,
                Dock = DockStyle.Top,
                BackColor = color
            };
            card.Controls.Add(accentBar);

            // Use TableLayoutPanel for responsive layout
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                RowCount = 4,
                ColumnCount = 1,
                Padding = new Padding(15, 10, 15, 10)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));  // Icon
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // Title
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));   // Description
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));   // XP
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var iconLabel = new Label
            {
                Text = title.Substring(0, 2),
                Font = new Font("Segoe UI", 40, FontStyle.Regular),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                ForeColor = color
            };

            var titleLabel = new Label
            {
                Text = title.Substring(3),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Padding = new Padding(5, 0, 5, 0)
            };

            var descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 9),
                ForeColor = TextGray,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopCenter,
                AutoEllipsis = true,
                BackColor = Color.Transparent,
                Padding = new Padding(5)
            };

            var xpLabel = new Label
            {
                Text = xpReward > 0 ? $"+{xpReward} XP" : "",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = color,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            layout.Controls.Add(iconLabel, 0, 0);
            layout.Controls.Add(titleLabel, 0, 1);
            layout.Controls.Add(descLabel, 0, 2);
            layout.Controls.Add(xpLabel, 0, 3);

            card.Controls.Add(layout);

            // Mouse hover effects - subtle elevation
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(250, 250, 250);
                card.Invalidate();
            };
            card.MouseLeave += (s, e) =>
            {
                card.BackColor = CardWhite;
                card.Invalidate();
            };

            // Forward events from child controls
            void SetupChildEvents(Control ctrl)
            {
                ctrl.MouseEnter += (s, e) =>
                {
                    card.BackColor = Color.FromArgb(250, 250, 250);
                    card.Invalidate();
                };
                ctrl.MouseLeave += (s, e) =>
                {
                    card.BackColor = CardWhite;
                    card.Invalidate();
                };
                ctrl.Cursor = Cursors.Hand;
                
                // Forward click to parent card
                ctrl.Click += (s, e) =>
                {
                    card.RaiseClick();
                };
                
                foreach (Control child in ctrl.Controls)
                {
                    SetupChildEvents(child);
                }
            }
            
            SetupChildEvents(layout);

            return card;
        }

        private void ShowPracticeContent()
        {
            mainContentPanel.SuspendLayout();
            mainContentPanel.Controls.Clear();

            var lblTitle = new Label
            {
                Text = "🎯 Luyện tập",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            var lblDesc = new Label
            {
                Text = "Chọn chủ đề để luyện tập thêm",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(120, 120, 120),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            var cardsFlow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight
            };

            var practiceCard1 = CreateLearningCard("📝 Quiz Nhanh", "10 câu hỏi ngẫu nhiên", SecondaryPurple, 15);
            practiceCard1.Click += QuizCard_Click;

            var practiceCard2 = CreateLearningCard("🎤 Speaking", "Luyện phát âm", AccentOrange, 20);
            practiceCard2.Click += SpeakingCard_Click;

            var practiceCard3 = CreateLearningCard("🎮 Trò Chơi Ghi Nhớ", "Tìm cặp từ tiếng Anh - Việt", PrimaryBlue, 25);
            practiceCard3.Click += MemoryGameCard_Click;

            cardsFlow.Controls.AddRange(new Control[] { practiceCard1, practiceCard2, practiceCard3 });

            mainContentPanel.Controls.Add(lblTitle, 0, 0);
            mainContentPanel.Controls.Add(lblDesc, 0, 1);
            mainContentPanel.Controls.Add(cardsFlow, 0, 2);
            
            mainContentPanel.ResumeLayout();
        }

        private void ShowReviewContent()
        {
            mainContentPanel.SuspendLayout();
            mainContentPanel.Controls.Clear();

            var lblTitle = new Label
            {
                Text = "🔄 Ôn tập",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            var lblDesc = new Label
            {
                Text = "Ôn lại kiến thức đã học",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(120, 120, 120),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 30)
            };

            var cardsFlow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight
            };

            var reviewCard = CreateLearningCard("📖 12 Thì", "Xem lại các thì", PrimaryBlue, 0);
            reviewCard.Click += TensesCard_Click;

            cardsFlow.Controls.Add(reviewCard);

            mainContentPanel.Controls.Add(lblTitle, 0, 0);
            mainContentPanel.Controls.Add(lblDesc, 0, 1);
            mainContentPanel.Controls.Add(cardsFlow, 0, 2);
            
            mainContentPanel.ResumeLayout();
        }

        private void ShowToolsMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("🌍 Dịch Tiếng Anh", null, (s, e) =>
            {
                this.Cursor = Cursors.WaitCursor;
                var translateForm = new TranslateForm();
                this.Cursor = Cursors.Default;
                translateForm.ShowDialog();
            });
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("🚪 Đăng xuất", null, (s, e) =>
            {
                this.Close();
                new LoginForm().Show();
            });
            menu.Show(Cursor.Position);
        }

        private void LoadUserInfo()
        {
            var user = dbContext.Users.Find(currentUser.UserId);
            if (user != null)
            {
                currentUser = user;
                lblStreak.Text = $"🔥 {currentUser.CurrentStreak}";
                lblXP.Text = $"⭐ {currentUser.TotalXP} XP";
                
                // Refresh progress panel to update visual progress
                foreach (Control ctrl in mainContentPanel.Controls)
                {
                    if (ctrl is Panel panel)
                    {
                        foreach (Control child in panel.Controls)
                        {
                            if (child is Panel progressContainer && progressContainer.Controls.Count > 0)
                            {
                                progressContainer.Invalidate();
                                panel.Invalidate();
                            }
                        }
                    }
                }
            }
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            
            return path;
        }
    }

    // Extension methods for Graphics
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rect, int radius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int diameter = radius * 2;
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                graphics.FillPath(brush, path);
            }
        }

        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle rect, int radius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int diameter = radius * 2;
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                graphics.DrawPath(pen, path);
            }
        }
    }
}
