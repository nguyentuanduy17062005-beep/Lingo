using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LingoAppNet8.Data;
using LingoAppNet8.Models;
using Microsoft.EntityFrameworkCore;

namespace LingoAppNet8.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private TextBox txtEmail = null!;
        private Button btnLogin = null!;
        private Button btnRegister = null!;
        private CheckBox chkRemember = null!;
        private Panel headerPanel = null!;

        public LoginForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "LingoApp - Đăng Nhập";
            this.Size = new Size(600, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new Size(500, 650);
            this.AutoScroll = false;

            // Header Panel with Gradient
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 280,
                BackColor = Color.FromArgb(58, 134, 255)
            };
            headerPanel.Paint += (s, e) =>
            {
                LinearGradientBrush brush = new LinearGradientBrush(
                    headerPanel.ClientRectangle,
                    Color.FromArgb(58, 134, 255),
                    Color.FromArgb(88, 204, 2),
                    LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(brush, headerPanel.ClientRectangle);
            };

            Label lblLogo = new Label
            {
                Text = "🦉",
                Font = new Font("Segoe UI", 48F),
                Size = new Size(100, 80),
                Location = new Point(250, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            Label lblTitle = new Label
            {
                Text = "LINGO",
                Font = new Font("Segoe UI", 36F, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(300, 80),
                Location = new Point(150, 150),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Center labels on resize
            headerPanel.Resize += (s, e) =>
            {
                lblLogo.Left = (headerPanel.Width - lblLogo.Width) / 2;
                lblTitle.Left = (headerPanel.Width - lblTitle.Width) / 2;
            };

            headerPanel.Controls.AddRange(new Control[] { lblLogo, lblTitle });

            // Content Panel - Centered
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250),
                AutoScroll = false
            };

            // Form Container (centered, fixed width)
            Panel formContainer = new Panel
            {
                Width = 400,
                Height = 450,
                BackColor = Color.Transparent
            };

            // Center form container
            contentPanel.Resize += (s, e) =>
            {
                formContainer.Left = (contentPanel.Width - formContainer.Width) / 2;
                formContainer.Top = 70;
            };

            // Subtitle
            Label lblSubtitle = new Label
            {
                Text = "Học Tiếng Anh Mỗi Ngày! 🚀",
                Font = new Font("Segoe UI", 13F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(0, 0),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Username Label
            Label lblUsername = new Label
            {
                Text = "TÊN ĐĂNG NHẬP",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(0, 50),
                Size = new Size(400, 25)
            };

            // Username TextBox
            txtUsername = new TextBox
            {
                Font = new Font("Segoe UI", 13F),
                Location = new Point(0, 80),
                Size = new Size(400, 35),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Email Label
            Label lblEmail = new Label
            {
                Text = "EMAIL",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(0, 135),
                Size = new Size(400, 25)
            };

            // Email TextBox
            txtEmail = new TextBox
            {
                Font = new Font("Segoe UI", 13F),
                Location = new Point(0, 165),
                Size = new Size(400, 35),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Remember CheckBox
            chkRemember = new CheckBox
            {
                Text = "Nhớ tài khoản",
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 220),
                Size = new Size(200, 30),
                Checked = true,
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            // Login Button
            btnLogin = CreateModernButton("ĐĂNG NHẬP", Color.FromArgb(88, 204, 2));
            btnLogin.Location = new Point(0, 270);
            btnLogin.Size = new Size(200, 55);
            btnLogin.Click += BtnLogin_Click;

            // Register Button
            btnRegister = CreateModernButton("ĐĂNG KÝ", Color.FromArgb(33, 150, 243));
            btnRegister.Location = new Point(200, 270);
            btnRegister.Size = new Size(200, 55);
            btnRegister.Click += BtnRegister_Click;

            // Footer Label
            Label lblFooter = new Label
            {
                Text = "Chào mừng bạn đến với LingoApp! 💚",
                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(0, 350),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Add all to form container
            formContainer.Controls.AddRange(new Control[] {
                lblSubtitle,
                lblUsername, txtUsername,
                lblEmail, txtEmail,
                chkRemember,
                btnLogin, btnRegister,
                lblFooter
            });

            contentPanel.Controls.Add(formContainer);
            this.Controls.Add(contentPanel);
            this.Controls.Add(headerPanel);
        }

        private Button CreateModernButton(string text, Color color)
        {
            Button btn = new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.1f);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color, 0.1f);
            return btn;
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Username và Email!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var optionsBuilder = new DbContextOptionsBuilder<LingoDbContext>();
            optionsBuilder.UseSqlServer("Server=LAPTOP-7TOIFEJI\\SQLEXPRESS;Database=LingoDb;Integrated Security=True;TrustServerCertificate=True;");
            
            using (var context = new LingoDbContext(optionsBuilder.Options))
            {
                var user = context.Users.FirstOrDefault(u =>
                    u.Username == username && u.Email == email);

                if (user == null)
                {
                    MessageBox.Show("Username hoặc Email không đúng!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show($"Đăng nhập thành công!\nChào mừng {username} 🎉\n\nXP: {user.TotalXP}\nLevel: {user.CurrentLevel}", 
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open Main Form
                this.Hide();
                var mainForm = new MainForm(user);
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Username và Email!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var optionsBuilder = new DbContextOptionsBuilder<LingoDbContext>();
            optionsBuilder.UseSqlServer("Server=LAPTOP-7TOIFEJI\\SQLEXPRESS;Database=LingoDb;Integrated Security=True;TrustServerCertificate=True;");
            
            using (var context = new LingoDbContext(optionsBuilder.Options))
            {
                var existingUser = context.Users.FirstOrDefault(u =>
                    u.Username == username || u.Email == email);

                if (existingUser != null)
                {
                    MessageBox.Show("Username hoặc Email đã tồn tại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var newUser = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = "demo123",
                    CreatedDate = DateTime.Now,
                    CurrentLevel = 1,
                    TotalXP = 0,
                    CurrentStreak = 0,
                    LongestStreak = 0
                };

                context.Users.Add(newUser);
                context.SaveChanges();

                MessageBox.Show($"Đăng ký thành công!\nChào mừng {username} 🎉", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtUsername.Clear();
                txtEmail.Clear();
            }
        }
    }
}
