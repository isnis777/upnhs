namespace QLTK;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
        System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
        System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
        System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
        System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
        System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();

        pnlTop = new Panel();
        flowTop = new FlowLayoutPanel();
        lblServer = new Label();
        numServer = new NumericUpDown();
        lblThreads = new Label();
        numThreads = new NumericUpDown();
        lblDelayTab = new Label();
        numDelayTab = new NumericUpDown();
        lblScreenSize = new Label();
        txtScreenSize = new TextBox();
        chkAutoTile = new CheckBox();
        chkRandomName = new CheckBox();
        chkDyTest = new CheckBox();

        flowGamePath = new FlowLayoutPanel();
        lblGamePath = new Label();
        txtGamePath = new TextBox();
        btnBrowseGame = new Button();

        flowOpt = new FlowLayoutPanel();
        chkOptimizeRam = new CheckBox();
        chkBlackScreen = new CheckBox();
        lblFps = new Label();
        numFps = new NumericUpDown();
        btnCleanRam = new Button();
        
        flowButtons = new FlowLayoutPanel();
        btnAddRow = new Button();
        btnDelete = new Button();
        btnStart = new Button();
        btnStop = new Button();
        btnAuto = new Button();
        btnStopAuto = new Button();
        btnReset = new Button();
        btnTileWindows = new Button();
        btnOpenLog = new Button();

        dgvAccounts = new DataGridView();
        colSTT = new DataGridViewTextBoxColumn();
        colUserAo = new DataGridViewTextBoxColumn();
        colCharName = new DataGridViewTextBoxColumn();
        colServer = new DataGridViewTextBoxColumn();
        colPlanet = new DataGridViewTextBoxColumn();
        colPower = new DataGridViewTextBoxColumn();
        colGold = new DataGridViewTextBoxColumn();
        colStatus = new DataGridViewTextBoxColumn();
        colPID = new DataGridViewTextBoxColumn();

        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();

        pnlTop.SuspendLayout();
        flowGamePath.SuspendLayout();
        flowTop.SuspendLayout();
        flowOpt.SuspendLayout();
        flowButtons.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numServer).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numThreads).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numDelayTab).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numFps).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvAccounts).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();

        // pnlTop
        pnlTop.BackColor = Color.FromArgb(245, 247, 250);
        pnlTop.BorderStyle = BorderStyle.FixedSingle;
        pnlTop.Controls.Add(flowButtons);
        pnlTop.Controls.Add(flowOpt);
        pnlTop.Controls.Add(flowTop);
        pnlTop.Controls.Add(flowGamePath);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Padding = new Padding(8);
        pnlTop.Size = new Size(960, 160);
        pnlTop.TabIndex = 0;

        // flowGamePath
        flowGamePath.Dock = DockStyle.Top;
        flowGamePath.FlowDirection = FlowDirection.LeftToRight;
        flowGamePath.Location = new Point(8, 8);
        flowGamePath.Name = "flowGamePath";
        flowGamePath.Size = new Size(942, 34);
        flowGamePath.TabIndex = 0;
        flowGamePath.WrapContents = false;

        // lblGamePath
        lblGamePath.AutoSize = true;
        lblGamePath.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        lblGamePath.Location = new Point(3, 6);
        lblGamePath.Margin = new Padding(3, 6, 3, 0);
        lblGamePath.Name = "lblGamePath";
        lblGamePath.Size = new Size(98, 17);
        lblGamePath.Text = "File Game (.exe):";

        // txtGamePath
        txtGamePath.Font = new Font("Segoe UI", 9.5F);
        txtGamePath.Location = new Point(107, 3);
        txtGamePath.Margin = new Padding(3, 3, 10, 3);
        txtGamePath.Name = "txtGamePath";
        txtGamePath.Size = new Size(600, 24);
        txtGamePath.Text = @"D:\DragonBoy250_pc\DragonBoy250_pc\DragonBoy250.exe";

        // btnBrowseGame
        btnBrowseGame.BackColor = Color.FromArgb(52, 152, 219);
        btnBrowseGame.FlatStyle = FlatStyle.Flat;
        btnBrowseGame.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnBrowseGame.ForeColor = Color.White;
        btnBrowseGame.Location = new Point(720, 2);
        btnBrowseGame.Margin = new Padding(2);
        btnBrowseGame.Name = "btnBrowseGame";
        btnBrowseGame.Size = new Size(115, 26);
        btnBrowseGame.Text = "📁 Chọn file...";
        btnBrowseGame.UseVisualStyleBackColor = false;
        btnBrowseGame.Click += BtnBrowseGame_Click;

        flowGamePath.Controls.Add(lblGamePath);
        flowGamePath.Controls.Add(txtGamePath);
        flowGamePath.Controls.Add(btnBrowseGame);

        // flowTop (Inputs & Checkboxes)
        flowTop.Dock = DockStyle.Top;
        flowTop.FlowDirection = FlowDirection.LeftToRight;
        flowTop.Location = new Point(8, 42);
        flowTop.Name = "flowTop";
        flowTop.Size = new Size(942, 34);
        flowTop.TabIndex = 1;
        flowTop.WrapContents = false;

        // lblServer
        lblServer.AutoSize = true;
        lblServer.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        lblServer.Location = new Point(3, 6);
        lblServer.Margin = new Padding(3, 6, 3, 0);
        lblServer.Name = "lblServer";
        lblServer.Size = new Size(51, 17);
        lblServer.Text = "Server:";

        // numServer
        numServer.Font = new Font("Segoe UI", 9.5F);
        numServer.Location = new Point(60, 3);
        numServer.Margin = new Padding(3, 3, 16, 3);
        numServer.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numServer.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
        numServer.Name = "numServer";
        numServer.Size = new Size(60, 24);
        numServer.Value = new decimal(new int[] { 1, 0, 0, 0 });

        // lblThreads
        lblThreads.AutoSize = true;
        lblThreads.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        lblThreads.Location = new Point(139, 6);
        lblThreads.Margin = new Padding(3, 6, 3, 0);
        lblThreads.Name = "lblThreads";
        lblThreads.Size = new Size(66, 17);
        lblThreads.Text = "Số luồng:";

        // numThreads
        numThreads.Font = new Font("Segoe UI", 9.5F);
        numThreads.Location = new Point(211, 3);
        numThreads.Margin = new Padding(3, 3, 20, 3);
        numThreads.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numThreads.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
        numThreads.Name = "numThreads";
        numThreads.Size = new Size(65, 24);
        numThreads.Value = new decimal(new int[] { 1, 0, 0, 0 });

        // lblDelayTab
        lblDelayTab.AutoSize = true;
        lblDelayTab.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        lblDelayTab.Location = new Point(282, 6);
        lblDelayTab.Margin = new Padding(3, 6, 3, 0);
        lblDelayTab.Name = "lblDelayTab";
        lblDelayTab.Size = new Size(88, 17);
        lblDelayTab.Text = "Delay mở (s):";

        // numDelayTab
        numDelayTab.Font = new Font("Segoe UI", 9.5F);
        numDelayTab.Location = new Point(373, 3);
        numDelayTab.Margin = new Padding(3, 3, 16, 3);
        numDelayTab.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numDelayTab.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
        numDelayTab.Name = "numDelayTab";
        numDelayTab.Size = new Size(50, 24);
        numDelayTab.Value = new decimal(new int[] { 3, 0, 0, 0 });

        // lblScreenSize
        lblScreenSize.AutoSize = true;
        lblScreenSize.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        lblScreenSize.Location = new Point(299, 6);
        lblScreenSize.Margin = new Padding(3, 6, 3, 0);
        lblScreenSize.Name = "lblScreenSize";
        lblScreenSize.Size = new Size(135, 17);
        lblScreenSize.Text = "Kích thước (cao x rộng):";

        // txtScreenSize
        txtScreenSize.Font = new Font("Segoe UI", 9.5F);
        txtScreenSize.Location = new Point(440, 3);
        txtScreenSize.Margin = new Padding(3, 3, 20, 3);
        txtScreenSize.Name = "txtScreenSize";
        txtScreenSize.Size = new Size(90, 24);
        txtScreenSize.Text = "320 x 480";

        // chkRandomName
        chkRandomName.AutoSize = true;
        chkRandomName.Checked = true;
        chkRandomName.CheckState = CheckState.Checked;
        chkRandomName.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        chkRandomName.Location = new Point(299, 5);
        chkRandomName.Margin = new Padding(3, 5, 20, 3);
        chkRandomName.Name = "chkRandomName";
        chkRandomName.Size = new Size(135, 21);
        chkRandomName.Text = "Tự tạo tên (dn...)";
        chkRandomName.UseVisualStyleBackColor = true;

        // chkDyTest
        chkDyTest.AutoSize = true;
        chkDyTest.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        chkDyTest.ForeColor = Color.DarkRed;
        chkDyTest.Location = new Point(433, 5);
        chkDyTest.Margin = new Padding(3, 5, 10, 3);
        chkDyTest.Name = "chkDyTest";
        chkDyTest.Size = new Size(68, 21);
        chkDyTest.Text = "dytest";
        chkDyTest.UseVisualStyleBackColor = true;

        // chkAutoTile
        chkAutoTile.AutoSize = true;
        chkAutoTile.Checked = true;
        chkAutoTile.CheckState = CheckState.Checked;
        chkAutoTile.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        chkAutoTile.Location = new Point(510, 5);
        chkAutoTile.Margin = new Padding(3, 5, 12, 3);
        chkAutoTile.Name = "chkAutoTile";
        chkAutoTile.Size = new Size(100, 21);
        chkAutoTile.Text = "Tự xếp tab";
        chkAutoTile.UseVisualStyleBackColor = true;

        // flowOpt (Performance & RAM/CPU Options)
        flowOpt.Dock = DockStyle.Top;
        flowOpt.FlowDirection = FlowDirection.LeftToRight;
        flowOpt.Location = new Point(8, 76);
        flowOpt.Name = "flowOpt";
        flowOpt.Size = new Size(942, 34);
        flowOpt.TabIndex = 2;
        flowOpt.WrapContents = false;

        // chkOptimizeRam
        chkOptimizeRam.AutoSize = true;
        chkOptimizeRam.Checked = true;
        chkOptimizeRam.CheckState = CheckState.Checked;
        chkOptimizeRam.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        chkOptimizeRam.ForeColor = Color.FromArgb(39, 174, 96);
        chkOptimizeRam.Location = new Point(3, 5);
        chkOptimizeRam.Margin = new Padding(3, 5, 16, 3);
        chkOptimizeRam.Name = "chkOptimizeRam";
        chkOptimizeRam.Size = new Size(135, 21);
        chkOptimizeRam.Text = "⚡ Tối ưu RAM/CPU";
        chkOptimizeRam.UseVisualStyleBackColor = true;
        chkOptimizeRam.CheckedChanged += ChkOptimizeRam_CheckedChanged;

        // chkBlackScreen
        chkBlackScreen.AutoSize = true;
        chkBlackScreen.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        chkBlackScreen.ForeColor = Color.FromArgb(41, 128, 185);
        chkBlackScreen.Location = new Point(157, 5);
        chkBlackScreen.Margin = new Padding(3, 5, 20, 3);
        chkBlackScreen.Name = "chkBlackScreen";
        chkBlackScreen.Size = new Size(185, 21);
        chkBlackScreen.Text = "⬛ Màn hình đen (0.4% CPU)";
        chkBlackScreen.UseVisualStyleBackColor = true;
        chkBlackScreen.CheckedChanged += ChkBlackScreen_CheckedChanged;

        // lblFps
        lblFps.AutoSize = true;
        lblFps.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        lblFps.Location = new Point(365, 6);
        lblFps.Margin = new Padding(3, 6, 3, 0);
        lblFps.Name = "lblFps";
        lblFps.Size = new Size(86, 17);
        lblFps.Text = "Giới hạn FPS:";

        // numFps
        numFps.Font = new Font("Segoe UI", 9.5F);
        numFps.Location = new Point(457, 3);
        numFps.Margin = new Padding(3, 3, 20, 3);
        numFps.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
        numFps.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
        numFps.Name = "numFps";
        numFps.Size = new Size(55, 24);
        numFps.Value = new decimal(new int[] { 20, 0, 0, 0 });
        numFps.ValueChanged += NumFps_ValueChanged;

        // btnCleanRam
        btnCleanRam.BackColor = Color.FromArgb(230, 126, 34);
        btnCleanRam.FlatStyle = FlatStyle.Flat;
        btnCleanRam.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCleanRam.ForeColor = Color.White;
        btnCleanRam.Location = new Point(535, 2);
        btnCleanRam.Margin = new Padding(2);
        btnCleanRam.Name = "btnCleanRam";
        btnCleanRam.Size = new Size(135, 26);
        btnCleanRam.Text = "🧹 Giải phóng RAM";
        btnCleanRam.UseVisualStyleBackColor = false;
        btnCleanRam.Click += BtnCleanRam_Click;

        flowOpt.Controls.Add(chkOptimizeRam);
        flowOpt.Controls.Add(chkBlackScreen);
        flowOpt.Controls.Add(lblFps);
        flowOpt.Controls.Add(numFps);
        flowOpt.Controls.Add(btnCleanRam);

        // flowButtons (Action buttons)
        flowButtons.Dock = DockStyle.Bottom;
        flowButtons.FlowDirection = FlowDirection.LeftToRight;
        flowButtons.Location = new Point(8, 46);
        flowButtons.Name = "flowButtons";
        flowButtons.Size = new Size(942, 40);
        flowButtons.TabIndex = 1;
        flowButtons.WrapContents = false;

        // btnAddRow
        btnAddRow.BackColor = Color.FromArgb(41, 128, 185);
        btnAddRow.FlatStyle = FlatStyle.Flat;
        btnAddRow.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddRow.ForeColor = Color.White;
        btnAddRow.Location = new Point(2, 2);
        btnAddRow.Margin = new Padding(2);
        btnAddRow.Name = "btnAddRow";
        btnAddRow.Size = new Size(100, 32);
        btnAddRow.Text = "➕ Thêm dòng";
        btnAddRow.UseVisualStyleBackColor = false;
        btnAddRow.Click += BtnAddRow_Click;

        // btnDelete
        btnDelete.BackColor = Color.FromArgb(192, 57, 43);
        btnDelete.FlatStyle = FlatStyle.Flat;
        btnDelete.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDelete.ForeColor = Color.White;
        btnDelete.Location = new Point(106, 2);
        btnDelete.Margin = new Padding(2);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(80, 32);
        btnDelete.Text = "🗑 Xóa";
        btnDelete.UseVisualStyleBackColor = false;
        btnDelete.Click += BtnDelete_Click;

        // btnStart
        btnStart.BackColor = Color.FromArgb(39, 174, 96);
        btnStart.FlatStyle = FlatStyle.Flat;
        btnStart.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnStart.ForeColor = Color.White;
        btnStart.Location = new Point(190, 2);
        btnStart.Margin = new Padding(2);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(75, 32);
        btnStart.Text = "▶ Bật";
        btnStart.UseVisualStyleBackColor = false;
        btnStart.Click += BtnStart_Click;

        // btnStop
        btnStop.BackColor = Color.FromArgb(127, 140, 141);
        btnStop.FlatStyle = FlatStyle.Flat;
        btnStop.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnStop.ForeColor = Color.White;
        btnStop.Location = new Point(269, 2);
        btnStop.Margin = new Padding(2);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(75, 32);
        btnStop.Text = "⏹ Tắt";
        btnStop.UseVisualStyleBackColor = false;
        btnStop.Click += BtnStop_Click;

        // btnAuto
        btnAuto.BackColor = Color.FromArgb(142, 68, 173);
        btnAuto.FlatStyle = FlatStyle.Flat;
        btnAuto.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAuto.ForeColor = Color.White;
        btnAuto.Location = new Point(348, 2);
        btnAuto.Margin = new Padding(2);
        btnAuto.Name = "btnAuto";
        btnAuto.Size = new Size(85, 32);
        btnAuto.Text = "⚡ Auto";
        btnAuto.UseVisualStyleBackColor = false;
        btnAuto.Click += BtnAuto_Click;

        // btnStopAuto
        btnStopAuto.BackColor = Color.FromArgb(211, 84, 0);
        btnStopAuto.FlatStyle = FlatStyle.Flat;
        btnStopAuto.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnStopAuto.ForeColor = Color.White;
        btnStopAuto.Location = new Point(437, 2);
        btnStopAuto.Margin = new Padding(2);
        btnStopAuto.Name = "btnStopAuto";
        btnStopAuto.Size = new Size(95, 32);
        btnStopAuto.Text = "🛑 Tắt auto";
        btnStopAuto.UseVisualStyleBackColor = false;
        btnStopAuto.Click += BtnStopAuto_Click;

        // btnReset
        btnReset.BackColor = Color.FromArgb(52, 73, 94);
        btnReset.FlatStyle = FlatStyle.Flat;
        btnReset.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnReset.ForeColor = Color.White;
        btnReset.Location = new Point(536, 2);
        btnReset.Margin = new Padding(2);
        btnReset.Name = "btnReset";
        btnReset.Size = new Size(110, 32);
        btnReset.Text = "🔄 Reset bảng";
        btnReset.UseVisualStyleBackColor = false;
        btnReset.Click += BtnReset_Click;

        // btnTileWindows
        btnTileWindows.BackColor = Color.FromArgb(22, 160, 133);
        btnTileWindows.FlatStyle = FlatStyle.Flat;
        btnTileWindows.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnTileWindows.ForeColor = Color.White;
        btnTileWindows.Location = new Point(650, 2);
        btnTileWindows.Margin = new Padding(2);
        btnTileWindows.Name = "btnTileWindows";
        btnTileWindows.Size = new Size(115, 32);
        btnTileWindows.Text = "📐 Sắp xếp tab";
        btnTileWindows.UseVisualStyleBackColor = false;
        btnTileWindows.Click += BtnTileWindows_Click;

        // btnOpenLog
        btnOpenLog.BackColor = Color.FromArgb(44, 62, 80);
        btnOpenLog.FlatStyle = FlatStyle.Flat;
        btnOpenLog.Font = new Font("Segoe UI", 9F);
        btnOpenLog.ForeColor = Color.White;
        btnOpenLog.Location = new Point(769, 2);
        btnOpenLog.Margin = new Padding(2);
        btnOpenLog.Name = "btnOpenLog";
        btnOpenLog.Size = new Size(120, 32);
        btnOpenLog.Text = "📄 File Log dytest";
        btnOpenLog.UseVisualStyleBackColor = false;
        btnOpenLog.Click += BtnOpenLog_Click;

        // Add to flowLayouts
        flowTop.Controls.Add(lblServer);
        flowTop.Controls.Add(numServer);
        flowTop.Controls.Add(lblThreads);
        flowTop.Controls.Add(numThreads);
        flowTop.Controls.Add(lblDelayTab);
        flowTop.Controls.Add(numDelayTab);
        flowTop.Controls.Add(lblScreenSize);
        flowTop.Controls.Add(txtScreenSize);
        flowTop.Controls.Add(chkAutoTile);
        flowTop.Controls.Add(chkRandomName);
        flowTop.Controls.Add(chkDyTest);

        flowButtons.Controls.Add(btnAddRow);
        flowButtons.Controls.Add(btnDelete);
        flowButtons.Controls.Add(btnStart);
        flowButtons.Controls.Add(btnStop);
        flowButtons.Controls.Add(btnAuto);
        flowButtons.Controls.Add(btnStopAuto);
        flowButtons.Controls.Add(btnReset);
        flowButtons.Controls.Add(btnTileWindows);
        flowButtons.Controls.Add(btnOpenLog);

        // dgvAccounts
        dgvAccounts.AllowUserToAddRows = false;
        dgvAccounts.AllowUserToDeleteRows = false;
        dgvAccounts.AllowUserToResizeRows = false;
        dgvAccounts.BackgroundColor = Color.White;
        dgvAccounts.BorderStyle = BorderStyle.None;
        dgvAccounts.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle1.BackColor = Color.FromArgb(236, 240, 241);
        dataGridViewCellStyle1.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        dataGridViewCellStyle1.ForeColor = Color.FromArgb(44, 62, 80);
        dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
        dgvAccounts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
        dgvAccounts.ColumnHeadersHeight = 32;
        dgvAccounts.Columns.AddRange(new DataGridViewColumn[] {
            colSTT, colUserAo, colCharName, colServer, colPlanet, colPower, colGold, colStatus, colPID
        });
        dgvAccounts.Dock = DockStyle.Fill;
        dgvAccounts.EnableHeadersVisualStyles = false;
        dgvAccounts.Font = new Font("Segoe UI", 9.5F);
        dgvAccounts.GridColor = Color.FromArgb(220, 224, 230);
        dgvAccounts.Location = new Point(0, 126);
        dgvAccounts.MultiSelect = true; // Cho phép giữ Shift/Ctrl kéo chuột chọn nhiều dòng!
        dgvAccounts.Name = "dgvAccounts";
        dgvAccounts.ReadOnly = true;
        dgvAccounts.RowHeadersVisible = false;
        dgvAccounts.RowTemplate.Height = 28;
        dgvAccounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvAccounts.Size = new Size(960, 450);
        dgvAccounts.TabIndex = 1;

        // colSTT
        colSTT.DataPropertyName = "STT";
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
        colSTT.DefaultCellStyle = dataGridViewCellStyle2;
        colSTT.HeaderText = "STT";
        colSTT.Name = "colSTT";
        colSTT.ReadOnly = true;
        colSTT.Width = 55;

        // colUserAo
        colUserAo.DataPropertyName = "UserAo";
        colUserAo.HeaderText = "Tài khoản ảo";
        colUserAo.Name = "colUserAo";
        colUserAo.ReadOnly = true;
        colUserAo.Width = 130;

        // colCharName
        colCharName.DataPropertyName = "CharacterName";
        colCharName.HeaderText = "Tên nhân vật";
        colCharName.Name = "colCharName";
        colCharName.ReadOnly = true;
        colCharName.Width = 140;

        // colServer
        colServer.DataPropertyName = "Server";
        dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
        colServer.DefaultCellStyle = dataGridViewCellStyle3;
        colServer.HeaderText = "Server";
        colServer.Name = "colServer";
        colServer.ReadOnly = true;
        colServer.Width = 70;

        // colPlanet
        colPlanet.DataPropertyName = "Planet";
        dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
        colPlanet.DefaultCellStyle = dataGridViewCellStyle4;
        colPlanet.HeaderText = "Hành tinh";
        colPlanet.Name = "colPlanet";
        colPlanet.ReadOnly = true;
        colPlanet.Width = 85;

        // colPower
        colPower.DataPropertyName = "Power";
        dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleRight;
        colPower.DefaultCellStyle = dataGridViewCellStyle5;
        colPower.HeaderText = "Sức mạnh";
        colPower.Name = "colPower";
        colPower.ReadOnly = true;
        colPower.Width = 110;

        // colGold
        colGold.DataPropertyName = "Gold";
        dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleRight;
        colGold.DefaultCellStyle = dataGridViewCellStyle6;
        colGold.HeaderText = "Vàng";
        colGold.Name = "colGold";
        colGold.ReadOnly = true;
        colGold.Width = 110;

        // colStatus
        colStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        colStatus.DataPropertyName = "Status";
        colStatus.HeaderText = "Trạng thái";
        colStatus.Name = "colStatus";
        colStatus.ReadOnly = true;
        colStatus.MinimumWidth = 200;

        // colPID
        colPID.DataPropertyName = "PID";
        colPID.DefaultCellStyle = dataGridViewCellStyle2;
        colPID.HeaderText = "PID";
        colPID.Name = "colPID";
        colPID.ReadOnly = true;
        colPID.Width = 80;

        // statusStrip
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
        statusStrip.Location = new Point(0, 576);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(960, 22);
        statusStrip.TabIndex = 2;

        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(135, 17);
        lblStatus.Text = "Sẵn sàng | Tổng dòng: 0";

        // MainForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1020, 600);
        Controls.Add(dgvAccounts);
        Controls.Add(statusStrip);
        Controls.Add(pnlTop);
        MinimumSize = new Size(880, 450);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "QLTK - Quản Lý Tài Khoản & Auto Game Dragon Boy";
        FormClosing += MainForm_FormClosing;
        Load += MainForm_Load;

        pnlTop.ResumeLayout(false);
        flowTop.ResumeLayout(false);
        flowTop.PerformLayout();
        flowOpt.ResumeLayout(false);
        flowOpt.PerformLayout();
        flowButtons.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)numServer).EndInit();
        ((System.ComponentModel.ISupportInitialize)numThreads).EndInit();
        ((System.ComponentModel.ISupportInitialize)numDelayTab).EndInit();
        ((System.ComponentModel.ISupportInitialize)numFps).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvAccounts).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel pnlTop;
    private FlowLayoutPanel flowGamePath;
    private Label lblGamePath;
    private TextBox txtGamePath;
    private Button btnBrowseGame;
    private FlowLayoutPanel flowTop;
    private Label lblServer;
    private NumericUpDown numServer;
    private Label lblThreads;
    private NumericUpDown numThreads;
    private Label lblDelayTab;
    private NumericUpDown numDelayTab;
    private Label lblScreenSize;
    private TextBox txtScreenSize;
    private CheckBox chkAutoTile;
    private CheckBox chkRandomName;
    private CheckBox chkDyTest;

    private FlowLayoutPanel flowOpt;
    private CheckBox chkOptimizeRam;
    private CheckBox chkBlackScreen;
    private Label lblFps;
    private NumericUpDown numFps;
    private Button btnCleanRam;

    private FlowLayoutPanel flowButtons;
    private Button btnAddRow;
    private Button btnDelete;
    private Button btnStart;
    private Button btnStop;
    private Button btnAuto;
    private Button btnStopAuto;
    private Button btnReset;
    private Button btnTileWindows;
    private Button btnOpenLog;

    private DataGridView dgvAccounts;
    private DataGridViewTextBoxColumn colSTT;
    private DataGridViewTextBoxColumn colUserAo;
    private DataGridViewTextBoxColumn colCharName;
    private DataGridViewTextBoxColumn colServer;
    private DataGridViewTextBoxColumn colPlanet;
    private DataGridViewTextBoxColumn colPower;
    private DataGridViewTextBoxColumn colGold;
    private DataGridViewTextBoxColumn colStatus;
    private DataGridViewTextBoxColumn colPID;

    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
}
