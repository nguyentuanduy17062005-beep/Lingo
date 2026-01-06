using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LingoAppNet8.Data;
using LingoAppNet8.Models;
using LingoAppNet8.Services;
using NAudio.Wave;
using Microsoft.EntityFrameworkCore;

namespace LingoAppNet8.Forms
{
    public partial class SpeakingPracticeForm : Form
    {
        private readonly LingoDbContext _context;
        private readonly User _currentUser;
        private readonly SpeechRecognitionService _speechService;
        
        private SpeakingSentence? _currentSentence;
        private WaveInEvent? _waveIn;
        private WaveFileWriter? _writer;
        private string _audioFilePath = "";
        private bool _isRecording = false;

        private Panel headerPanel = null!;
        private Label lblSentence = null!;
        private Label lblTranslation = null!;
        private Label lblCategory = null!;
        private Button btnMicrophone = null!;
        private Button btnNext = null!;
        private Button btnPlayback = null!;
        private Panel resultPanel = null!;
        private Label lblAccuracy = null!;
        private Label lblFluency = null!;
        private Label lblCompleteness = null!;
        private Label lblOverallScore = null!;
        private ProgressBar pbAccuracy = null!;
        private ProgressBar pbFluency = null!;
        private ProgressBar pbCompleteness = null!;
        private ProgressBar pbOverall = null!;
        private ComboBox cboDifficulty = null!;
        private FlowLayoutPanel wordPanel = null!;
        private System.Speech.Synthesis.SpeechSynthesizer? _synthesizer;

        public SpeakingPracticeForm(LingoDbContext context, User user)
        {
            _context = context;
            _currentUser = user;
            _speechService = new SpeechRecognitionService();
            _synthesizer = new System.Speech.Synthesis.SpeechSynthesizer();
            
            InitializeComponents();
            LoadRandomSentence();
        }

