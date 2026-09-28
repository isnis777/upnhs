using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;

namespace QLTK;

public partial class MainForm : Form
{
    private readonly BindingList<AccountRow> _accountList = new();
    private InternalTcpServer? _tcpServer;
    private System.Windows.Forms.Timer? _autoTimer;
    private bool _isAutoRunning;
    private string _gameExePath = @"D:\DragonBoy250_pc\DragonBoy250_pc\DragonBoy250.exe";
    private readonly string _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "game_path.txt");
    private readonly string _accountsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "accounts.json");
    private readonly string _settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_settings.json");
    private readonly object _saveLock = new();

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        // Cấu hình DataGridView data binding
        dgvAccounts.AutoGenerateColumns = false;
        dgvAccounts.DataSource = _accountList;

        // Phục hồi dữ liệu tài khoản và cấu hình đã lưu
        LoadAccountsFromFile();

        // Khởi động TCP Server
        StartTcpServer();

        // Khởi tạo timer điều phối Auto & giám sát tab
        _autoTimer = new System.Windows.Forms.Timer();
        _autoTimer.Interval = 1000;
        _autoTimer.Tick += AutoTimer_Tick;
        _autoTimer.Start();

        // Kiểm tra đường dẫn game
        EnsureGamePath();

        UpdateStatusText();
    }

    private void EnsureGamePath()
    {
        try
        {
            if (File.Exists(_configFilePath))
            {
                string saved = File.ReadAllText(_configFilePath).Trim();
                if (File.Exists(saved))
                {
                    _gameExePath = saved;
                    txtGamePath.Text = _gameExePath;
                    return;
                }
            }
        }
        catch { }

        string dPath = @"D:\DragonBoy250_pc\DragonBoy250_pc\DragonBoy250.exe";
        if (File.Exists(dPath))
        {
            _gameExePath = dPath;
        }
        else if (!File.Exists(_gameExePath))
        {
            string cPath = @"C:\Users\laptopcuady\Downloads\DragonBoy250_pc\DragonBoy250_pc\DragonBoy250.exe";
            if (File.Exists(cPath))
            {
                _gameExePath = cPath;
            }
            else
            {
                string fallback = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DragonBoy250.exe");
                if (File.Exists(fallback))
                {
                    _gameExePath = fallback;
                }
            }
        }

        txtGamePath.Text = _gameExePath;
    }

    private void BtnBrowseGame_Click(object? sender, EventArgs e)
    {
        using OpenFileDialog ofd = new();
        ofd.Title = "Chọn file thực thi game Dragon Boy (.exe)";
        ofd.Filter = "DragonBoy250.exe (*.exe)|*.exe|Tất cả file (*.*)|*.*";

        string initDir = @"D:\DragonBoy250_pc\DragonBoy250_pc";
        if (Directory.Exists(initDir))
        {
            ofd.InitialDirectory = initDir;
        }
        else if (!string.IsNullOrEmpty(txtGamePath.Text))
        {
            string? dir = Path.GetDirectoryName(txtGamePath.Text);
            if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
            {
                ofd.InitialDirectory = dir;
            }
        }

        if (ofd.ShowDialog() == DialogResult.OK)
        {
            _gameExePath = ofd.FileName;
            txtGamePath.Text = _gameExePath;
            try
            {
                File.WriteAllText(_configFilePath, _gameExePath);
            }
            catch { }
        }
    }

    private void StartTcpServer()
    {
        try
        {
            _tcpServer = new InternalTcpServer(9999);
            _tcpServer.OnDataReceived += TcpServer_OnDataReceived;
            _tcpServer.OnConfigRequest += TcpServer_OnConfigRequest;
            _tcpServer.OnClientDisconnected += TcpServer_OnClientDisconnected;
            _tcpServer.OnClientStuckTimeout += TcpServer_OnClientStuckTimeout;
            _tcpServer.Start();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể khởi động TCP Server: {ex.Message}", "Lỗi TCP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private (int server, bool randomName, bool dytest, string screenSize, string userAo, int fps, bool optimizeRam, bool blackScreen) TcpServer_OnConfigRequest(string clientId)
    {
        int server = 1;
        bool randomName = false;
        bool dytest = false;
        string screenSize = "320 x 480";
        string userAo = string.Empty;
        int fps = 20;
        bool optimizeRam = true;
        bool blackScreen = false;

        try
        {
            if (InvokeRequired)
            {
                Invoke(() =>
                {
                    var row = _accountList.FirstOrDefault(r => r.ClientId == clientId);
                    server = row != null ? row.Server : (int)numServer.Value;
                    userAo = row != null ? row.UserAo : string.Empty;
                    randomName = chkRandomName.Checked;
                    dytest = chkDyTest.Checked;
                    screenSize = txtScreenSize.Text.Trim();
                    fps = (int)numFps.Value;
                    optimizeRam = chkOptimizeRam.Checked;
                    blackScreen = chkBlackScreen.Checked;
                });
            }
            else
            {
                var row = _accountList.FirstOrDefault(r => r.ClientId == clientId);
                server = row != null ? row.Server : (int)numServer.Value;
                userAo = row != null ? row.UserAo : string.Empty;
                randomName = chkRandomName.Checked;
                dytest = chkDyTest.Checked;
                screenSize = txtScreenSize.Text.Trim();
                fps = (int)numFps.Value;
                optimizeRam = chkOptimizeRam.Checked;
                blackScreen = chkBlackScreen.Checked;
            }
        }
        catch
        {
            randomName = chkRandomName.Checked;
            dytest = chkDyTest.Checked;
            screenSize = txtScreenSize.Text.Trim();
            fps = (int)numFps.Value;
            optimizeRam = chkOptimizeRam.Checked;
            blackScreen = chkBlackScreen.Checked;
        }

        return (server, randomName, dytest, screenSize, userAo, fps, optimizeRam, blackScreen);
    }

    public static bool TryParseScreenSize(string input, out int height, out int width)
    {
        height = 0;
        width = 0;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var parts = input.Split(new[] { 'x', 'X', '*', ',', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
        {
            if (int.TryParse(parts[0].Trim(), out int h) && int.TryParse(parts[1].Trim(), out int w))
            {
                if (h > 0 && w > 0)
                {
                    height = h;
                    width = w;
                    return true;
                }
            }
        }
        return false;
    }

    private void TcpServer_OnDataReceived(string clientId, string cName, int server, string planet, string power, string gold, string status, string pid, string userAo, string tiemNang)
    {
        if (IsDisposed || !IsHandleCreated) return;

        BeginInvoke(() =>
        {
            var row = _accountList.FirstOrDefault(r => r.ClientId == clientId);
            if (row == null && !string.IsNullOrEmpty(pid))
            {
                row = _accountList.FirstOrDefault(r => r.PID == pid);
            }

            if (row != null)
            {
                bool changed = false;
                if (!string.IsNullOrEmpty(userAo) && row.UserAo != userAo) { row.UserAo = userAo; changed = true; }
                if (!string.IsNullOrEmpty(cName) && row.CharacterName != cName) 
                { 
                    row.CharacterName = cName; 
                    changed = true; 
                    row.LastProgressTime = DateTime.Now;
                }
                if (server > 0 && row.Server != server) { row.Server = server; changed = true; }
                if (!string.IsNullOrEmpty(planet) && row.Planet != planet) { row.Planet = planet; changed = true; }

                long.TryParse(power, out long pVal);
                long.TryParse(tiemNang, out long tnVal);

                if (pVal > row.LastPowerVal || tnVal > row.LastTiemNangVal)
                {
                    if (pVal > row.LastPowerVal) row.LastPowerVal = pVal;
                    if (tnVal > row.LastTiemNangVal) row.LastTiemNangVal = tnVal;
                    row.LastProgressTime = DateTime.Now;
                }

                string fPower = FormatNumber(power);
                if (!string.IsNullOrEmpty(power) && row.Power != fPower) { row.Power = fPower; changed = true; }
                string fGold = FormatNumber(gold);
                if (!string.IsNullOrEmpty(gold) && row.Gold != fGold) { row.Gold = fGold; changed = true; }
                if (!string.IsNullOrEmpty(status) && row.Status != status) 
                { 
                    row.Status = status; 
                    changed = true; 
                    if (status != "đang đánh quái")
                    {
                        row.LastProgressTime = DateTime.Now;
                    }
                }
                if (!string.IsNullOrEmpty(pid))
                {
                    row.PID = pid;
                    if (int.TryParse(pid, out int pInt) && pInt > 0)
                    {
                        string title = string.IsNullOrEmpty(cName) ? $"STT {row.STT} - SV {row.Server}" : $"STT {row.STT} - {cName}";
                        SetProcessWindowTitle(pInt, title);
                    }
                }

                dgvAccounts.Refresh();
                UpdateStatusText();

                if (changed)
                {
                    SaveAccountsToFile();
                }
            }
        });
    }

    private void TcpServer_OnClientStuckTimeout(string clientId, long power, long tiemNang)
    {
        if (IsDisposed || !IsHandleCreated) return;

        BeginInvoke(() =>
        {
            var row = _accountList.FirstOrDefault(r => r.ClientId == clientId);
            if (row != null && row.Status != "hoàn thành" && !row.IsStoppedManually)
            {
                try
                {
                    if (row.ActiveProcess != null && !row.ActiveProcess.HasExited)
                    {
                        row.ActiveProcess.Kill();
                    }
                }
                catch { }

                row.ActiveProcess = null;
                row.PID = string.Empty;
                row.IsWaitingReopen = true;
                row.ReopenAt = DateTime.Now.AddSeconds(30);
                row.Status = "Chờ mở lại (30s)";
                dgvAccounts.Refresh();
                UpdateStatusText();
            }
        });
    }

    private void TcpServer_OnClientDisconnected(string clientId)
    {
        if (IsDisposed || !IsHandleCreated) return;

        BeginInvoke(() =>
        {
            var row = _accountList.FirstOrDefault(r => r.ClientId == clientId);
            if (row != null && row.ActiveProcess != null && row.ActiveProcess.HasExited)
            {
                row.ActiveProcess = null;
                row.PID = string.Empty;
                if (row.Status != "hoàn thành" && !row.IsStoppedManually)
                {
                    if (!row.IsWaitingReopen)
                    {
                        row.IsWaitingReopen = true;
                        row.ReopenAt = DateTime.Now.AddSeconds(30);
                        row.Status = "Chờ mở lại (30s)";
                    }
                }
                else if (row.IsStoppedManually)
                {
                    row.Status = "Đã tắt";
                }
                dgvAccounts.Refresh();
                UpdateStatusText();
                SaveAccountsToFile();
            }
        });
    }

    #region Nút Thao Tác

    // 1. Thêm dòng: Dùng ô "Số luồng" để biết thêm bao nhiêu dòng, gán Server từ ô "Server"
    private void BtnAddRow_Click(object? sender, EventArgs e)
    {
        int countToAdd = (int)numThreads.Value;
        int currentServer = (int)numServer.Value;

        for (int i = 0; i < countToAdd; i++)
        {
            int nextSTT = _accountList.Count + 1;
            string clientId = "tab_" + nextSTT;
            var newRow = new AccountRow(nextSTT, currentServer, clientId);
            _accountList.Add(newRow);
        }

        UpdateStatusText();
        SaveAccountsToFile();
    }

    // 1.1 Thêm tài khoản thật (Hỗ trợ cả dán danh sách hàng loạt và thêm từng nick lẻ)
    private void BtnAddReal_Click(object? sender, EventArgs e)
    {
        int defaultServer = (int)numServer.Value;
        using var form = new AddAccountForm(defaultServer);
        if (form.ShowDialog(this) == DialogResult.OK || form.AddedAccounts.Count > 0)
        {
            foreach (var (user, pass, sv) in form.AddedAccounts)
            {
                int nextSTT = _accountList.Count + 1;
                string clientId = "tab_" + nextSTT;
                var newRow = new AccountRow(nextSTT, user, pass, sv, clientId);
                _accountList.Add(newRow);
            }

            UpdateStatusText();
            SaveAccountsToFile();
            dgvAccounts.Refresh();
        }
    }

    // 2. Xóa: Xóa toàn bộ dòng đang chọn (hỗ trợ giữ Shift + kéo chuột chọn nhiều dòng)
    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (dgvAccounts.SelectedRows.Count == 0)
        {
            MessageBox.Show("Vui lòng chọn ít nhất 1 dòng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedRows = dgvAccounts.SelectedRows.Cast<DataGridViewRow>()
            .Select(r => r.DataBoundItem as AccountRow)
            .Where(r => r != null)
            .Cast<AccountRow>()
            .ToList();

        foreach (var row in selectedRows)
        {
            try
            {
                if (row.ActiveProcess != null && !row.ActiveProcess.HasExited)
                {
                    row.ActiveProcess.Kill();
                }
            }
            catch { }

            _accountList.Remove(row);
        }

        // Đánh lại STT tăng dần
        for (int i = 0; i < _accountList.Count; i++)
        {
            _accountList[i].STT = i + 1;
        }

        dgvAccounts.Refresh();
        UpdateStatusText();
        SaveAccountsToFile();
    }

    // 3. Bật: Chạy dòng đang chọn (chỉ chạy 1 dòng đó)
    private void BtnStart_Click(object? sender, EventArgs e)
    {
        if (dgvAccounts.CurrentRow?.DataBoundItem is not AccountRow row)
        {
            MessageBox.Show("Vui lòng chọn dòng cần bật!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (row.ActiveProcess != null && !row.ActiveProcess.HasExited)
        {
            MessageBox.Show($"Dòng STT {row.STT} đang chạy với PID: {row.PID}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        row.IsStoppedManually = false;
        LaunchTab(row);
    }

    // 4. Tắt: Tắt dòng đang chọn không cho chạy lại nữa
    private void BtnStop_Click(object? sender, EventArgs e)
    {
        if (dgvAccounts.CurrentRow?.DataBoundItem is not AccountRow row)
        {
            MessageBox.Show("Vui lòng chọn dòng cần tắt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        row.IsStoppedManually = true;
        row.IsWaitingReopen = false;
        row.ReopenAt = DateTime.MinValue;
        StopTab(row);
        row.Status = "Đã tắt";
        dgvAccounts.Refresh();
        UpdateStatusText();
        SaveAccountsToFile();
    }

    // 5. Auto: Mở tab song song theo số luồng
    private void BtnAuto_Click(object? sender, EventArgs e)
    {
        if (_isAutoRunning)
        {
            MessageBox.Show("Chế độ Auto đang chạy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _isAutoRunning = true;
        btnAuto.Enabled = false;
        btnStopAuto.Enabled = true;

        TriggerAutoCycle();
        UpdateStatusText();
    }

    // 6. Tắt auto: Tắt toàn bộ các luồng đang chạy song song lại và không được mở lại nữa
    private void BtnStopAuto_Click(object? sender, EventArgs e)
    {
        _isAutoRunning = false;

        btnAuto.Enabled = true;
        btnStopAuto.Enabled = true;

        foreach (var row in _accountList)
        {
            row.IsWaitingReopen = false;
            row.ReopenAt = DateTime.MinValue;
            if (row.ActiveProcess != null && !row.ActiveProcess.HasExited)
            {
                StopTab(row);
                if (row.Status != "hoàn thành")
                {
                    row.Status = "Đã dừng";
                }
            }
            else if (row.Status.StartsWith("Chờ mở lại"))
            {
                row.Status = "Đã dừng";
            }
        }

        dgvAccounts.Refresh();
        UpdateStatusText();
        SaveAccountsToFile();
    }

    // 7. Reset bảng: Trả nội dung cột trạng thái thành rỗng như khi chưa có dữ liệu để chạy lại từ đầu
    private void BtnReset_Click(object? sender, EventArgs e)
    {
        foreach (var row in _accountList)
        {
            row.Status = string.Empty;
            row.IsStoppedManually = false;
            row.IsWaitingReopen = false;
            row.ReopenAt = DateTime.MinValue;
            row.LastProgressTime = DateTime.MinValue;
            row.LastPowerVal = -1;
            row.LastTiemNangVal = -1;
        }

        dgvAccounts.Refresh();
        UpdateStatusText();
        SaveAccountsToFile();
    }

    // Mở file log dytest
    private void BtnOpenLog_Click(object? sender, EventArgs e)
    {
        string logPath = @"g:\My Drive\dycopy\game_action_log.txt";
        if (File.Exists(logPath))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = logPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở file log: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        else
        {
            MessageBox.Show($"Chưa có file log tại: {logPath}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // Sắp xếp các tab đang chạy thành lưới trên màn hình
    private void BtnTileWindows_Click(object? sender, EventArgs e)
    {
        ArrangeTabs();
    }

    #endregion

    #region Quản Lý Tiến Trình & Auto

    private void LaunchTab(AccountRow row)
    {
        _gameExePath = txtGamePath.Text.Trim();

        if (!File.Exists(_gameExePath))
        {
            BtnBrowseGame_Click(null, EventArgs.Empty);
            _gameExePath = txtGamePath.Text.Trim();
            if (!File.Exists(_gameExePath))
            {
                MessageBox.Show("Vui lòng chọn đúng file thực thi game DragonBoy250.exe!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        try
        {
            bool isDyTest = chkDyTest.Checked;
            bool isRandomName = chkRandomName.Checked;
            string screenSize = txtScreenSize.Text.Trim();

            string sizeArgs = "";
            if (TryParseScreenSize(screenSize, out int h, out int w))
            {
                sizeArgs = $"-size {h}x{w} -screen-width {w} -screen-height {h} -screen-fullscreen 0";
            }

            string userAoArgs = "";
            if (row.IsRealAccount)
            {
                userAoArgs = $"-user {row.Username.Trim()} -pass \"{row.Password.Trim()}\" -autonhs 0";
            }
            else
            {
                userAoArgs = "-autonhs 1";
                if (!string.IsNullOrWhiteSpace(row.UserAo))
                {
                    userAoArgs += $" -userao {row.UserAo.Trim()}";
                }
            }

            string optArgs = $"-fps {(int)numFps.Value} -lowram {(chkOptimizeRam.Checked ? 1 : 0)} -blackscreen {(chkBlackScreen.Checked ? 1 : 0)}";

            // Tham số dòng lệnh: clientId stt server dytest randomname size userao fps lowram blackscreen
            string args = $"-id {row.ClientId} -stt {row.STT} -server {row.Server} -dytest {(isDyTest ? 1 : 0)} -randomname {(isRandomName ? 1 : 0)} {sizeArgs} {userAoArgs} {optArgs}".Trim();

            ProcessStartInfo psi = new()
            {
                FileName = _gameExePath,
                Arguments = args,
                WorkingDirectory = Path.GetDirectoryName(_gameExePath),
                UseShellExecute = false
            };

            Process? p = Process.Start(psi);
            if (p != null)
            {
                int currentPid = p.Id;
                int currentStt = row.STT;
                int currentSv = row.Server;
                _ = Task.Run(async () =>
                {
                    for (int i = 0; i < 20; i++)
                    {
                        await Task.Delay(500);
                        if (SetProcessWindowTitle(currentPid, $"STT {currentStt} - SV {currentSv}"))
                        {
                            if (chkAutoTile.Checked && !IsDisposed && IsHandleCreated)
                            {
                                BeginInvoke(() => ArrangeTabs());
                            }
                            break;
                        }
                    }
                });

                p.EnableRaisingEvents = true;
                p.Exited += (s, ev) =>
                {
                    if (IsDisposed || !IsHandleCreated) return;
                    BeginInvoke(() =>
                    {
                        row.ActiveProcess = null;
                        row.PID = string.Empty;
                        if (row.Status != "hoàn thành" && !row.IsStoppedManually)
                        {
                            if (!row.IsWaitingReopen)
                            {
                                row.IsWaitingReopen = true;
                                row.ReopenAt = DateTime.Now.AddSeconds(30);
                                row.Status = "Chờ mở lại (30s)";
                            }
                        }
                        else if (row.IsStoppedManually)
                        {
                            row.Status = "Đã tắt";
                        }
                        dgvAccounts.Refresh();
                        UpdateStatusText();
                    });
                };

                row.ActiveProcess = p;
                row.PID = p.Id.ToString();
                row.Status = "đang mở tab";
                row.IsStoppedManually = false;
                row.IsWaitingReopen = false;
                row.ReopenAt = DateTime.MinValue;
                row.LastProgressTime = DateTime.Now;
                row.LastPowerVal = -1;
                row.LastTiemNangVal = -1;
                dgvAccounts.Refresh();
                UpdateStatusText();
            }
        }
        catch (Exception ex)
        {
            row.Status = $"Lỗi mở: {ex.Message}";
            dgvAccounts.Refresh();
        }
    }

    private void StopTab(AccountRow row)
    {
        try
        {
            if (row.ActiveProcess != null && !row.ActiveProcess.HasExited)
            {
                row.ActiveProcess.Kill();
            }
        }
        catch { }
        finally
        {
            row.ActiveProcess = null;
            row.PID = string.Empty;
        }
    }

    private int _tickCount = 0;

    private void AutoTimer_Tick(object? sender, EventArgs e)
    {
        DateTime now = DateTime.Now;

        _tickCount++;
        if (_tickCount % 30 == 0 && chkOptimizeRam.Checked)
        {
            CleanCurrentProcessMemory();
        }

        // 1. Kiểm tra tab đang chạy bị treo quá 60s không tăng SM/TN hoặc mất phản hồi (chỉ áp dụng cho tài khoản ảo đang làm nhiệm vụ)
        foreach (var row in _accountList)
        {
            if (row.ActiveProcess != null && !row.ActiveProcess.HasExited && !row.IsStoppedManually && row.Status != "hoàn thành" && !row.IsRealAccount)
            {
                if (row.LastProgressTime != DateTime.MinValue && (now - row.LastProgressTime).TotalSeconds >= 60)
                {
                    try
                    {
                        row.ActiveProcess.Kill();
                    }
                    catch { }

                    row.ActiveProcess = null;
                    row.PID = string.Empty;
                    row.IsWaitingReopen = true;
                    row.ReopenAt = now.AddSeconds(30);
                    row.Status = "Chờ mở lại (30s)";
                }
            }
        }

        // 2. Xử lý đếm ngược 30s mở lại
        bool refreshNeeded = false;
        foreach (var row in _accountList)
        {
            if (row.IsWaitingReopen && (row.ActiveProcess == null || row.ActiveProcess.HasExited))
            {
                if (row.IsStoppedManually || row.Status == "hoàn thành")
                {
                    row.IsWaitingReopen = false;
                    refreshNeeded = true;
                    continue;
                }

                int remain = (int)Math.Ceiling((row.ReopenAt - now).TotalSeconds);
                if (remain > 0)
                {
                    string newStatus = $"Chờ mở lại ({remain}s)";
                    if (row.Status != newStatus)
                    {
                        row.Status = newStatus;
                        refreshNeeded = true;
                    }
                }
                else
                {
                    row.IsWaitingReopen = false;
                    row.Status = "Chờ chạy";
                    row.LastProgressTime = DateTime.MinValue;
                    refreshNeeded = true;

                    if (!_isAutoRunning && !row.IsStoppedManually)
                    {
                        LaunchTab(row);
                    }
                }
            }
        }

        if (refreshNeeded)
        {
            dgvAccounts.Refresh();
            UpdateStatusText();
        }

        if (_isAutoRunning)
        {
            TriggerAutoCycle();
        }
    }

    private bool _isLaunchingBatch = false;

    private async void TriggerAutoCycle()
    {
        if (!_isAutoRunning || _isLaunchingBatch) return;

        int maxThreads = (int)numThreads.Value;

        // Đếm số tab đang hoạt động
        int runningCount = _accountList.Count(r => r.ActiveProcess != null && !r.ActiveProcess.HasExited);

        if (runningCount >= maxThreads) return;

        int availableSlots = maxThreads - runningCount;

        // Tìm các dòng cần chạy: chưa hoàn thành, chưa dừng thủ công, không trong lúc chờ mở lại, và chưa có process đang chạy
        var candidates = _accountList.Where(r =>
            (r.ActiveProcess == null || r.ActiveProcess.HasExited) &&
            !r.IsStoppedManually &&
            !r.IsWaitingReopen &&
            r.Status != "hoàn thành" &&
            (string.IsNullOrEmpty(r.Status) || r.Status == "Chờ chạy" || r.Status == "Đã dừng")
        ).Take(availableSlots).ToList();

        if (candidates.Count == 0) return;

        _isLaunchingBatch = true;
        try
        {
            int delayMs = Math.Max(500, (int)numDelayTab.Value * 1000);
            for (int i = 0; i < candidates.Count; i++)
            {
                if (!_isAutoRunning) break;
                var row = candidates[i];
                LaunchTab(row);

                // Delay giữa các lần mở tab để tránh nghẽn DirectX/socket và lỗi crash
                if (i < candidates.Count - 1 && delayMs > 0)
                {
                    await Task.Delay(delayMs);
                }
            }
        }
        finally
        {
            _isLaunchingBatch = false;
        }
    }

    private void UpdateStatusText()
    {
        int totalRows = _accountList.Count;
        int running = _accountList.Count(r => r.ActiveProcess != null && !r.ActiveProcess.HasExited);
        int completed = _accountList.Count(r => r.Status == "hoàn thành");

        string autoState = _isAutoRunning ? "⚡ Auto: ĐANG CHẠY" : "⏹ Auto: ĐÃ TẮT";
        lblStatus.Text = $"{autoState} | Đang chạy: {running}/{(int)numThreads.Value} | Tổng dòng: {totalRows} | Hoàn thành: {completed}";
    }

    private static string FormatNumber(string input)
    {
        if (long.TryParse(input, out long val))
        {
            return val.ToString("N0");
        }
        return input;
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _autoTimer?.Stop();
        _isAutoRunning = false;

        // Tắt các tab đang mở khi đóng QLTK
        foreach (var row in _accountList)
        {
            StopTab(row);
        }

        // Lưu dữ liệu tài khoản và cấu hình trước khi thoát
        SaveAccountsToFile();

        _tcpServer?.Stop();
    }

    public void SaveAccountsToFile()
    {
        try
        {
            lock (_saveLock)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                List<AccountRow> snapshot = _accountList.ToList();
                string json = JsonSerializer.Serialize(snapshot, options);
                File.WriteAllText(_accountsFilePath, json);

                var settings = new AppSettings
                {
                    Threads = (int)numThreads.Value,
                    Server = (int)numServer.Value,
                    RandomName = chkRandomName.Checked,
                    DyTest = chkDyTest.Checked,
                    ScreenSize = txtScreenSize.Text.Trim(),
                    OpenTabDelaySeconds = (int)numDelayTab.Value,
                    AutoTileWindows = chkAutoTile.Checked,
                    OptimizeRam = chkOptimizeRam.Checked,
                    BlackScreen = chkBlackScreen.Checked,
                    TargetFps = (int)numFps.Value
                };
                string settingsJson = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(_settingsFilePath, settingsJson);
            }
        }
        catch
        {
        }
    }

    public void LoadAccountsFromFile()
    {
        try
        {
            // 1. Phục hồi cấu hình giao diện nếu có
            string sPath = _settingsFilePath;
            if (!File.Exists(sPath) && File.Exists(Path.Combine(Environment.CurrentDirectory, "app_settings.json")))
            {
                sPath = Path.Combine(Environment.CurrentDirectory, "app_settings.json");
            }

            if (File.Exists(sPath))
            {
                string sJson = File.ReadAllText(sPath);
                if (!string.IsNullOrWhiteSpace(sJson))
                {
                    var settings = JsonSerializer.Deserialize<AppSettings>(sJson);
                    if (settings != null)
                    {
                        if (settings.Threads >= numThreads.Minimum && settings.Threads <= numThreads.Maximum)
                            numThreads.Value = settings.Threads;
                        if (settings.Server >= numServer.Minimum && settings.Server <= numServer.Maximum)
                            numServer.Value = settings.Server;
                        chkRandomName.Checked = settings.RandomName;
                        chkDyTest.Checked = settings.DyTest;
                        if (!string.IsNullOrWhiteSpace(settings.ScreenSize))
                            txtScreenSize.Text = settings.ScreenSize;
                        if (settings.OpenTabDelaySeconds >= numDelayTab.Minimum && settings.OpenTabDelaySeconds <= numDelayTab.Maximum)
                            numDelayTab.Value = settings.OpenTabDelaySeconds;
                        chkAutoTile.Checked = settings.AutoTileWindows;
                        chkOptimizeRam.Checked = settings.OptimizeRam;
                        chkBlackScreen.Checked = settings.BlackScreen;
                        if (settings.TargetFps >= numFps.Minimum && settings.TargetFps <= numFps.Maximum)
                            numFps.Value = settings.TargetFps;
                    }
                }
            }

            // 2. Phục hồi danh sách tài khoản
            string accPath = _accountsFilePath;
            if (!File.Exists(accPath) && File.Exists(Path.Combine(Environment.CurrentDirectory, "accounts.json")))
            {
                accPath = Path.Combine(Environment.CurrentDirectory, "accounts.json");
            }

            if (!File.Exists(accPath)) return;
            string json = File.ReadAllText(accPath);
            if (string.IsNullOrWhiteSpace(json)) return;

            var loaded = JsonSerializer.Deserialize<List<AccountRow>>(json);
            if (loaded != null && loaded.Count > 0)
            {
                _accountList.Clear();
                int stt = 1;
                foreach (var row in loaded)
                {
                    row.STT = stt++;
                    row.PID = string.Empty;
                    row.ActiveProcess = null;
                    if (string.IsNullOrEmpty(row.ClientId))
                    {
                        row.ClientId = "tab_" + row.STT;
                    }
                    if (row.Status.StartsWith("đang", StringComparison.OrdinalIgnoreCase))
                    {
                        row.Status = "Đã dừng";
                    }
                    _accountList.Add(row);
                }

                dgvAccounts.Refresh();
                UpdateStatusText();
            }
        }
        catch
        {
        }
    }

    #endregion

    #region Performance Optimization & Event Handlers
    private void ChkOptimizeRam_CheckedChanged(object? sender, EventArgs e)
    {
        SaveAccountsToFile();
    }

    private void ChkBlackScreen_CheckedChanged(object? sender, EventArgs e)
    {
        try
        {
            _tcpServer?.Broadcast($"CMD|SET_BLACKSCREEN|{(chkBlackScreen.Checked ? 1 : 0)}");
            SaveAccountsToFile();
        }
        catch { }
    }

    private void NumFps_ValueChanged(object? sender, EventArgs e)
    {
        try
        {
            _tcpServer?.Broadcast($"CMD|SET_FPS|{(int)numFps.Value}");
            SaveAccountsToFile();
        }
        catch { }
    }

    private void BtnCleanRam_Click(object? sender, EventArgs e)
    {
        try
        {
            _tcpServer?.Broadcast("CMD|CLEAN_RAM");
            CleanCurrentProcessMemory();
            lblStatus.Text = "Đã gửi lệnh giải phóng RAM tới tất cả các tab game và QLTK!";
        }
        catch (Exception ex)
        {
            lblStatus.Text = $"Lỗi giải phóng RAM: {ex.Message}";
        }
    }
    #endregion

    #region Win32 Window Title & Positioning Management
    [System.Runtime.InteropServices.DllImport("psapi.dll")]
    private static extern int EmptyWorkingSet(IntPtr hwProc);

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

    public static void CleanCurrentProcessMemory()
    {
        try
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            IntPtr handle = Process.GetCurrentProcess().Handle;
            EmptyWorkingSet(handle);
            SetProcessWorkingSetSize(handle, -1, -1);
        }
        catch { }
    }
    [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowTextW", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern bool SetWindowText(IntPtr hWnd, string lpString);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_SHOWWINDOW = 0x0040;

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public static IntPtr FindWindowForProcess(int pid)
    {
        if (pid <= 0) return IntPtr.Zero;
        IntPtr found = IntPtr.Zero;
        try
        {
            EnumWindows((hWnd, lParam) =>
            {
                GetWindowThreadProcessId(hWnd, out uint wPid);
                if (wPid == (uint)pid && IsWindowVisible(hWnd))
                {
                    found = hWnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
        }
        catch { }
        return found;
    }

    public static bool SetProcessWindowTitle(int pid, string title)
    {
        if (pid <= 0 || string.IsNullOrEmpty(title)) return false;
        IntPtr hWnd = FindWindowForProcess(pid);
        if (hWnd != IntPtr.Zero)
        {
            SetWindowText(hWnd, title);
            return true;
        }
        return false;
    }

    public void ArrangeTabs()
    {
        try
        {
            var runningRows = _accountList
                .Where(r => r.ActiveProcess != null && !r.ActiveProcess.HasExited)
                .OrderBy(r => r.STT)
                .ToList();

            if (runningRows.Count == 0) return;

            Rectangle workArea = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1920, 1080);

            // Kích thước cửa sổ game
            int winW = 320;
            int winH = 480;
            string screenSize = txtScreenSize.Text.Trim();
            if (TryParseScreenSize(screenSize, out int h, out int w))
            {
                winW = Math.Max(200, w);
                winH = Math.Max(200, h);
            }

            int maxCols = Math.Max(1, workArea.Width / winW);

            for (int i = 0; i < runningRows.Count; i++)
            {
                var row = runningRows[i];
                if (row.ActiveProcess == null || row.ActiveProcess.HasExited) continue;

                IntPtr hWnd = FindWindowForProcess(row.ActiveProcess.Id);
                if (hWnd == IntPtr.Zero)
                {
                    try { hWnd = row.ActiveProcess.MainWindowHandle; } catch { }
                }

                if (hWnd != IntPtr.Zero)
                {
                    int col = i % maxCols;
                    int r = i / maxCols;
                    int posX = workArea.Left + col * winW;
                    int posY = workArea.Top + r * winH;

                    SetWindowPos(hWnd, IntPtr.Zero, posX, posY, winW, winH, SWP_NOZORDER | SWP_NOACTIVATE | SWP_SHOWWINDOW);
                }
            }
        }
        catch { }
    }
    #endregion
}
