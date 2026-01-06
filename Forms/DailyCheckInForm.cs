using LingoAppNet8.Data;
using LingoAppNet8.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;

namespace LingoAppNet8.Forms
{
    // Custom Flat 3D Button with depth effect
    public class Flat3DButton : Button
    {
        private Color baseColor;
        private Color darkColor;
        private int depthSize = 6;
        private bool isPressed = false;
        private Point originalLocation;

        public Flat3DButton(Color buttonColor)
        {
            baseColor = buttonColor;
            darkColor = ControlPaint.Dark(buttonColor, 0.3f);
            
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.BackColor = baseColor;
            this.Cursor = Cursors.Hand;
            
            // Store original location for press effect
            this.LocationChanged += (s, e) =>
            {
                if (!isPressed)
                    originalLocation = this.Location;
            };
            
            this.MouseDown += Flat3DButton_MouseDown;
            this.MouseUp += Flat3DButton_MouseUp;
            this.MouseLeave += Flat3DButton_MouseLeave;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Draw 3D depth effect at the bottom
            if (!isPressed)
            {
                using (SolidBrush brush = new SolidBrush(darkColor))
                {
                    Rectangle depthRect = new Rectangle(0, this.Height - depthSize, this.Width, depthSize);
                    e.Graphics.FillRectangle(brush, depthRect);
                }
                
                // Redraw text on top to avoid overlap
                TextRenderer.DrawText(e.Graphics, this.Text, this.Font, 
                    new Rectangle(0, 0, this.Width, this.Height - depthSize), 
                    this.ForeColor, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            else
            {
                // When pressed, draw text without depth offset
                TextRenderer.DrawText(e.Graphics, this.Text, this.Font, 
                    new Rectangle(0, 0, this.Width, this.Height), 
                    this.ForeColor, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private void Flat3DButton_MouseDown(object? sender, MouseEventArgs e)
        {
            if (this.Enabled && e.Button == MouseButtons.Left)
            {
                isPressed = true;
                originalLocation = this.Location;
                // Move button down to simulate press
                this.Location = new Point(this.Location.X, this.Location.Y + depthSize);
                this.Invalidate();
            }
        }

        private void Flat3DButton_MouseUp(object? sender, MouseEventArgs e)
        {
            if (isPressed)
            {
                isPressed = false;
                // Move button back to original position
                this.Location = originalLocation;
                this.Invalidate();
            }
        }

        private void Flat3DButton_MouseLeave(object? sender, EventArgs e)
        {
            if (isPressed)
            {
                isPressed = false;
                this.Location = originalLocation;
                this.Invalidate();
            }
        }

        // Update colors if needed
        public void SetColors(Color newBaseColor)
        {
            baseColor = newBaseColor;
            darkColor = ControlPaint.Dark(newBaseColor, 0.3f);
            this.BackColor = baseColor;
            this.Invalidate();
        }
    }

    // Custom Day Card with rounded corners and status-based colors
    public class DayCardPanel : Panel
    {
        public enum CardStatus
        {
            Checked,    // Already checked in
            Missed,     // Past date, not checked in
            Future      // Future date
        }

        private CardStatus status;
        private int cornerRadius = 15;

        public DayCardPanel(CardStatus cardStatus)
        {
            status = cardStatus;
            this.BorderStyle = BorderStyle.None;
            this.DoubleBuffered = true;
            SetColors();
        }

        private void SetColors()
        {
            switch (status)
            {
                case CardStatus.Checked:
                    this.BackColor = Color.FromArgb(88, 204, 2); // Green #58CC02
                    break;
                case CardStatus.Missed:
                    this.BackColor = Color.FromArgb(229, 229, 229); // Gray #E5E5E5
                    break;
                case CardStatus.Future:
                    this.BackColor = Color.White;
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            
            using (GraphicsPath path = GetRoundedRectangle(rect, cornerRadius))
            {
                // Fill background
                using (SolidBrush brush = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
                
                // Draw border
                if (status == CardStatus.Future)
                {
                    // Gray border for future dates
                    using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Subtle border for checked and missed
                    using (Pen pen = new Pen(ControlPaint.Dark(this.BackColor, 0.1f), 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        public CardStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                SetColors();
                this.Invalidate();
            }
        }
    }

    public class DailyCheckInForm : Form
    {
        private User currentUser;
        private LingoDbContext dbContext;
        private Panel calendarPanel = null!;
        private Label lblStreak = null!;
        private Label lblTotalDays = null!;
        private Button btnCheckIn = null!;
        private Panel statsPanel = null!;
        private bool hasCheckedInToday = false;

        public DailyCheckInForm(User user, LingoDbContext context)
        {
            currentUser = user;
            dbContext = context;
            InitializeComponent();
            LoadCheckInData();
        }

        private void InitializeComponent()
        {
            this.Text = "LingoApp - Điểm Danh Hàng Ngày";
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized;

            // Header
            Label lblTitle = new Label
            {
                Text = "📅 ĐIỂM DANH HÀNG NGÀY",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(58, 134, 255),
                Location = new Point(50, 20),
                AutoSize = true
            };

            // Stats Panel (Top)
            statsPanel = new Panel
            {
                Location = new Point(50, 80),
                Size = new Size(700, 140),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            statsPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, statsPanel.Width - 1, statsPanel.Height - 1);
                }
            };

            // Current Streak
            Label lblStreakTitle = new Label
            {
                Text = "🔥 Chuỗi Ngày Liên Tiếp",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(30, 20),
                Size = new Size(300, 30),
                ForeColor = Color.FromArgb(80, 80, 80)
            };

            lblStreak = new Label
            {
                Text = $"{currentUser.CurrentStreak} ngày",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                Location = new Point(30, 50),
                AutoSize = true,
                ForeColor = Color.FromArgb(255, 107, 0)
            };

            // Total Days
            Label lblTotalTitle = new Label
            {
                Text = "✅ Tổng Số Ngày Đã Học",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(400, 20),
                Size = new Size(260, 30),
                ForeColor = Color.FromArgb(80, 80, 80)
            };

            lblTotalDays = new Label
            {
                Text = "0 ngày",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                Location = new Point(400, 50),
                AutoSize = true,
                ForeColor = Color.FromArgb(88, 204, 2)
            };

            statsPanel.Controls.AddRange(new Control[] {
                lblStreakTitle, lblStreak, lblTotalTitle, lblTotalDays
            });

            // Check-in Button with 3D effect
            btnCheckIn = new Flat3DButton(Color.FromArgb(88, 204, 2))
            {
                Text = "✨ ĐIỂM DANH HÔM NAY",
                Size = new Size(300, 60),
                Location = new Point(250, 220),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold)
            };
            btnCheckIn.Click += BtnCheckIn_Click;

            // Calendar Panel
            Label lblHistory = new Label
            {
                Text = "📆 Lịch Sử Điểm Danh (7 Ngày Gần Nhất)",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(50, 300),
                Size = new Size(500, 30),
                ForeColor = Color.FromArgb(58, 134, 255)
            };

            calendarPanel = new Panel
            {
                Location = new Point(50, 340),
                Size = new Size(700, 220),
                BackColor = Color.White,
                AutoScroll = true,
                BorderStyle = BorderStyle.None
            };
            calendarPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, calendarPanel.Width - 1, calendarPanel.Height - 1);
                }
            };

            // Close Button with 3D effect
            Flat3DButton btnClose = new Flat3DButton(Color.FromArgb(180, 180, 180))
            {
                Text = "ĐÓNG",
                Size = new Size(120, 40),
                Location = new Point(340, 570),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitle, statsPanel, btnCheckIn, lblHistory, calendarPanel, btnClose
            });
        }

        private void LoadCheckInData()
        {
            // Load check-in data from database
            var checkIns = dbContext.DailyCheckIns
                .Where(dc => dc.UserId == currentUser.UserId)
                .OrderByDescending(dc => dc.CheckInDate)
                .Take(7)
                .ToList();

            // Update total days
            int totalDays = dbContext.DailyCheckIns
                .Count(dc => dc.UserId == currentUser.UserId);
            lblTotalDays.Text = $"{totalDays} ngày";

            // Check if already checked in today
            var today = DateTime.Today;
            hasCheckedInToday = checkIns.Any(dc => dc.CheckInDate.Date == today);

            if (hasCheckedInToday)
            {
                btnCheckIn.Text = "✅ ĐÃ ĐIỂM DANH HÔM NAY";
                if (btnCheckIn is Flat3DButton flat3DBtn)
                {
                    flat3DBtn.SetColors(Color.FromArgb(150, 150, 150));
                }
                btnCheckIn.Enabled = false;
            }

            // Display calendar
            DisplayCalendar(checkIns);
        }

        private void DisplayCalendar(List<DailyCheckIn> checkIns)
        {
            calendarPanel.Controls.Clear();
            int x = 30;
            int y = 30;

            for (int i = 6; i >= 0; i--)
            {
                DateTime date = DateTime.Today.AddDays(-i);
                bool isCheckedIn = checkIns.Any(dc => dc.CheckInDate.Date == date);
                bool isFuture = date > DateTime.Today;
                
                // Determine card status
                DayCardPanel.CardStatus cardStatus;
                if (isFuture)
                {
                    cardStatus = DayCardPanel.CardStatus.Future;
                }
                else if (isCheckedIn)
                {
                    cardStatus = DayCardPanel.CardStatus.Checked;
                }
                else
                {
                    cardStatus = DayCardPanel.CardStatus.Missed;
                }

                DayCardPanel dayPanel = new DayCardPanel(cardStatus)
                {
                    Location = new Point(x, y),
                    Size = new Size(80, 155)
                };

                // Set text colors based on status
                Color textColor;
                if (cardStatus == DayCardPanel.CardStatus.Checked)
                {
                    textColor = Color.White;
                }
                else if (cardStatus == DayCardPanel.CardStatus.Missed)
                {
                    textColor = Color.FromArgb(80, 80, 80); // Dark text
                }
                else // Future
                {
                    textColor = Color.FromArgb(150, 150, 150); // Gray text
                }

                Label lblDay = new Label
                {
                    Text = date.ToString("dd"),
                    Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                    Location = new Point(0, 15),
                    Size = new Size(80, 40),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = textColor,
                    BackColor = Color.Transparent
                };

                Label lblMonth = new Label
                {
                    Text = date.ToString("MMM"),
                    Font = new Font("Segoe UI", 10F),
                    Location = new Point(0, 60),
                    Size = new Size(80, 20),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = textColor,
                    BackColor = Color.Transparent
                };

                Label lblStatus = new Label
                {
                    Text = isCheckedIn ? "✓" : (isFuture ? "○" : "✗"),
                    Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                    Location = new Point(0, 95),
                    Size = new Size(80, 45),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = textColor,
                    BackColor = Color.Transparent
                };

                dayPanel.Controls.AddRange(new Control[] { lblDay, lblMonth, lblStatus });
                calendarPanel.Controls.Add(dayPanel);

                x += 95;
            }
        }

        private void BtnCheckIn_Click(object? sender, EventArgs e)
        {
            if (hasCheckedInToday)
            {
                MessageBox.Show("Bạn đã điểm danh hôm nay rồi!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Create new check-in
                var checkIn = new DailyCheckIn
                {
                    UserId = currentUser.UserId,
                    CheckInDate = DateTime.Now,
                    XPEarned = 10
                };
                dbContext.DailyCheckIns.Add(checkIn);

                // Update user streak
                var yesterday = DateTime.Today.AddDays(-1);
                var checkedInYesterday = dbContext.DailyCheckIns
                    .Any(dc => dc.UserId == currentUser.UserId && dc.CheckInDate.Date == yesterday);

                if (checkedInYesterday)
                {
                    currentUser.CurrentStreak++;
                }
                else
                {
                    currentUser.CurrentStreak = 1;
                }

                if (currentUser.CurrentStreak > currentUser.LongestStreak)
                {
                    currentUser.LongestStreak = currentUser.CurrentStreak;
                }

                // Add XP
                currentUser.TotalXP += 10;

                dbContext.SaveChanges();

                MessageBox.Show($"🎉 Điểm danh thành công!\n\n+10 XP\nChuỗi ngày: {currentUser.CurrentStreak} ngày",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reload data
                LoadCheckInData();
                lblStreak.Text = $"{currentUser.CurrentStreak} ngày";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