        private void InitializeComponents()
        {
            this.Text = "LingoApp - Luyện Phát Âm";
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(900, 700);

            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(156, 39, 176)
            };
            headerPanel.Paint += (s, e) =>
            {
                LinearGradientBrush brush = new LinearGradientBrush(
                    headerPanel.ClientRectangle,
                    Color.FromArgb(156, 39, 176),
                    Color.FromArgb(103, 58, 183),
                    LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(brush, headerPanel.ClientRectangle);
            };

            Label lblTitle = new Label
            {
                Text = "🎤 LUYỆN PHÁT ÂM TIẾNG ANH",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(lblTitle);

            // Difficulty Selection Panel
            Panel difficultyPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            Label lblDifficultyLabel = new Label
            {
                Text = "Chọn độ khó:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 18),
                Size = new Size(120, 25),
                BackColor = Color.Transparent
            };

            cboDifficulty = new ComboBox
            {
                Location = new Point(150, 15),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 11F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboDifficulty.Items.AddRange(new object[] { "Dễ", "Bình thường", "Khó" });
            cboDifficulty.SelectedIndex = 0;
            cboDifficulty.SelectedIndexChanged += (s, e) => LoadRandomSentence();

            difficultyPanel.Controls.AddRange(new Control[] { lblDifficultyLabel, cboDifficulty });

            // Main Content Panel
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(50),
                BackColor = Color.FromArgb(245, 247, 250),
                AutoScroll = true
            };

            // Sentence Panel
            Panel sentencePanel = new Panel
            {
                Location = new Point(50, 20),
                Size = new Size(800, 180),
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            sentencePanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = GetRoundedRectangle(sentencePanel.ClientRectangle, 15);
                e.Graphics.FillPath(new SolidBrush(Color.White), path);
                e.Graphics.DrawPath(new Pen(Color.FromArgb(220, 220, 220), 2), path);
            };

            lblCategory = new Label
            {
                Text = "📚 Daily Conversation",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(156, 39, 176),
                Location = new Point(30, 20),
                Size = new Size(740, 30),
                BackColor = Color.Transparent
            };

            lblSentence = new Label
            {
                Text = "Good morning! How are you today?",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                Location = new Point(20, 45),
                Size = new Size(760, 40),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            lblTranslation = new Label
            {
                Text = "🇻🇳 Chào buổi sáng! Hôm nay bạn thế nào?",
                Font = new Font("Segoe UI", 11F, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(20, 90),
                Size = new Size(760, 25),
                BackColor = Color.Transparent
            };

            // Word Panel - Hiển thị từng từ có thể click để dịch
            wordPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 120),
                Size = new Size(760, 45),
                BackColor = Color.Transparent,
                AutoScroll = true
            };

            sentencePanel.Controls.AddRange(new Control[] { lblCategory, lblSentence, lblTranslation, wordPanel });

            // Recording Controls Panel
            Panel recordingPanel = new Panel
            {
                Location = new Point(50, 220),
                Size = new Size(800, 100),
                BackColor = Color.Transparent
            };

            btnMicrophone = new Button
            {
                Text = "🎤 BẤM VÀ GIỮ ĐỂ NÓI",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Size = new Size(400, 80),
                Location = new Point(200, 20),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(244, 67, 54),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnMicrophone.FlatAppearance.BorderSize = 0;
            btnMicrophone.MouseDown += BtnMicrophone_MouseDown;
            btnMicrophone.MouseUp += BtnMicrophone_MouseUp;

            recordingPanel.Controls.Add(btnMicrophone);

            // Result Panel
            resultPanel = new Panel
            {
                Location = new Point(50, 340),
                Size = new Size(800, 320),
                BackColor = Color.White,
                Padding = new Padding(20),
                Visible = false
            };
            resultPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = GetRoundedRectangle(resultPanel.ClientRectangle, 15);
                e.Graphics.FillPath(new SolidBrush(Color.White), path);
                e.Graphics.DrawPath(new Pen(Color.FromArgb(220, 220, 220), 2), path);
            };

            Label lblResultTitle = new Label
            {
                Text = "📊 KẾT QUẢ ĐÁNH GIÁ",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(76, 175, 80),
                Location = new Point(30, 20),
                Size = new Size(740, 35),
                BackColor = Color.Transparent
            };

            // Overall Score (large display)
            lblOverallScore = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 38F, FontStyle.Bold),
                ForeColor = Color.FromArgb(76, 175, 80),
                Location = new Point(20, 55),
                Size = new Size(180, 65),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblOverallLabel = new Label
            {
                Text = "ĐIỂM TỔNG",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(20, 120),
                Size = new Size(180, 25),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };

            pbOverall = new ProgressBar
            {
                Location = new Point(20, 150),
                Size = new Size(180, 8),
                Style = ProgressBarStyle.Continuous
            };

            // Detail Scores
            int yPos = 55;
            
            // Accuracy
            Label lblAccuracyLabel = new Label
            {
                Text = "Độ chính xác:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(240, yPos),
                Size = new Size(150, 22),
                BackColor = Color.Transparent
            };
            lblAccuracy = new Label
            {
                Text = "0%",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Location = new Point(690, yPos),
                Size = new Size(90, 22),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight
            };
            pbAccuracy = new ProgressBar
            {
                Location = new Point(240, yPos + 25),
                Size = new Size(540, 12)
            };

            // Fluency
            yPos += 50;
            Label lblFluencyLabel = new Label
            {
                Text = "Độ trôi chảy:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(240, yPos),
                Size = new Size(150, 22),
                BackColor = Color.Transparent
            };
            lblFluency = new Label
            {
                Text = "0%",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 152, 0),
                Location = new Point(690, yPos),
                Size = new Size(90, 22),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight
            };
            pbFluency = new ProgressBar
            {
                Location = new Point(240, yPos + 25),
                Size = new Size(540, 12)
            };

            // Completeness
            yPos += 50;
            Label lblCompletenessLabel = new Label
            {
                Text = "Độ hoàn chỉnh:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(240, yPos),
                Size = new Size(150, 22),
                BackColor = Color.Transparent
            };
            lblCompleteness = new Label
            {
                Text = "0%",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(76, 175, 80),
                Location = new Point(690, yPos),
                Size = new Size(90, 22),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight
            };
            pbCompleteness = new ProgressBar
            {
                Location = new Point(240, yPos + 25),
                Size = new Size(540, 12)
            };

