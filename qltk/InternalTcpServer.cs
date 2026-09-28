using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace QLTK;

public class InternalTcpServer
{
    private TcpListener? _listener;
    private bool _isRunning;
    private readonly int _port;
    private readonly ConcurrentDictionary<string, TcpClient> _clients = new();

    public event Action<string>? OnLog;
    public event Action<string, string, int, string, string, string, string, string, string, string>? OnDataReceived;
    public event Action<string, long, long>? OnClientStuckTimeout;
    public event Func<string, (int server, bool randomName, bool dytest, string screenSize, string userAo, int fps, bool optimizeRam, bool blackScreen)>? OnConfigRequest;
    public event Action<string>? OnClientDisconnected;

    public InternalTcpServer(int port = 9999)
    {
        _port = port;
    }

    public void Start()
    {
        if (_isRunning) return;

        try
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            _isRunning = true;
            OnLog?.Invoke($"TCP Server đang lắng nghe trên cổng {_port}...");

            Thread acceptThread = new(AcceptClientsLoop)
            {
                IsBackground = true
            };
            acceptThread.Start();
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"Lỗi khởi động TCP Server: {ex.Message}");
        }
    }

    public void Stop()
    {
        _isRunning = false;
        try
        {
            foreach (var kvp in _clients)
            {
                try
                {
                    kvp.Value.Close();
                }
                catch { }
            }
            _clients.Clear();
            _listener?.Stop();
            OnLog?.Invoke("TCP Server đã dừng.");
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"Lỗi khi dừng server: {ex.Message}");
        }
    }

    public void Broadcast(string message)
    {
        try
        {
            byte[] b = Encoding.UTF8.GetBytes(message + "\n");
            foreach (var client in _clients.Values)
            {
                try
                {
                    if (client.Connected)
                    {
                        var stream = client.GetStream();
                        stream.Write(b, 0, b.Length);
                        stream.Flush();
                    }
                }
                catch { }
            }
        }
        catch { }
    }

    private void AcceptClientsLoop()
    {
        while (_isRunning && _listener != null)
        {
            try
            {
                TcpClient client = _listener.AcceptTcpClient();
                Thread clientThread = new(() => HandleClient(client))
                {
                    IsBackground = true
                };
                clientThread.Start();
            }
            catch
            {
                if (!_isRunning) break;
            }
        }
    }

    private void HandleClient(TcpClient client)
    {
        string? assignedClientId = null;
        NetworkStream? stream = null;

        try
        {
            stream = client.GetStream();
            byte[] buffer = new byte[4096];
            StringBuilder sb = new();

            while (_isRunning && client.Connected)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;

                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                string content = sb.ToString();

                int newlineIndex;
                while ((newlineIndex = content.IndexOf('\n')) != -1)
                {
                    string line = content.Substring(0, newlineIndex).Trim();
                    content = content.Substring(newlineIndex + 1);

                    if (!string.IsNullOrEmpty(line))
                    {
                        ProcessMessage(client, stream, ref assignedClientId, line);
                    }
                }

                sb.Clear();
                sb.Append(content);
            }
        }
        catch
        {
        }
        finally
        {
            if (!string.IsNullOrEmpty(assignedClientId))
            {
                _clients.TryRemove(assignedClientId, out _);
                OnClientDisconnected?.Invoke(assignedClientId);
                OnLog?.Invoke($"Tab [{assignedClientId}] đã ngắt kết nối.");
            }

            try
            {
                stream?.Close();
                client.Close();
            }
            catch { }
        }
    }

    private void ProcessMessage(TcpClient client, NetworkStream stream, ref string? assignedClientId, string message)
    {
        try
        {
            string[] parts = message.Split('|');
            if (parts.Length == 0) return;

            string cmd = parts[0];

            if (cmd.Equals("REQ_CONFIG", StringComparison.OrdinalIgnoreCase) && parts.Length >= 2)
            {
                string clientId = parts[1];
                assignedClientId = clientId;
                _clients[clientId] = client;

                if (OnConfigRequest != null)
                {
                    var cfg = OnConfigRequest.Invoke(clientId);
                    string resp = $"CONFIG|{clientId}|{cfg.server}|{(cfg.randomName ? 1 : 0)}|{(cfg.dytest ? 1 : 0)}|{cfg.screenSize}|{cfg.userAo}|{cfg.fps}|{(cfg.optimizeRam ? 1 : 0)}|{(cfg.blackScreen ? 1 : 0)}\n";
                    byte[] b = Encoding.UTF8.GetBytes(resp);
                    stream.Write(b, 0, b.Length);
                    stream.Flush();
                }
            }
            else if (cmd.Equals("DATA", StringComparison.OrdinalIgnoreCase) && parts.Length >= 9)
            {
                // DATA|clientId|cName|server|planet|power|gold|status|pid|userAo|tiemNang
                string clientId = parts[1];
                assignedClientId = clientId;
                _clients[clientId] = client;

                string cName = parts[2];
                int.TryParse(parts[3], out int server);
                string planet = parts[4];
                string power = parts[5];
                string gold = parts[6];
                string status = parts[7];
                string pid = parts[8];
                string userAo = parts.Length >= 10 ? parts[9] : string.Empty;
                string tiemNang = parts.Length >= 11 ? parts[10] : string.Empty;

                OnDataReceived?.Invoke(clientId, cName, server, planet, power, gold, status, pid, userAo, tiemNang);
            }
            else if (cmd.Equals("STUCK_TIMEOUT", StringComparison.OrdinalIgnoreCase) && parts.Length >= 2)
            {
                string clientId = parts[1];
                long pVal = 0, tnVal = 0;
                if (parts.Length >= 3) long.TryParse(parts[2], out pVal);
                if (parts.Length >= 4) long.TryParse(parts[3], out tnVal);
                OnClientStuckTimeout?.Invoke(clientId, pVal, tnVal);
            }
            else if (cmd.Equals("DISCONNECT", StringComparison.OrdinalIgnoreCase) && parts.Length >= 2)
            {
                string clientId = parts[1];
                OnClientDisconnected?.Invoke(clientId);
            }
        }
        catch (Exception ex)
        {
            OnLog?.Invoke($"Lỗi xử lý gói tin TCP ({message}): {ex.Message}");
        }
    }

    public void SendToClient(string clientId, string message)
    {
        if (_clients.TryGetValue(clientId, out TcpClient? client) && client.Connected)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message + "\n");
                NetworkStream stream = client.GetStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
            catch { }
        }
    }
}
