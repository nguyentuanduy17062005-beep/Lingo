using LingoAppNet8.Models;
using LingoAppNet8.Data;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;

namespace LingoAppNet8.Forms
{
    public partial class QuizForm : Form
    {
        private List<QuizQuestion> questions = new List<QuizQuestion>();
        private int currentQuestionIndex = 0;
        private int correctAnswers = 0;
        private int totalTimeSpent = 0;
        private System.Windows.Forms.Timer questionTimer = null!;
        private int currentTimeLeft;
        private User currentUser;
        private LingoDbContext dbContext;

        private Label lblQuestion = null!;
        private Label lblQuestionNumber = null!;
        private Label lblTimer = null!;
        private Label lblDifficulty = null!;
        private ProgressBar progressBar = null!;
        private RadioButton rbOptionA = null!;
        private RadioButton rbOptionB = null!;
        private RadioButton rbOptionC = null!;
        private RadioButton rbOptionD = null!;
        private Button btnNext = null!;
        private Button btnSubmit = null!;
        private Panel panelOptions = null!;
        private Panel headerPanel = null!;

        public QuizForm(User user, LingoDbContext context)
        {
            currentUser = user;
            dbContext = context;
            InitializeComponent();
            InitializeQuiz();
        }

        private void InitializeComponent()
        {
            this.Text = "LingoApp - Bài Kiểm Tra";
            this.Size = new Size(1200, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Header Panel
            headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(900, 80),
                BackColor = Color.FromArgb(156, 39, 176)
            };

            Label lblTitle = new Label
            {
                Text = "📝 BÀI KIỂM TRA",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 25),
                Size = new Size(300, 35),
                BackColor = Color.Transparent
            };

            headerPanel.Controls.Add(lblTitle);

            // Progress Bar
            progressBar = new ProgressBar
            {
                Location = new Point(50, 100),
                Size = new Size(800, 25),
                Maximum = 10,
                Value = 0,
                Style = ProgressBarStyle.Continuous
            };