            resultPanel.Controls.AddRange(new Control[] {
                lblResultTitle, lblOverallScore, lblOverallLabel, pbOverall,
                lblAccuracyLabel, lblAccuracy, pbAccuracy,
                lblFluencyLabel, lblFluency, pbFluency,
                lblCompletenessLabel, lblCompleteness, pbCompleteness
            });

            // Bottom Buttons
            Panel buttonPanel = new Panel
            {
                Location = new Point(50, 680),
                Size = new Size(800, 60),
                BackColor = Color.Transparent
            };

            btnNext = CreateModernButton("➡️ CÂU TIẾP THEO", Color.FromArgb(76, 175, 80));
            btnNext.Size = new Size(250, 55);
            btnNext.Location = new Point(550, 0);
            btnNext.Click += BtnNext_Click;

            btnPlayback = CreateModernButton("🔊 NGHE LẠI", Color.FromArgb(33, 150, 243));
            btnPlayback.Size = new Size(200, 55);
            btnPlayback.Location = new Point(330, 0);
            btnPlayback.Click += BtnPlayback_Click;
            btnPlayback.Visible = false;

            buttonPanel.Controls.AddRange(new Control[] { btnPlayback, btnNext });

            contentPanel.Controls.AddRange(new Control[] {
                sentencePanel, recordingPanel, resultPanel, buttonPanel
            });

            // Center panels on resize
            contentPanel.Resize += (s, e) =>
            {
                int centerX = (contentPanel.Width - 800) / 2;
                sentencePanel.Left = centerX;
                recordingPanel.Left = centerX;
                resultPanel.Left = centerX;
                buttonPanel.Left = centerX;
            };

            this.Controls.Add(contentPanel);
            this.Controls.Add(difficultyPanel);
            this.Controls.Add(headerPanel);
        }

