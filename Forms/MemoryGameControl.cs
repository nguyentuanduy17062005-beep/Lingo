using System.Drawing.Drawing2D;
using Microsoft.Data.SqlClient;

namespace LingoAppNet8.Forms
{
    // Word Pair class for matching game
    public class WordPair
    {
        public string English { get; set; } = "";
        public string Vietnamese { get; set; } = "";
        public bool IsEnglish { get; set; }

        public string DisplayText => IsEnglish ? English : Vietnamese;
    }

    public class MemoryGameControl : UserControl
    {
        private TableLayoutPanel gamePanel = null!;
        private Button? firstClicked = null;
        private Button? secondClicked = null;
        private System.Windows.Forms.Timer hideTimer = null!;
        private List<WordPair> wordPairs = null!;
        private int matchesFound = 0;
        private Label lblTitle = null!;
        private Label lblMatches = null!;
        private Button btnRestart = null!;

        // Database connection string
        private readonly string connectionString = "Server=LAPTOP-7TOIFEJI\\SQLEXPRESS;Database=LingoDb;Integrated Security=True;TrustServerCertificate=True;";

        // Modern Professional Colors
        private readonly Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        private readonly Color AccentOrange = Color.FromArgb(255, 126, 95);
        private readonly Color CardWhite = Color.FromArgb(255, 255, 255);
        private readonly Color BackgroundGray = Color.FromArgb(244, 247, 246);
        private readonly Color MatchGreen = Color.FromArgb(39, 174, 96);
        private readonly Color TextDark = Color.FromArgb(40, 40, 40);
        private readonly Color TextGray = Color.FromArgb(100, 100, 100);

        public MemoryGameControl()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 700);
            this.BackColor = BackgroundGray;
            this.AutoScroll = true;

            // Title
            lblTitle = new Label
            {
                Text = "🎮 TRÒ CHƠI GHI NHỚ",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = PrimaryBlue,
                Location = new Point(30, 20),
                AutoSize = true
            };

            // Instructions
            Label lblInstructions = new Label
            {
                Text = "Nhấp vào các thẻ để tìm cặp từ tiếng Anh - tiếng Việt giống nhau",
                Font = new Font("Segoe UI", 11),
                ForeColor = TextGray,
                Location = new Point(30, 55),
                AutoSize = true
            };

            // Matches counter
            lblMatches = new Label
            {
                Text = "Đã tìm thấy: 0/8",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = AccentOrange,
                Location = new Point(30, 90),
                AutoSize = true
            };