            // Question Info Panel
            Panel infoPanel = new Panel
            {
                Location = new Point(50, 140),
                Size = new Size(800, 60),
                BackColor = Color.White
            };
            infoPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, infoPanel.Width - 1, infoPanel.Height - 1);
                }
            };

            // Question Number Label
            lblQuestionNumber = new Label
            {
                Location = new Point(20, 15),
                Size = new Size(250, 35),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Text = "Câu 1/10",
                ForeColor = Color.FromArgb(58, 134, 255),
                BackColor = Color.Transparent
            };

            // Difficulty Label
            lblDifficulty = new Label
            {
                Location = new Point(350, 15),
                Size = new Size(200, 35),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Text = "● Trung bình",
                ForeColor = Color.FromArgb(255, 152, 0),
                BackColor = Color.Transparent
            };

            // Timer Label
            lblTimer = new Label
            {
                Location = new Point(650, 10),
                Size = new Size(130, 40),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(244, 67, 54),
                Text = "⏱ 10s",
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };

            infoPanel.Controls.AddRange(new Control[] { lblQuestionNumber, lblDifficulty, lblTimer });

            // Question Panel
            Panel questionPanel = new Panel
            {
                Location = new Point(50, 220),
                Size = new Size(800, 120),
                BackColor = Color.White
            };
            questionPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.FromArgb(156, 39, 176), 3))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, questionPanel.Width - 1, questionPanel.Height - 1);
                }
            };

            // Question Label
            lblQuestion = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(760, 80),
                Font = new Font("Segoe UI", 14),
                Text = "Câu hỏi sẽ hiển thị ở đây...",
                BackColor = Color.Transparent
            };

            questionPanel.Controls.Add(lblQuestion);

            // Options Panel
            panelOptions = new Panel
            {
                Location = new Point(50, 360),
                Size = new Size(800, 260),
                BackColor = Color.Transparent
            };

            rbOptionA = CreateOptionRadioButton("A. Đáp án A", 0, Color.FromArgb(33, 150, 243));
            rbOptionB = CreateOptionRadioButton("B. Đáp án B", 65, Color.FromArgb(76, 175, 80));
            rbOptionC = CreateOptionRadioButton("C. Đáp án C", 130, Color.FromArgb(255, 152, 0));
            rbOptionD = CreateOptionRadioButton("D. Đáp án D", 195, Color.FromArgb(156, 39, 176));
            panelOptions.Controls.AddRange(new Control[] { rbOptionA, rbOptionB, rbOptionC, rbOptionD });

            // Next Button
            btnNext = new Button
            {
                Location = new Point(720, 635),
                Size = new Size(130, 50),
                Text = "TIẾP THEO →",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += BtnNext_Click;

            // Submit Button
            btnSubmit = new Button
            {
                Location = new Point(720, 635),
                Size = new Size(130, 50),
                Text = "NỘP BÀI ✓",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = false,
                Cursor = Cursors.Hand
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Click += BtnSubmit_Click;

            this.Controls.AddRange(new Control[] {
                headerPanel, progressBar, infoPanel, questionPanel,
                panelOptions, btnNext, btnSubmit
            });

            // Initialize Timer
            questionTimer = new System.Windows.Forms.Timer();
            questionTimer.Interval = 1000; // 1 second
            questionTimer.Tick += QuestionTimer_Tick;
        }

        private RadioButton CreateOptionRadioButton(string text, int yOffset, Color accentColor)
        {
            RadioButton rb = new RadioButton
            {
                Location = new Point(10, yOffset),
                Size = new Size(780, 55),
                Font = new Font("Segoe UI", 12),
                Text = text,
                Appearance = Appearance.Button,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(60, 60, 60),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            rb.FlatAppearance.BorderSize = 2;
            rb.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            rb.FlatAppearance.CheckedBackColor = Color.FromArgb(accentColor.R, accentColor.G, accentColor.B, 30);
            rb.CheckedChanged += (s, e) =>
            {
                if (rb.Checked)
                {
                    rb.FlatAppearance.BorderColor = accentColor;
                    rb.ForeColor = accentColor;
                    rb.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                }
                else
                {
                    rb.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
                    rb.ForeColor = Color.FromArgb(60, 60, 60);
                    rb.Font = new Font("Segoe UI", 12);
                }
            };
            return rb;
        }

        private void InitializeQuiz()
        {
            // Load questions from database
            questions = dbContext.QuizQuestions.Include(q => q.Tense).Take(10).ToList();

            if (questions.Count == 0)
            {
                MessageBox.Show("Không có câu hỏi nào trong database. Vui lòng thêm câu hỏi trước.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            // Shuffle questions
            questions = questions.OrderBy(x => Guid.NewGuid()).ToList();

            // Display first question
            DisplayQuestion();
        }

        private void DisplayQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                FinishQuiz();
                return;
            }

            var question = questions[currentQuestionIndex];

            lblQuestionNumber.Text = $"Câu {currentQuestionIndex + 1}/10";
            lblQuestion.Text = question.Question;
            rbOptionA.Text = $"A. {question.OptionA}";
            rbOptionB.Text = $"B. {question.OptionB}";
            rbOptionC.Text = $"C. {question.OptionC}";
            rbOptionD.Text = $"D. {question.OptionD}";

            // Update difficulty label
            string difficultyText = question.Difficulty.ToLower() switch
            {
                "easy" => "● Dễ",
                "hard" => "● Khó",
                _ => "● Trung bình"
            };
            lblDifficulty.Text = difficultyText;
            lblDifficulty.ForeColor = question.Difficulty.ToLower() switch
            {
                "easy" => Color.FromArgb(76, 175, 80),
                "hard" => Color.FromArgb(244, 67, 54),
                _ => Color.FromArgb(255, 152, 0)
            };

            // Clear selection
            rbOptionA.Checked = false;
            rbOptionB.Checked = false;
            rbOptionC.Checked = false;
            rbOptionD.Checked = false;

            // Set timer based on difficulty (Dễ=15s, Trung bình=10s, Khó=20s)
            switch (question.Difficulty.ToLower())
            {
                case "easy":
                    currentTimeLeft = 15;
                    break;
                case "hard":
                    currentTimeLeft = 20;
                    break;
                default: // normal
                    currentTimeLeft = 10;
                    break;
            }

            lblTimer.Text = $"⏱ {currentTimeLeft}s";
            progressBar.Value = currentQuestionIndex;

            // Show/Hide buttons
            if (currentQuestionIndex == questions.Count - 1)
            {
                btnNext.Visible = false;
                btnSubmit.Visible = true;
            }
            else
            {
                btnNext.Visible = true;
                btnSubmit.Visible = false;
            }

            questionTimer.Start();
        }

        private void QuestionTimer_Tick(object? sender, EventArgs e)
        {
            currentTimeLeft--;
            lblTimer.Text = $"⏱ {currentTimeLeft}s";

            if (currentTimeLeft <= 5)
            {
                lblTimer.ForeColor = Color.Red;
            }
            else if (currentTimeLeft <= 10)
            {
                lblTimer.ForeColor = Color.Orange;
            }

            if (currentTimeLeft <= 0)
            {
                questionTimer.Stop();
                MessageBox.Show("Hết giờ! Tự động chuyển câu tiếp theo.", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                MoveToNextQuestion();
            }
        }

        private void BtnNext_Click(object? sender, EventArgs e)
        {
            questionTimer.Stop();
            CheckAnswer();
            MoveToNextQuestion();
        }

        private void BtnSubmit_Click(object? sender, EventArgs e)
        {
            questionTimer.Stop();
            CheckAnswer();
            FinishQuiz();
        }

        private void CheckAnswer()
        {
            var question = questions[currentQuestionIndex];
            string selectedAnswer = "";

            if (rbOptionA.Checked) selectedAnswer = "A";
            else if (rbOptionB.Checked) selectedAnswer = "B";
            else if (rbOptionC.Checked) selectedAnswer = "C";
            else if (rbOptionD.Checked) selectedAnswer = "D";

            if (selectedAnswer == question.CorrectAnswer)
            {
                correctAnswers++;
            }

            totalTimeSpent += GetQuestionTime(question.Difficulty) - currentTimeLeft;
        }

        private int GetQuestionTime(string difficulty)
        {
            return difficulty.ToLower() switch
            {
                "easy" => 15,
                "hard" => 20,
                _ => 10
            };
        }

        private void MoveToNextQuestion()
        {
            currentQuestionIndex++;
            lblTimer.ForeColor = Color.Red;
            DisplayQuestion();
        }

        private void FinishQuiz()
        {
            progressBar.Value = 10;
            questionTimer.Stop();

            int score = (int)((correctAnswers / 10.0) * 100);

            // Save result to database
            var result = new QuizResult
            {
                UserId = currentUser.UserId,
                CompletedDate = DateTime.Now,
                TotalQuestions = 10,
                CorrectAnswers = correctAnswers,
                Score = score,
                TimeSpent = totalTimeSpent
            };

            dbContext.QuizResults.Add(result);
            dbContext.SaveChanges();

            // Update user XP
            currentUser.TotalXP += score / 10;
            dbContext.SaveChanges();

            MessageBox.Show($"Hoàn thành!\n\nĐúng: {correctAnswers}/10\nĐiểm: {score}\nThời gian: {totalTimeSpent}s\n+{score/10} XP",
                "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }
    }
}