        private void BtnMicrophone_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                StartRecording();
            }
        }

        private void BtnMicrophone_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                StopRecording();
            }
        }

        private void StartRecording()
        {
            try
            {
                _isRecording = true;
                btnMicrophone.Text = "🔴 ĐANG GHI ÂM...";
                btnMicrophone.BackColor = Color.FromArgb(156, 39, 176);

                _audioFilePath = Path.Combine(Path.GetTempPath(), $"lingo_recording_{DateTime.Now.Ticks}.wav");
                
                _waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(16000, 1)
                };

                _writer = new WaveFileWriter(_audioFilePath, _waveIn.WaveFormat);

                _waveIn.DataAvailable += (s, e) =>
                {
                    _writer?.Write(e.Buffer, 0, e.BytesRecorded);
                };

                _waveIn.StartRecording();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi bắt đầu ghi âm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void StopRecording()
        {
            if (!_isRecording) return;

            try
            {
                _isRecording = false;
                btnMicrophone.Text = "⏳ ĐANG PHÂN TÍCH...";
                btnMicrophone.Enabled = false;

                _waveIn?.StopRecording();
                _writer?.Dispose();
                _waveIn?.Dispose();

                // Wait a bit for file to be written
                await Task.Delay(500);

                if (_currentSentence != null && File.Exists(_audioFilePath))
                {
                    var result = await _speechService.AssessPronunciationAsync(_currentSentence.EnglishText, _audioFilePath);

                    if (result.Success)
                    {
                        DisplayResults(result);
                        SaveResult(result);
                    }
                    else
                    {
                        MessageBox.Show(result.ErrorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                btnMicrophone.Text = "🎤 BẤM VÀ GIỮ ĐỂ NÓI";
                btnMicrophone.BackColor = Color.FromArgb(244, 67, 54);
                btnMicrophone.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnMicrophone.Text = "🎤 BẤM VÀ GIỮ ĐỂ NÓI";
                btnMicrophone.BackColor = Color.FromArgb(244, 67, 54);
                btnMicrophone.Enabled = true;
            }
        }

        private void DisplayResults(PronunciationResult result)
        {
            lblOverallScore.Text = $"{(int)result.PronunciationScore}";
            lblAccuracy.Text = $"{(int)result.AccuracyScore}%";
            lblFluency.Text = $"{(int)result.FluencyScore}%";
            lblCompleteness.Text = $"{(int)result.CompletenessScore}%";

            pbOverall.Value = (int)Math.Min(100, result.PronunciationScore);
            pbAccuracy.Value = (int)Math.Min(100, result.AccuracyScore);
            pbFluency.Value = (int)Math.Min(100, result.FluencyScore);
            pbCompleteness.Value = (int)Math.Min(100, result.CompletenessScore);

            // Color code based on score
            Color scoreColor;
            if (result.PronunciationScore >= 80)
                scoreColor = Color.FromArgb(76, 175, 80); // Green
            else if (result.PronunciationScore >= 60)
                scoreColor = Color.FromArgb(255, 152, 0); // Orange
            else
                scoreColor = Color.FromArgb(244, 67, 54); // Red

            lblOverallScore.ForeColor = scoreColor;

            resultPanel.Visible = true;
            btnPlayback.Visible = true;
        }

        private void SaveResult(PronunciationResult result)
        {
            if (_currentSentence == null) return;

            var speakingResult = new SpeakingResult
            {
                UserId = _currentUser.UserId,
                SentenceId = _currentSentence.Id,
                AccuracyScore = result.AccuracyScore,
                FluencyScore = result.FluencyScore,
                CompletenessScore = result.CompletenessScore,
                PronunciationScore = result.PronunciationScore,
                RecognizedText = result.RecognizedText,
                PracticeDate = DateTime.Now
            };

            _context.Set<SpeakingResult>().Add(speakingResult);
            
            // Award XP
            int xpGained = (int)(result.PronunciationScore / 5); // 1 XP per 5 points
            _currentUser.TotalXP += xpGained;
            
            _context.SaveChanges();
        }

        private void BtnNext_Click(object? sender, EventArgs e)
        {
            LoadRandomSentence();
        }

        private void BtnPlayback_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_audioFilePath) || !File.Exists(_audioFilePath))
            {
                MessageBox.Show("Chưa có bản ghi âm. Vui lòng ghi âm trước!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                btnPlayback.Enabled = false;
                btnPlayback.Text = "🔊 ĐANG PHÁT...";

                // Use Task to avoid blocking UI
                Task.Run(() =>
                {
                    using var audioFile = new AudioFileReader(_audioFilePath);
                    using var outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }).ContinueWith(t =>
                {
                    // Re-enable button on UI thread
                    this.Invoke(new Action(() =>
                    {
                        btnPlayback.Text = "🔊 NGHE LẠI";
                        btnPlayback.Enabled = true;
                        
                        if (t.IsFaulted && t.Exception != null)
                        {
                            MessageBox.Show($"Lỗi phát âm thanh: {t.Exception.InnerException?.Message}", 
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }));
                });
            }
            catch (Exception ex)
            {
                btnPlayback.Text = "🔊 NGHE LẠI";
                btnPlayback.Enabled = true;
                MessageBox.Show($"Không thể phát lại: {ex.Message}", "Lỗi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Button CreateModernButton(string text, Color color)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = color,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private GraphicsPath GetRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
            path.AddArc(bounds.Right - radius, bounds.Y, radius, radius, 270, 90);
            path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void LoadRandomSentence()
        {
            var allSentences = _context.Set<SpeakingSentence>().ToList();
            
            if (!allSentences.Any())
            {
                SeedSentences();
                allSentences = _context.Set<SpeakingSentence>().ToList();
            }

            // Lọc theo độ khó
            string selectedDifficulty = cboDifficulty?.SelectedItem?.ToString() ?? "Dễ";
            string levelFilter = selectedDifficulty switch
            {
                "Dễ" => "Easy",
                "Bình thường" => "Medium",
                "Khó" => "Hard",
                _ => "Easy"
            };

            var sentences = allSentences.Where(s => s.Level == levelFilter).ToList();
            if (!sentences.Any())
            {
                sentences = allSentences;
            }

            var random = new Random();
            _currentSentence = sentences[random.Next(sentences.Count)];

            lblSentence.Text = _currentSentence.EnglishText;
            lblTranslation.Text = $"🇻🇳 {_currentSentence.VietnameseTranslation}";
            lblCategory.Text = $"📚 {_currentSentence.Category} - {_currentSentence.Level}";

            CreateWordButtons(_currentSentence.EnglishText);

            resultPanel.Visible = false;
            btnPlayback.Visible = false;
        }

        private void CreateWordButtons(string sentence)
        {
            wordPanel.Controls.Clear();
            string[] words = sentence.Split(new[] { ' ', ',', '.', '!', '?', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                Button btnWord = new Button
                {
                    Text = word,
                    AutoSize = true,
                    Padding = new Padding(8, 4, 8, 4),
                    Margin = new Padding(3),
                    Font = new Font("Segoe UI", 10F),
                    BackColor = Color.FromArgb(230, 230, 250),
                    ForeColor = Color.FromArgb(63, 81, 181),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnWord.FlatAppearance.BorderColor = Color.FromArgb(63, 81, 181);
                btnWord.FlatAppearance.BorderSize = 1;
                btnWord.Click += async (s, e) => await TranslateAndSpeakWord(word);
                
                wordPanel.Controls.Add(btnWord);
            }
        }

        private async Task TranslateAndSpeakWord(string word)
        {
            try
            {
                string translation = await GetWordTranslation(word);
                
                // Phát âm từ
                await Task.Run(() => _synthesizer?.SpeakAsync(word));

                MessageBox.Show($"📖 {word}\n\n🇻🇳 {translation}\n\n🔊 Đang phát âm...", 
                    "Dịch từ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể dịch từ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task<string> GetWordTranslation(string word)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"Good", "Tốt"}, {"Morning", "Buổi sáng"}, {"How", "Như thế nào"}, {"Are", "Là"}, {"You", "Bạn"},
                {"Today", "Hôm nay"}, {"Thank", "Cảm ơn"}, {"Very", "Rất"}, {"Much", "Nhiều"}, {"Help", "Giúp đỡ"},
                {"Could", "Có thể"}, {"Please", "Làm ơn"}, {"Speak", "Nói"}, {"More", "Hơn"}, {"Slowly", "Chậm"},
                {"Would", "Muốn"}, {"Like", "Thích"}, {"Book", "Đặt"}, {"Table", "Bàn"}, {"Two", "Hai"},
                {"Excuse", "Xin lỗi"}, {"Me", "Tôi"}, {"Where", "Ở đâu"}, {"Nearest", "Gần nhất"}, {"Station", "Trạm"},
                {"Looking", "Đang tìm"}, {"Forward", "Mong chờ"}, {"Working", "Làm việc"}, {"With", "Với"}, {"Project", "Dự án"},
                {"Elaborate", "Giải thích chi tiết"}, {"Previous", "Trước đây"}, {"Experience", "Kinh nghiệm"},
                {"Weather", "Thời tiết"}, {"Beautiful", "Đẹp"}, {"Isn't", "Phải không"}, {"Need", "Cần"},
                {"Assistance", "Trợ giúp"}, {"Issue", "Vấn đề"}, {"Resolved", "Giải quyết"}, {"Quickly", "Nhanh chóng"},
                {"Appreciate", "Trân trọng"}, {"Recommend", "Đề xuất"}, {"Restaurant", "Nhà hàng"}, {"Area", "Khu vực"},
                {"Understand", "Hiểu"}, {"Correctly", "Chính xác"}, {"Concern", "Quan tâm"}, {"Environmental", "Môi trường"},
                {"Sustainability", "Bền vững"}, {"Impressive", "Ấn tượng"}, {"Presentation", "Trình bày"},
                {"Congratulations", "Chúc mừng"}, {"Achieving", "Đạt được"}, {"Goals", "Mục tiêu"},
                {"Discuss", "Thảo luận"}, {"Details", "Chi tiết"}, {"Proposal", "Đề xuất"}, {"Meeting", "Cuộc họp"},
                {"Delighted", "Vui mừng"}, {"Opportunity", "Cơ hội"}, {"Collaborate", "Hợp tác"},
                {"Excited", "Háo hức"}, {"Start", "Bắt đầu"}, {"Journey", "Hành trình"}, {"Together", "Cùng nhau"},
                {"This", "Cái này"}, {"My", "Của tôi"}, {"Friend", "Bạn bè"}, {"Love", "Yêu"}, {"Coffee", "Cà phê"},
                {"Tea", "Trà"}, {"Live", "Sống"}, {"Do", "Làm"}, {"Have", "Có"}, {"Nice", "Đẹp"}, {"Day", "Ngày"},
                {"See", "Gặp"}, {"Tomorrow", "Ngày mai"}, {"Hungry", "Đói"}, {"Eat", "Ăn"}, {"Let's", "Hãy"},
                {"People", "Người"}, {"For", "Cho"}, {"Can", "Có thể"}, {"Tell", "Nói"}, {"About", "Về"},
                {"Trying", "Cố gắng"}, {"Find", "Tìm"}, {"Best", "Tốt nhất"}, {"Solution", "Giải pháp"},
                {"Problem", "Vấn đề"}, {"Long", "Dài"}, {"Does", "Làm"}, {"Take", "Mất"}, {"Get", "Đến"},
                {"There", "Đó"}, {"Hotel", "Khách sạn"}, {"View", "Quang cảnh"}, {"What", "Gì"},
                {"Order", "Gọi món"}, {"Dinner", "Bữa tối"}, {"Sorry", "Xin lỗi"}, {"But", "Nhưng"},
                {"Don't", "Không"}, {"Mean", "Ý nghĩa"}, {"Important", "Quan trọng"},
                {"Field", "Lĩnh vực"}, {"Believe", "Tin"}, {"We", "Chúng ta"}, {"During", "Trong"},
                {"Our", "Của chúng ta"}, {"Next", "Tiếp theo"}, {"Implementation", "Thực hiện"},
                {"Strategy", "Chiến lược"}, {"Requires", "Đòi hỏi"}, {"Careful", "Cẩn thận"},
                {"Planning", "Lập kế hoạch"}, {"Coordination", "Phối hợp"}, {"Among", "Giữa"},
                {"All", "Tất cả"}, {"Team", "Nhóm"}, {"Members", "Thành viên"}, {"Your", "Của bạn"},
                {"Feedback", "Phản hồi"}, {"Delivered", "Trình bày"}, {"Yesterday", "Hôm qua"},
                {"Afternoon", "Chiều"}, {"In", "Trong"}, {"Opinion", "Ý kiến"}, {"Current", "Hiện tại"},
                {"Situation", "Tình huống"}, {"Immediate", "Ngay lập tức"}, {"Attention", "Chú ý"},
                {"Decisive", "Quyết đoán"}, {"Action", "Hành động"}, {"From", "Từ"}, {"Stakeholders", "Bên liên quan"},
                {"Should", "Nên"}, {"Consider", "Xem xét"}, {"Long-term", "Dài hạn"}, {"Implications", "Tác động"},
                {"Decision", "Quyết định"}, {"Before", "Trước khi"}, {"Proceeding", "Tiếp tục"},
                {"Further", "Xa hơn"}, {"Such", "Như vậy"}, {"Talented", "Tài năng"}, {"Professionals", "Chuyên gia"},
                {"Despite", "Mặc dù"}, {"Challenges", "Thách thức"}, {"Faced", "Đối mặt"},
                {"Successfully", "Thành công"}, {"Ahead", "Trước"}, {"Schedule", "Thời hạn"},
                {"New", "Mới"}, {"Achieve", "Đạt được"}, {"Ambitious", "Tham vọng"}
            };

            await Task.Delay(50);
            return dictionary.TryGetValue(word, out var translation) ? translation : "(Chưa có bản dịch)";
        }

        private void SeedSentences()
        {
            var sentences = new[]
            {
                // EASY - Câu ngắn, từ thường ngày (5-8 từ)
                new SpeakingSentence { EnglishText = "Good morning! How are you today?", VietnameseTranslation = "Chào buổi sáng! Hôm nay bạn thế nào?", Category = "Daily", Level = "Easy" },
                new SpeakingSentence { EnglishText = "Thank you very much for your help.", VietnameseTranslation = "Cảm ơn bạn rất nhiều vì sự giúp đỡ.", Category = "Daily", Level = "Easy" },
                new SpeakingSentence { EnglishText = "The weather is beautiful today.", VietnameseTranslation = "Thời tiết hôm nay rất đẹp.", Category = "Daily", Level = "Easy" },
                new SpeakingSentence { EnglishText = "I like this book very much.", VietnameseTranslation = "Tôi rất thích quyển sách này.", Category = "Daily", Level = "Easy" },
                new SpeakingSentence { EnglishText = "What time is it now?", VietnameseTranslation = "Bây giờ là mấy giờ?", Category = "Daily", Level = "Easy" },
                new SpeakingSentence { EnglishText = "Nice to meet you!", VietnameseTranslation = "Rất vui được gặp bạn!", Category = "Daily", Level = "Easy" },
                new SpeakingSentence { EnglishText = "See you tomorrow!", VietnameseTranslation = "Hẹn gặp lại ngày mai!", Category = "Daily", Level = "Easy" },
                new SpeakingSentence { EnglishText = "I'm hungry. Let's eat.", VietnameseTranslation = "Tôi đói rồi. Đi ăn thôi.", Category = "Daily", Level = "Easy" },
                new SpeakingSentence { EnglishText = "This is my friend.", VietnameseTranslation = "Đây là bạn tôi.", Category = "Daily", Level = "Easy" },
                new SpeakingSentence { EnglishText = "I love coffee and tea.", VietnameseTranslation = "Tôi thích cà phê và trà.", Category = "Daily", Level = "Easy" },
                new SpeakingSentence { EnglishText = "Where do you live?", VietnameseTranslation = "Bạn sống ở đâu?", Category = "Daily", Level = "Easy" },
                new SpeakingSentence { EnglishText = "Have a nice day!", VietnameseTranslation = "Chúc bạn một ngày tốt lành!", Category = "Daily", Level = "Easy" },

                // MEDIUM - Câu dài hơn, từ vựng nâng cao (9-15 từ)
                new SpeakingSentence { EnglishText = "Could you please speak more slowly?", VietnameseTranslation = "Bạn có thể nói chậm hơn được không?", Category = "Communication", Level = "Medium" },
                new SpeakingSentence { EnglishText = "I would like to book a table for two people.", VietnameseTranslation = "Tôi muốn đặt bàn cho hai người.", Category = "Restaurant", Level = "Medium" },
                new SpeakingSentence { EnglishText = "Excuse me, where is the nearest subway station?", VietnameseTranslation = "Xin lỗi, trạm tàu điện ngầm gần nhất ở đâu?", Category = "Travel", Level = "Medium" },
                new SpeakingSentence { EnglishText = "I need some assistance with this issue, please.", VietnameseTranslation = "Tôi cần trợ giúp với vấn đề này.", Category = "Communication", Level = "Medium" },
                new SpeakingSentence { EnglishText = "Can you recommend a good restaurant in this area?", VietnameseTranslation = "Bạn có thể đề xuất nhà hàng ngon ở khu này không?", Category = "Restaurant", Level = "Medium" },
                new SpeakingSentence { EnglishText = "I would appreciate it if you could help me.", VietnameseTranslation = "Tôi sẽ rất biết ơn nếu bạn giúp tôi.", Category = "Communication", Level = "Medium" },
                new SpeakingSentence { EnglishText = "How long does it take to get there?", VietnameseTranslation = "Mất bao lâu để đến đó?", Category = "Travel", Level = "Medium" },
                new SpeakingSentence { EnglishText = "I'm trying to find the best solution for this problem.", VietnameseTranslation = "Tôi đang cố tìm giải pháp tốt nhất cho vấn đề này.", Category = "Business", Level = "Medium" },
                new SpeakingSentence { EnglishText = "Could you tell me more about your experience?", VietnameseTranslation = "Bạn có thể kể thêm về kinh nghiệm của bạn không?", Category = "Business", Level = "Medium" },
                new SpeakingSentence { EnglishText = "I'm looking for a hotel with a good view.", VietnameseTranslation = "Tôi đang tìm khách sạn có view đẹp.", Category = "Travel", Level = "Medium" },
                new SpeakingSentence { EnglishText = "What would you like to order for dinner?", VietnameseTranslation = "Bạn muốn gọi món gì cho bữa tối?", Category = "Restaurant", Level = "Medium" },
                new SpeakingSentence { EnglishText = "I'm sorry, but I don't understand what you mean.", VietnameseTranslation = "Xin lỗi, nhưng tôi không hiểu ý bạn.", Category = "Communication", Level = "Medium" },

                // HARD - Câu dài, từ vựng phức tạp (16+ từ)
                new SpeakingSentence { EnglishText = "I'm looking forward to working with you on this important project.", VietnameseTranslation = "Tôi mong được làm việc với bạn trong dự án quan trọng này.", Category = "Business", Level = "Hard" },
                new SpeakingSentence { EnglishText = "Could you elaborate on your previous experience in this field?", VietnameseTranslation = "Bạn có thể nói rõ hơn về kinh nghiệm trước đây trong lĩnh vực này không?", Category = "Business", Level = "Hard" },
                new SpeakingSentence { EnglishText = "I believe we need to discuss the details of this proposal during our next meeting.", VietnameseTranslation = "Tôi tin rằng chúng ta cần thảo luận chi tiết về đề xuất này trong cuộc họp tiếp theo.", Category = "Business", Level = "Hard" },
                new SpeakingSentence { EnglishText = "The implementation of this strategy requires careful planning and coordination among all team members.", VietnameseTranslation = "Việc thực hiện chiến lược này đòi hỏi lập kế hoạch cẩn thận và phối hợp giữa tất cả thành viên nhóm.", Category = "Business", Level = "Hard" },
                new SpeakingSentence { EnglishText = "I would appreciate your feedback on the presentation I delivered yesterday afternoon.", VietnameseTranslation = "Tôi sẽ rất trân trọng phản hồi của bạn về bài thuyết trình tôi đã trình bày chiều hôm qua.", Category = "Business", Level = "Hard" },
                new SpeakingSentence { EnglishText = "In my opinion, the current situation requires immediate attention and decisive action from all stakeholders.", VietnameseTranslation = "Theo ý kiến của tôi, tình hình hiện tại đòi hỏi sự chú ý ngay lập tức và hành động quyết đoán từ tất cả các bên liên quan.", Category = "Business", Level = "Hard" },
                new SpeakingSentence { EnglishText = "We should consider the long-term implications of this decision before proceeding further.", VietnameseTranslation = "Chúng ta nên xem xét những tác động dài hạn của quyết định này trước khi tiếp tục.", Category = "Business", Level = "Hard" },
                new SpeakingSentence { EnglishText = "I'm delighted to have this opportunity to collaborate with such talented professionals.", VietnameseTranslation = "Tôi rất vui mừng có cơ hội hợp tác với những chuyên gia tài năng như vậy.", Category = "Business", Level = "Hard" },
                new SpeakingSentence { EnglishText = "Despite the challenges we faced, the team successfully delivered the project ahead of schedule.", VietnameseTranslation = "Mặc dù những thách thức chúng tôi đã gặp, nhóm đã hoàn thành dự án thành công trước thời hạn.", Category = "Business", Level = "Hard" },
                new SpeakingSentence { EnglishText = "I'm excited to start this new journey together and achieve our ambitious goals.", VietnameseTranslation = "Tôi háo hức bắt đầu hành trình mới này cùng nhau và đạt được những mục tiêu đầy tham vọng của chúng ta.", Category = "Business", Level = "Hard" }
            };

            _context.Set<SpeakingSentence>().AddRange(sentences);
            _context.SaveChanges();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _waveIn?.Dispose();
            _writer?.Dispose();
            
            if (File.Exists(_audioFilePath))
            {
                try { File.Delete(_audioFilePath); } catch { }
            }
            
            base.OnFormClosing(e);
        }
    }
}
