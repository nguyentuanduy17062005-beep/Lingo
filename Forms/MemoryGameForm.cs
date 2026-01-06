namespace LingoAppNet8.Forms
{
    public class MemoryGameForm : Form
    {
        private MemoryGameControl gameControl = null!;

        public MemoryGameForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "LingoApp - Trò Chơi Ghi Nhớ";
            this.Size = new Size(860, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(244, 247, 246);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            gameControl = new MemoryGameControl
            {
                Location = new Point(10, 10),
                Dock = DockStyle.Fill
            };

            this.Controls.Add(gameControl);
        }
    }
}
