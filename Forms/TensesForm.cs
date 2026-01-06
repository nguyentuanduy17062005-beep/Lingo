using LingoAppNet8.Models;
using LingoAppNet8.Data;

namespace LingoAppNet8.Forms
{
    public partial class TensesForm : Form
    {
        private LingoDbContext dbContext;
        private ListBox lstTenses = null!;
        private RichTextBox rtbTenseDetails = null!;
        private Label lblTitle = null!;
        private Panel panelDetails = null!;

        public TensesForm(LingoDbContext context)
        {
            dbContext = context;
            InitializeComponent();
            LoadTenses();
        }

        private void InitializeComponent()
        {
            this.Text = "Kho Các Thì Tiếng Anh";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title
            lblTitle = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(950, 40),
                Text = "📚 Kho Các Thì Tiếng Anh",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 212),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Tenses ListBox
            lstTenses = new ListBox
            {
                Location = new Point(20, 80),
                Size = new Size(300, 560),
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(245, 245, 245)
            };
            lstTenses.SelectedIndexChanged += LstTenses_SelectedIndexChanged;

            // Details Panel
            panelDetails = new Panel
            {
                Location = new Point(340, 80),
                Size = new Size(630, 560),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                AutoScroll = true
            };

            rtbTenseDetails = new RichTextBox
            {
                Location = new Point(10, 10),
                Size = new Size(600, 530),
                Font = new Font("Arial", 10),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White
            };

            panelDetails.Controls.Add(rtbTenseDetails);

            this.Controls.AddRange(new Control[] { lblTitle, lstTenses, panelDetails });
        }

        private void LoadTenses()
        {
            var tenses = dbContext.TensesData.OrderBy(t => t.Level).ThenBy(t => t.TenseId).ToList();
            
            lstTenses.Items.Clear();
            foreach (var tense in tenses)
            {
                lstTenses.Items.Add(tense);
            }

            lstTenses.DisplayMember = "VietnameseName";

            if (lstTenses.Items.Count > 0)
            {
                lstTenses.SelectedIndex = 0;
            }
        }

        private void LstTenses_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstTenses.SelectedItem is TenseData tense)
            {
                DisplayTenseDetails(tense);
            }
        }

        private void DisplayTenseDetails(TenseData tense)
        {
            rtbTenseDetails.Clear();
            rtbTenseDetails.SelectionFont = new Font("Arial", 14, FontStyle.Bold);
            rtbTenseDetails.SelectionColor = Color.FromArgb(0, 120, 212);
            rtbTenseDetails.AppendText($"{tense.Name}\n");
            rtbTenseDetails.AppendText($"{tense.VietnameseName}\n\n");

            rtbTenseDetails.SelectionFont = new Font("Arial", 11, FontStyle.Bold);
            rtbTenseDetails.SelectionColor = Color.Black;
            rtbTenseDetails.AppendText("📖 Mô tả:\n");
            rtbTenseDetails.SelectionFont = new Font("Arial", 10);
            rtbTenseDetails.AppendText($"{tense.Description}\n\n");

            rtbTenseDetails.SelectionFont = new Font("Arial", 11, FontStyle.Bold);
            rtbTenseDetails.AppendText("📝 Cấu trúc:\n");
            rtbTenseDetails.SelectionFont = new Font("Arial", 10);
            rtbTenseDetails.SelectionColor = Color.DarkGreen;
            rtbTenseDetails.AppendText($"{tense.Structure}\n\n");

            rtbTenseDetails.SelectionFont = new Font("Arial", 11, FontStyle.Bold);
            rtbTenseDetails.SelectionColor = Color.Black;
            rtbTenseDetails.AppendText("💡 Cách dùng:\n");
            rtbTenseDetails.SelectionFont = new Font("Arial", 10);
            rtbTenseDetails.AppendText($"{tense.Usage}\n\n");

            rtbTenseDetails.SelectionFont = new Font("Arial", 11, FontStyle.Bold);
            rtbTenseDetails.AppendText("📋 Ví dụ:\n");
            rtbTenseDetails.SelectionFont = new Font("Arial", 10);
            rtbTenseDetails.SelectionColor = Color.DarkBlue;
            rtbTenseDetails.AppendText($"{tense.Examples}\n\n");

            rtbTenseDetails.SelectionFont = new Font("Arial", 11, FontStyle.Bold);
            rtbTenseDetails.SelectionColor = Color.Black;
            rtbTenseDetails.AppendText("⏰ Dấu hiệu nhận biết:\n");
            rtbTenseDetails.SelectionFont = new Font("Arial", 10);
            rtbTenseDetails.SelectionColor = Color.DarkOrange;
            rtbTenseDetails.AppendText($"{tense.TimeMarkers}\n");

            rtbTenseDetails.SelectionStart = 0;
            rtbTenseDetails.ScrollToCaret();
        }
    }
}
