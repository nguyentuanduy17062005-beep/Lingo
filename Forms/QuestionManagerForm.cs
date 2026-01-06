using LingoAppNet8.Data;
using LingoAppNet8.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using System.Drawing.Drawing2D;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using OpenXmlText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace LingoAppNet8.Forms
{
    public partial class QuestionManagerForm : Form
    {
        private LingoDbContext dbContext;
        private DataGridView dgvQuestions;
        private Button btnAdd, btnEdit, btnDelete, btnImport, btnRefresh;
        private TextBox txtSearch, txtQuestion, txtOptionA, txtOptionB, txtOptionC, txtOptionD;
        private ComboBox cboDifficulty, cboCorrectAnswer, cboTense;
        private NumericUpDown numTimeLimit;
        private Panel panelEditor, panelHeader;
        private Label lblTitle;

        public QuestionManagerForm()
        {
            var scope = Program.ServiceProvider!.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<LingoDbContext>();
            InitializeComponent();
            LoadQuestions();
            LoadTenses();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Câu Hỏi";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.MinimumSize = new Size(1200, 700);

            // Header Panel
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(67, 97, 238)
            };
            panelHeader.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    panelHeader.ClientRectangle,
                    Color.FromArgb(67, 97, 238),
                    Color.FromArgb(115, 103, 240),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, panelHeader.ClientRectangle);
                }
            };

            lblTitle = new Label
            {
                Text = "📝 Quản Lý Câu Hỏi & Import File",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(30, 20)
            };
            panelHeader.Controls.Add(lblTitle);

            // Toolbar
            Panel toolbarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 10)
            };

            txtSearch = new TextBox
            {
                Location = new Point(20, 20),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "🔍 Tìm kiếm câu hỏi..."
            };
            txtSearch.TextChanged += (s, e) => LoadQuestions();

            btnRefresh = CreateModernButton("🔄 Làm mới", new Point(340, 18), new Size(120, 35));
            btnRefresh.Click += (s, e) => LoadQuestions();

            btnAdd = CreateModernButton("➕ Thêm mới", new Point(480, 18), new Size(130, 35));
            btnAdd.Click += BtnAdd_Click;
            btnAdd.BackColor = Color.FromArgb(76, 175, 80);

            btnEdit = CreateModernButton("✏️ Sửa", new Point(630, 18), new Size(100, 35));
            btnEdit.Click += BtnEdit_Click;
            btnEdit.BackColor = Color.FromArgb(33, 150, 243);

            btnDelete = CreateModernButton("🗑️ Xóa", new Point(750, 18), new Size(100, 35));
            btnDelete.Click += BtnDelete_Click;
            btnDelete.BackColor = Color.FromArgb(244, 67, 54);

            btnImport = CreateModernButton("📂 Import File (DOCX/TXT)", new Point(870, 18), new Size(220, 35));
            btnImport.Click += BtnImport_Click;
            btnImport.BackColor = Color.FromArgb(156, 39, 176);

            toolbarPanel.Controls.AddRange(new Control[] { txtSearch, btnRefresh, btnAdd, btnEdit, btnDelete, btnImport });

            // DataGridView
            dgvQuestions = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 10),
                RowTemplate = { Height = 45 },
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(248, 249, 250) },
                DefaultCellStyle = { SelectionBackColor = Color.FromArgb(103, 58, 183), SelectionForeColor = Color.White }
            };
            dgvQuestions.DoubleClick += (s, e) => BtnEdit_Click(s!, e);

            // Editor Panel
            panelEditor = new Panel
            {
                Dock = DockStyle.Right,
                Width = 450,
                BackColor = Color.White,
                Padding = new Padding(20),
                Visible = false
            };

            CreateEditorControls();

            // Layout
            Panel contentPanel = new Panel { Dock = DockStyle.Fill };
            contentPanel.Controls.Add(dgvQuestions);

            this.Controls.Add(contentPanel);
            this.Controls.Add(panelEditor);
            this.Controls.Add(toolbarPanel);
            this.Controls.Add(panelHeader);
        }

        private void CreateEditorControls()
        {
            int y = 10;

            Label lblEditorTitle = new Label
            {
                Text = "Chi tiết câu hỏi",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(10, y),
                Size = new Size(400, 35),
                ForeColor = Color.FromArgb(67, 97, 238)
            };
            panelEditor.Controls.Add(lblEditorTitle);
            y += 45;

            // Question
            panelEditor.Controls.Add(new Label
            {
                Text = "Câu hỏi:",
                Location = new Point(10, y),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
            y += 25;
            txtQuestion = new TextBox
            {
                Location = new Point(10, y),
                Size = new Size(410, 80),
                Multiline = true,
                Font = new Font("Segoe UI", 10),
                ScrollBars = ScrollBars.Vertical
            };
            panelEditor.Controls.Add(txtQuestion);
            y += 90;

            // Options
            string[] options = { "A", "B", "C", "D" };
            TextBox[] optionBoxes = new TextBox[4];

            for (int i = 0; i < 4; i++)
            {
                panelEditor.Controls.Add(new Label
                {
                    Text = $"Đáp án {options[i]}:",
                    Location = new Point(10, y),
                    Size = new Size(400, 20),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                });
                y += 22;
                optionBoxes[i] = new TextBox
                {
                    Location = new Point(10, y),
                    Size = new Size(410, 25),
                    Font = new Font("Segoe UI", 9)
                };
                panelEditor.Controls.Add(optionBoxes[i]);
                y += 32;
            }

            txtOptionA = optionBoxes[0];
            txtOptionB = optionBoxes[1];
            txtOptionC = optionBoxes[2];
            txtOptionD = optionBoxes[3];

            // Correct Answer
            panelEditor.Controls.Add(new Label
            {
                Text = "Đáp án đúng:",
                Location = new Point(10, y),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
            y += 22;
            cboCorrectAnswer = new ComboBox
            {
                Location = new Point(10, y),
                Size = new Size(195, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cboCorrectAnswer.Items.AddRange(new[] { "A", "B", "C", "D" });
            cboCorrectAnswer.SelectedIndex = 0;
            panelEditor.Controls.Add(cboCorrectAnswer);

            // Difficulty
            panelEditor.Controls.Add(new Label
            {
                Text = "Độ khó:",
                Location = new Point(220, y - 22),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
            cboDifficulty = new ComboBox
            {
                Location = new Point(220, y),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cboDifficulty.Items.AddRange(new[] { "Easy", "Normal", "Hard" });
            cboDifficulty.SelectedIndex = 1;
            panelEditor.Controls.Add(cboDifficulty);
            y += 35;

            // Time Limit
            panelEditor.Controls.Add(new Label
            {
                Text = "Thời gian (giây):",
                Location = new Point(10, y),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
            y += 22;
            numTimeLimit = new NumericUpDown
            {
                Location = new Point(10, y),
                Size = new Size(195, 25),
                Minimum = 10,
                Maximum = 300,
                Value = 60,
                Font = new Font("Segoe UI", 10)
            };
            panelEditor.Controls.Add(numTimeLimit);

            // Tense
            panelEditor.Controls.Add(new Label
            {
                Text = "Thì:",
                Location = new Point(220, y - 22),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
            cboTense = new ComboBox
            {
                Location = new Point(220, y),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                DisplayMember = "Name",
                ValueMember = "TenseId"
            };
            panelEditor.Controls.Add(cboTense);
            y += 40;

            // Buttons
            Button btnSave = CreateModernButton("💾 Lưu", new Point(10, y), new Size(200, 40));
            btnSave.BackColor = Color.FromArgb(76, 175, 80);
            btnSave.Click += BtnSave_Click;

            Button btnCancel = CreateModernButton("❌ Hủy", new Point(220, y), new Size(200, 40));
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.Click += (s, e) => { panelEditor.Visible = false; ClearEditor(); };

            panelEditor.Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private Button CreateModernButton(string text, Point location, Size size)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = size,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(103, 58, 183),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(81, 45, 168) }
            };
        }

        private void LoadQuestions()
        {
            var query = dbContext.QuizQuestions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();
                query = query.Where(q => q.Question.ToLower().Contains(search));
            }

            var questions = query.Select(q => new
            {
                q.QuestionId,
                CâuHỏi = q.Question.Length > 80 ? q.Question.Substring(0, 80) + "..." : q.Question,
                ĐápÁnĐúng = q.CorrectAnswer,
                ĐộKhó = q.Difficulty,
                ThoiGian = q.TimeLimit + "s",
                Thì = q.Tense != null ? q.Tense.Name : "N/A"
            }).ToList();

            dgvQuestions.DataSource = questions;

            if (dgvQuestions.Columns["QuestionId"] != null)
                dgvQuestions.Columns["QuestionId"]!.Visible = false;
        }

        private void LoadTenses()
        {
            var tenses = dbContext.TensesData.ToList();
            cboTense.DataSource = tenses;
            if (tenses.Any())
                cboTense.SelectedIndex = 0;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            ClearEditor();
            panelEditor.Tag = null; // null = Add mode
            panelEditor.Visible = true;
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn câu hỏi cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int questionId = (int)dgvQuestions.SelectedRows[0].Cells["QuestionId"].Value;
            var question = dbContext.QuizQuestions.Find(questionId);

            if (question != null)
            {
                txtQuestion.Text = question.Question;
                txtOptionA.Text = question.OptionA;
                txtOptionB.Text = question.OptionB;
                txtOptionC.Text = question.OptionC;
                txtOptionD.Text = question.OptionD;
                cboCorrectAnswer.SelectedItem = question.CorrectAnswer;
                cboDifficulty.SelectedItem = question.Difficulty;
                numTimeLimit.Value = question.TimeLimit;
                cboTense.SelectedValue = question.TenseId;

                panelEditor.Tag = questionId; // Store ID for edit mode
                panelEditor.Visible = true;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuestion.Text))
            {
                MessageBox.Show("Vui lòng nhập câu hỏi!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            QuizQuestion question;
            bool isNew = panelEditor.Tag == null;

            if (isNew)
            {
                question = new QuizQuestion();
                dbContext.QuizQuestions.Add(question);
            }
            else
            {
                int id = (int)panelEditor.Tag;
                question = dbContext.QuizQuestions.Find(id)!;
            }

            question.Question = txtQuestion.Text.Trim();
            question.OptionA = txtOptionA.Text.Trim();
            question.OptionB = txtOptionB.Text.Trim();
            question.OptionC = txtOptionC.Text.Trim();
            question.OptionD = txtOptionD.Text.Trim();
            question.CorrectAnswer = cboCorrectAnswer.SelectedItem?.ToString() ?? "A";
            question.Difficulty = cboDifficulty.SelectedItem?.ToString() ?? "Normal";
            question.TimeLimit = (int)numTimeLimit.Value;
            question.TenseId = (int)cboTense.SelectedValue;

            dbContext.SaveChanges();

            MessageBox.Show($"Đã {(isNew ? "thêm" : "cập nhật")} câu hỏi thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadQuestions();
            panelEditor.Visible = false;
            ClearEditor();
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvQuestions.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn câu hỏi cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa câu hỏi này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int questionId = (int)dgvQuestions.SelectedRows[0].Cells["QuestionId"].Value;
                var question = dbContext.QuizQuestions.Find(questionId);

                if (question != null)
                {
                    dbContext.QuizQuestions.Remove(question);
                    dbContext.SaveChanges();
                    MessageBox.Show("Đã xóa câu hỏi thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadQuestions();
                }
            }
        }

        private void BtnImport_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Word Documents (*.docx)|*.docx|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                openFileDialog.Title = "Chọn file để import câu hỏi";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string extension = Path.GetExtension(filePath).ToLower();

                    try
                    {
                        List<QuizQuestion> questions = new List<QuizQuestion>();

                        if (extension == ".docx")
                        {
                            questions = ImportFromDocx(filePath);
                        }
                        else if (extension == ".txt")
                        {
                            questions = ImportFromTxt(filePath);
                        }

                        if (questions.Any())
                        {
                            dbContext.QuizQuestions.AddRange(questions);
                            dbContext.SaveChanges();

                            MessageBox.Show($"Đã import thành công {questions.Count} câu hỏi!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadQuestions();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy câu hỏi nào trong file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi import file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private List<QuizQuestion> ImportFromDocx(string filePath)
        {
            List<QuizQuestion> questions = new List<QuizQuestion>();

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                var body = wordDoc.MainDocumentPart?.Document.Body;
                if (body == null) return questions;

                var paragraphs = body.Elements<OpenXmlParagraph>().Select(p => p.InnerText.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList();

                QuizQuestion? currentQuestion = null;
                int optionIndex = 0;

                foreach (var text in paragraphs)
                {
                    if (text.StartsWith("Q:") || text.StartsWith("Question:") || text.StartsWith("Câu"))
                    {
                        if (currentQuestion != null && !string.IsNullOrEmpty(currentQuestion.Question))
                        {
                            questions.Add(currentQuestion);
                        }

                        currentQuestion = new QuizQuestion
                        {
                            Question = text.Replace("Q:", "").Replace("Question:", "").Replace("Câu:", "").Trim(),
                            Difficulty = "Normal",
                            TimeLimit = 60,
                            TenseId = cboTense.Items.Count > 0 ? (int)cboTense.SelectedValue : 1
                        };
                        optionIndex = 0;
                    }
                    else if (currentQuestion != null && (text.StartsWith("A:") || text.StartsWith("A.") || text.StartsWith("A)")))
                    {
                        currentQuestion.OptionA = text.Substring(2).Trim();
                        optionIndex = 1;
                    }
                    else if (currentQuestion != null && (text.StartsWith("B:") || text.StartsWith("B.") || text.StartsWith("B)")))
                    {
                        currentQuestion.OptionB = text.Substring(2).Trim();
                        optionIndex = 2;
                    }
                    else if (currentQuestion != null && (text.StartsWith("C:") || text.StartsWith("C.") || text.StartsWith("C)")))
                    {
                        currentQuestion.OptionC = text.Substring(2).Trim();
                        optionIndex = 3;
                    }
                    else if (currentQuestion != null && (text.StartsWith("D:") || text.StartsWith("D.") || text.StartsWith("D)")))
                    {
                        currentQuestion.OptionD = text.Substring(2).Trim();
                        optionIndex = 4;
                    }
                    else if (currentQuestion != null && (text.StartsWith("Answer:") || text.StartsWith("Correct:") || text.StartsWith("Đáp án:")))
                    {
                        string answer = text.Replace("Answer:", "").Replace("Correct:", "").Replace("Đáp án:", "").Trim().ToUpper();
                        if (answer.Length > 0 && "ABCD".Contains(answer[0]))
                        {
                            currentQuestion.CorrectAnswer = answer[0].ToString();
                        }
                    }
                }

                if (currentQuestion != null && !string.IsNullOrEmpty(currentQuestion.Question))
                {
                    questions.Add(currentQuestion);
                }
            }

            return questions;
        }

        private List<QuizQuestion> ImportFromTxt(string filePath)
        {
            List<QuizQuestion> questions = new List<QuizQuestion>();
            var lines = File.ReadAllLines(filePath).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

            QuizQuestion? currentQuestion = null;

            foreach (var line in lines)
            {
                string trimmed = line.Trim();

                if (trimmed.StartsWith("Q:") || trimmed.StartsWith("Question:") || trimmed.StartsWith("Câu"))
                {
                    if (currentQuestion != null && !string.IsNullOrEmpty(currentQuestion.Question))
                    {
                        questions.Add(currentQuestion);
                    }

                    currentQuestion = new QuizQuestion
                    {
                        Question = trimmed.Replace("Q:", "").Replace("Question:", "").Replace("Câu:", "").Trim(),
                        Difficulty = "Normal",
                        TimeLimit = 60,
                        TenseId = cboTense.Items.Count > 0 ? (int)cboTense.SelectedValue : 1
                    };
                }
                else if (currentQuestion != null && (trimmed.StartsWith("A:") || trimmed.StartsWith("A.") || trimmed.StartsWith("A)")))
                {
                    currentQuestion.OptionA = trimmed.Substring(2).Trim();
                }
                else if (currentQuestion != null && (trimmed.StartsWith("B:") || trimmed.StartsWith("B.") || trimmed.StartsWith("B)")))
                {
                    currentQuestion.OptionB = trimmed.Substring(2).Trim();
                }
                else if (currentQuestion != null && (trimmed.StartsWith("C:") || trimmed.StartsWith("C.") || trimmed.StartsWith("C)")))
                {
                    currentQuestion.OptionC = trimmed.Substring(2).Trim();
                }
                else if (currentQuestion != null && (trimmed.StartsWith("D:") || trimmed.StartsWith("D.") || trimmed.StartsWith("D)")))
                {
                    currentQuestion.OptionD = trimmed.Substring(2).Trim();
                }
                else if (currentQuestion != null && (trimmed.StartsWith("Answer:") || trimmed.StartsWith("Correct:") || trimmed.StartsWith("Đáp án:")))
                {
                    string answer = trimmed.Replace("Answer:", "").Replace("Correct:", "").Replace("Đáp án:", "").Trim().ToUpper();
                    if (answer.Length > 0 && "ABCD".Contains(answer[0]))
                    {
                        currentQuestion.CorrectAnswer = answer[0].ToString();
                    }
                }
            }

            if (currentQuestion != null && !string.IsNullOrEmpty(currentQuestion.Question))
            {
                questions.Add(currentQuestion);
            }

            return questions;
        }

        private void ClearEditor()
        {
            txtQuestion.Clear();
            txtOptionA.Clear();
            txtOptionB.Clear();
            txtOptionC.Clear();
            txtOptionD.Clear();
            cboCorrectAnswer.SelectedIndex = 0;
            cboDifficulty.SelectedIndex = 1;
            numTimeLimit.Value = 60;
            if (cboTense.Items.Count > 0)
                cboTense.SelectedIndex = 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
