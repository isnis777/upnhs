using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QLTK;

public class AddAccountForm : Form
{
    public List<(string username, string password, int server)> AddedAccounts { get; } = new();

    private TabControl tabControl;
    private TabPage tabBatch;
    private TabPage tabSingle;

    // Batch Tab controls
    private TextBox txtBatch;
    private NumericUpDown numBatchServer;
    private Button btnImportBatch;
    private Button btnCancelBatch;

    // Single Tab controls
    private TextBox txtSingleUser;
    private TextBox txtSinglePass;
    private NumericUpDown numSingleServer;
    private Button btnAddSingle;
    private Button btnCloseSingle;
    private Label lblSingleStatus;

    public AddAccountForm(int defaultServer = 1)
    {
        InitializeComponents(defaultServer);
    }

    private void InitializeComponents(int defaultServer)
    {
        Text = "Thêm Tài Khoản Thật - QLTK";
        Size = new Size(560, 480);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Font = new Font("Segoe UI", 9.5F);
        BackColor = Color.FromArgb(245, 247, 250);

        tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
            Padding = new Point(14, 8)
        };

        // --- Tab 1: Dán danh sách hàng loạt ---
        tabBatch = new TabPage("📋 Dán danh sách hàng loạt")
        {
            BackColor = Color.White,
            Padding = new Padding(12)
        };

        Label lblBatchPrompt = new Label
        {
            Text = "Dán danh sách tài khoản thật vào khung bên dưới (mỗi nick 1 dòng):",
            Location = new Point(12, 12),
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(44, 62, 80)
        };

        Label lblBatchHint = new Label
        {
            Text = "💡 Hỗ trợ định dạng: taikhoan|matkhau hoặc taikhoan|matkhau|server (dùng | hoặc : hoặc Tab)",
            Location = new Point(12, 34),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = Color.FromArgb(127, 140, 141)
        };

        txtBatch = new TextBox
        {
            Location = new Point(12, 58),
            Size = new Size(510, 260),
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            Font = new Font("Consolas", 10F),
            WordWrap = false
        };

        Label lblBatchServer = new Label
        {
            Text = "Server mặc định:",
            Location = new Point(12, 332),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F)
        };

        numBatchServer = new NumericUpDown
        {
            Location = new Point(120, 330),
            Size = new Size(70, 25),
            Minimum = 1,
            Maximum = 50,
            Value = Math.Max(1, Math.Min(50, defaultServer))
        };

        btnImportBatch = new Button
        {
            Text = "➕ Thêm danh sách",
            Location = new Point(275, 326),
            Size = new Size(140, 32),
            BackColor = Color.FromArgb(41, 128, 185),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnImportBatch.Click += BtnImportBatch_Click;

        btnCancelBatch = new Button
        {
            Text = "Hủy",
            Location = new Point(425, 326),
            Size = new Size(95, 32),
            BackColor = Color.FromArgb(149, 165, 166),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            DialogResult = DialogResult.Cancel,
            Cursor = Cursors.Hand
        };

        tabBatch.Controls.Add(lblBatchPrompt);
        tabBatch.Controls.Add(lblBatchHint);
        tabBatch.Controls.Add(txtBatch);
        tabBatch.Controls.Add(lblBatchServer);
        tabBatch.Controls.Add(numBatchServer);
        tabBatch.Controls.Add(btnImportBatch);
        tabBatch.Controls.Add(btnCancelBatch);

        // --- Tab 2: Thêm từng tài khoản lẻ ---
        tabSingle = new TabPage("👤 Thêm từng tài khoản lẻ")
        {
            BackColor = Color.White,
            Padding = new Padding(20)
        };

        Label lblSingleUser = new Label
        {
            Text = "Tên tài khoản (Username):",
            Location = new Point(30, 25),
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
        };

        txtSingleUser = new TextBox
        {
            Location = new Point(30, 48),
            Size = new Size(460, 28),
            Font = new Font("Segoe UI", 10F)
        };

        Label lblSinglePass = new Label
        {
            Text = "Mật khẩu (Password):",
            Location = new Point(30, 90),
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
        };

        txtSinglePass = new TextBox
        {
            Location = new Point(30, 113),
            Size = new Size(460, 28),
            Font = new Font("Segoe UI", 10F)
        };

        Label lblSingleServer = new Label
        {
            Text = "Server:",
            Location = new Point(30, 155),
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
        };

        numSingleServer = new NumericUpDown
        {
            Location = new Point(30, 178),
            Size = new Size(100, 28),
            Minimum = 1,
            Maximum = 50,
            Value = Math.Max(1, Math.Min(50, defaultServer)),
            Font = new Font("Segoe UI", 10F)
        };

        btnAddSingle = new Button
        {
            Text = "➕ Thêm tài khoản này",
            Location = new Point(30, 225),
            Size = new Size(180, 36),
            BackColor = Color.FromArgb(39, 174, 96),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnAddSingle.Click += BtnAddSingle_Click;

        btnCloseSingle = new Button
        {
            Text = "Đóng",
            Location = new Point(220, 225),
            Size = new Size(100, 36),
            BackColor = Color.FromArgb(149, 165, 166),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5F),
            Cursor = Cursors.Hand
        };
        btnCloseSingle.Click += (s, e) => Close();

        lblSingleStatus = new Label
        {
            Location = new Point(30, 275),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = Color.FromArgb(39, 174, 96),
            Text = ""
        };

        tabSingle.Controls.Add(lblSingleUser);
        tabSingle.Controls.Add(txtSingleUser);
        tabSingle.Controls.Add(lblSinglePass);
        tabSingle.Controls.Add(txtSinglePass);
        tabSingle.Controls.Add(lblSingleServer);
        tabSingle.Controls.Add(numSingleServer);
        tabSingle.Controls.Add(btnAddSingle);
        tabSingle.Controls.Add(btnCloseSingle);
        tabSingle.Controls.Add(lblSingleStatus);

        tabControl.TabPages.Add(tabBatch);
        tabControl.TabPages.Add(tabSingle);
        Controls.Add(tabControl);
    }

    private void BtnImportBatch_Click(object? sender, EventArgs e)
    {
        string text = txtBatch.Text.Trim();
        if (string.IsNullOrEmpty(text))
        {
            MessageBox.Show("Vui lòng dán danh sách tài khoản vào ô!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string[] lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int defaultSv = (int)numBatchServer.Value;
        int countSuccess = 0;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(new char[] { '|', ':', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                string u = parts[0].Trim();
                string p = parts[1].Trim();
                int sv = defaultSv;
                if (parts.Length >= 3 && int.TryParse(parts[2].Trim(), out int parsedSv) && parsedSv > 0)
                {
                    sv = parsedSv;
                }
                AddedAccounts.Add((u, p, sv));
                countSuccess++;
            }
        }

        if (countSuccess == 0)
        {
            MessageBox.Show("Không tìm thấy dòng tài khoản hợp lệ nào!\nVui lòng nhập đúng định dạng: taikhoan|matkhau", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void BtnAddSingle_Click(object? sender, EventArgs e)
    {
        string u = txtSingleUser.Text.Trim();
        string p = txtSinglePass.Text.Trim();
        int sv = (int)numSingleServer.Value;

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
        {
            MessageBox.Show("Vui lòng nhập đầy đủ Tên tài khoản và Mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        AddedAccounts.Add((u, p, sv));
        lblSingleStatus.Text = $"✔ Đã thêm nick [{u}] (SV {sv}) thành công! Bạn có thể nhập tiếp nick khác.";
        txtSingleUser.Clear();
        txtSinglePass.Clear();
        txtSingleUser.Focus();
    }
}
