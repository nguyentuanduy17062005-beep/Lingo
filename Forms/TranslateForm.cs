using LingoAppNet8.Services;

namespace LingoAppNet8.Forms
{
    public partial class TranslateForm : Form
    {
        private TextBox txtSource = null!;
        private TextBox txtTranslation = null!;
        private Button btnTranslate = null!;
        private Button btnSwap = null!;
        private ComboBox cboSourceLang = null!;
        private ComboBox cboTargetLang = null!;
        private Label lblSource = null!;
        private Label lblTarget = null!;
        private TranslationService translationService;

        public TranslateForm()
        {
            translationService = new TranslationService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Dịch Tiếng Anh - Google Translate";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Source Language Label
            lblSource = new Label
            {
                Location = new Point(20, 20),
                Size = new Size(150, 25),
                Text = "Tiếng nguồn:",
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            // Source Language ComboBox
            cboSourceLang = new ComboBox
            {
                Location = new Point(20, 50),
                Size = new Size(350, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboSourceLang.Items.AddRange(new object[] { "Tiếng Việt", "Tiếng Anh" });
            cboSourceLang.SelectedIndex = 0;

            // Swap Button
            btnSwap = new Button
            {
                Location = new Point(400, 50),
                Size = new Size(80, 30),
                Text = "⇄",
                Font = new Font("Arial", 16, FontStyle.Bold),
                BackColor = Color.LightGray
            };
            btnSwap.Click += BtnSwap_Click;

            // Target Language Label
            lblTarget = new Label
            {
                Location = new Point(510, 20),
                Size = new Size(150, 25),
                Text = "Ngôn ngữ đích:",
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            // Target Language ComboBox
            cboTargetLang = new ComboBox
            {
                Location = new Point(510, 50),
                Size = new Size(350, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboTargetLang.Items.AddRange(new object[] { "Tiếng Việt", "Tiếng Anh" });
            cboTargetLang.SelectedIndex = 1;

            // Source TextBox
            txtSource = new TextBox
            {
                Location = new Point(20, 100),
                Size = new Size(840, 180),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 11),
                PlaceholderText = "Nhập văn bản cần dịch..."
            };

            // Translate Button
            btnTranslate = new Button
            {
                Location = new Point(350, 300),
                Size = new Size(180, 45),
                Text = "Dịch",
                Font = new Font("Arial", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnTranslate.Click += BtnTranslate_Click;

            // Translation TextBox
            txtTranslation = new TextBox
            {
                Location = new Point(20, 360),
                Size = new Size(840, 180),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 11),
                ReadOnly = true,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            this.Controls.AddRange(new Control[] {
                lblSource, cboSourceLang, btnSwap, lblTarget, cboTargetLang,
                txtSource, btnTranslate, txtTranslation
            });
        }

        private async void BtnTranslate_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSource.Text))
            {
                MessageBox.Show("Vui lòng nhập văn bản cần dịch!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnTranslate.Enabled = false;
            btnTranslate.Text = "Đang dịch...";
            txtTranslation.Text = "Đang xử lý...";

            try
            {
                string sourceLang = cboSourceLang.SelectedItem?.ToString() == "Tiếng Việt" ? "vi" : "en";
                string targetLang = cboTargetLang.SelectedItem?.ToString() == "Tiếng Việt" ? "vi" : "en";

                string translatedText = await translationService.TranslateAsync(
                    txtSource.Text, targetLang, sourceLang);

                txtTranslation.Text = translatedText;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi dịch: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTranslation.Text = "";
            }
            finally
            {
                btnTranslate.Enabled = true;
                btnTranslate.Text = "Dịch";
            }
        }

        private void BtnSwap_Click(object? sender, EventArgs e)
        {
            int tempIndex = cboSourceLang.SelectedIndex;
            cboSourceLang.SelectedIndex = cboTargetLang.SelectedIndex;
            cboTargetLang.SelectedIndex = tempIndex;

            string tempText = txtSource.Text;
            txtSource.Text = txtTranslation.Text;
            txtTranslation.Text = tempText;
        }
    }
}