            // Restart button
            btnRestart = new Button
            {
                Text = "🔄 Chơi Lại",
                Size = new Size(140, 45),
                Location = new Point(630, 85),
                BackColor = AccentOrange,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRestart.FlatAppearance.BorderSize = 0;
            btnRestart.Click += BtnRestart_Click;

            // Game panel (4x4 grid)
            gamePanel = new TableLayoutPanel
            {
                Location = new Point(30, 150),
                Size = new Size(740, 520),
                RowCount = 4,
                ColumnCount = 4,
                BackColor = Color.Transparent
            };

            // Setup equal rows and columns
            for (int i = 0; i < 4; i++)
            {
                gamePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
                gamePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }

            // Create 16 card buttons
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    Button cardButton = CreateCardButton();
                    gamePanel.Controls.Add(cardButton, col, row);
                }
            }

            // Timer for hiding mismatched cards
            hideTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            hideTimer.Tick += HideTimer_Tick;

            this.Controls.AddRange(new Control[] { 
                lblTitle, lblInstructions, lblMatches, btnRestart, gamePanel 
            });
        }

        private Button CreateCardButton()
        {
            Button btn = new Button
            {
                Dock = DockStyle.Fill,
                Text = "?",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                BackColor = CardWhite,
                ForeColor = PrimaryBlue,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(5)
            };
            btn.FlatAppearance.BorderSize = 0;
            
            // Custom paint for rounded corners and shadow
            btn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Draw subtle shadow
                using (GraphicsPath shadowPath = GetRoundedRectangle(
                    new Rectangle(2, 2, btn.Width - 4, btn.Height - 4), 10))
                {
                    using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                    {
                        e.Graphics.FillPath(shadowBrush, shadowPath);
                    }
                }
                
                // Draw card background
                using (GraphicsPath path = GetRoundedRectangle(
                    new Rectangle(0, 0, btn.Width - 1, btn.Height - 1), 10))
                {
                    using (SolidBrush brush = new SolidBrush(btn.BackColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            btn.Click += CardButton_Click;
            return btn;
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private List<WordPair> GetRandomWordsFromDB()
        {
            var pairs = new List<WordPair>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT TOP 8 TiengAnh, TiengViet FROM TuVung ORDER BY NEWID()";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pairs.Add(new WordPair
                            {
                                English = reader.GetString(0),
                                Vietnamese = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading words from database: {ex.Message}\nUsing default words.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Fallback to hardcoded words if database fails
                pairs = new List<WordPair>
                {
                    new WordPair { English = "Hello", Vietnamese = "Xin chào" },
                    new WordPair { English = "Goodbye", Vietnamese = "Tạm biệt" },
                    new WordPair { English = "Thank you", Vietnamese = "Cảm ơn" },
                    new WordPair { English = "Please", Vietnamese = "Làm ơn" },
                    new WordPair { English = "Yes", Vietnamese = "Có/Vâng" },
                    new WordPair { English = "No", Vietnamese = "Không" },
                    new WordPair { English = "Friend", Vietnamese = "Bạn bè" },
                    new WordPair { English = "Family", Vietnamese = "Gia đình" }
                };
            }

            return pairs;
        }

        private void InitializeGame()
        {
            // Get 8 random word pairs from database
            var pairs = GetRandomWordsFromDB();

            // Create list with both English and Vietnamese
            wordPairs = new List<WordPair>();
            foreach (var pair in pairs)
            {
                wordPairs.Add(new WordPair 
                { 
                    English = pair.English, 
                    Vietnamese = pair.Vietnamese, 
                    IsEnglish = true 
                });
                wordPairs.Add(new WordPair 
                { 
                    English = pair.English, 
                    Vietnamese = pair.Vietnamese, 
                    IsEnglish = false 
                });
            }

            // Shuffle
            ShuffleList(wordPairs);

            // Assign to buttons
            int index = 0;
            foreach (Control control in gamePanel.Controls)
            {
                if (control is Button btn)
                {
                    btn.Tag = wordPairs[index];
                    btn.Text = "?";
                    btn.BackColor = CardWhite;
                    btn.ForeColor = PrimaryBlue;
                    btn.Enabled = true;
                    index++;
                }
            }

            matchesFound = 0;
            UpdateMatchesLabel();
        }

        private void ShuffleList<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void CardButton_Click(object? sender, EventArgs e)
        {
            if (hideTimer.Enabled) return; // Ignore clicks during hide animation

            Button clickedButton = (Button)sender!;
            
            // Ignore if already revealed or same button clicked twice
            if (clickedButton.BackColor == MatchGreen || clickedButton == firstClicked)
                return;

            WordPair wordPair = (WordPair)clickedButton.Tag!;
            
            // Reveal the card
            clickedButton.Text = wordPair.DisplayText;
            clickedButton.BackColor = Color.FromArgb(240, 248, 255);
            clickedButton.ForeColor = TextDark;
            clickedButton.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            // First click
            if (firstClicked == null)
            {
                firstClicked = clickedButton;
                return;
            }

            // Second click
            secondClicked = clickedButton;
            WordPair firstPair = (WordPair)firstClicked.Tag!;
            WordPair secondPair = (WordPair)secondClicked.Tag!;

            // Check for match (same word but different language)
            if (firstPair.English == secondPair.English && 
                firstPair.Vietnamese == secondPair.Vietnamese &&
                firstPair.IsEnglish != secondPair.IsEnglish)
            {
                // Match found!
                MatchFound();
            }
            else
            {
                // Mismatch - start timer to hide cards
                hideTimer.Start();
            }
        }

        private void MatchFound()
        {
            firstClicked!.BackColor = MatchGreen;
            firstClicked.ForeColor = Color.White;
            secondClicked!.BackColor = MatchGreen;
            secondClicked.ForeColor = Color.White;

            matchesFound++;
            UpdateMatchesLabel();

            firstClicked = null;
            secondClicked = null;

            // Check if game is won
            if (matchesFound == 8)
            {
                MessageBox.Show("🎉 Chúc mừng! Bạn đã hoàn thành trò chơi!\n\n" +
                    "Bạn đã tìm được tất cả 8 cặp từ!", 
                    "Chiến thắng!", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
            }
        }

        private void HideTimer_Tick(object? sender, EventArgs e)
        {
            hideTimer.Stop();

            // Hide the cards
            firstClicked!.Text = "?";
            firstClicked.BackColor = CardWhite;
            firstClicked.ForeColor = PrimaryBlue;
            firstClicked.Font = new Font("Segoe UI", 24, FontStyle.Bold);

            secondClicked!.Text = "?";
            secondClicked.BackColor = CardWhite;
            secondClicked.ForeColor = PrimaryBlue;
            secondClicked.Font = new Font("Segoe UI", 24, FontStyle.Bold);

            firstClicked = null;
            secondClicked = null;
        }

        private void UpdateMatchesLabel()
        {
            lblMatches.Text = $"Đã tìm thấy: {matchesFound}/8";
        }

        private void BtnRestart_Click(object? sender, EventArgs e)
        {
            RestartGame();
        }

        public void RestartGame()
        {
            hideTimer.Stop();
            firstClicked = null;
            secondClicked = null;
            InitializeGame();
        }
    }
}
