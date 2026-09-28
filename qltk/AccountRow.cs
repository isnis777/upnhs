using System.Diagnostics;
using System.Text.Json.Serialization;

namespace QLTK;

public class AccountRow
{
    public int STT { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserAo { get; set; } = string.Empty;
    public string CharacterName { get; set; } = string.Empty;
    public int Server { get; set; } = 1;
    public string Planet { get; set; } = string.Empty;
    public string Power { get; set; } = string.Empty;
    public string Gold { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PID { get; set; } = string.Empty;

    [JsonIgnore]
    public bool IsRealAccount => !string.IsNullOrWhiteSpace(Username);

    [JsonIgnore]
    public string DisplayAccount
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(Username)) return Username;
            if (!string.IsNullOrWhiteSpace(UserAo)) return UserAo;
            return "Chơi mới (ảo)";
        }
    }

    // Internal management
    public string ClientId { get; set; } = string.Empty;
    public bool IsStoppedManually { get; set; }

    [JsonIgnore]
    public Process? ActiveProcess { get; set; }

    [JsonIgnore]
    public long LastPowerVal { get; set; } = -1;

    [JsonIgnore]
    public long LastTiemNangVal { get; set; } = -1;

    [JsonIgnore]
    public DateTime LastProgressTime { get; set; } = DateTime.MinValue;

    [JsonIgnore]
    public DateTime ReopenAt { get; set; } = DateTime.MinValue;

    [JsonIgnore]
    public bool IsWaitingReopen { get; set; } = false;

    public AccountRow()
    {
    }

    public AccountRow(int stt, int server, string clientId)
    {
        STT = stt;
        Server = server;
        ClientId = clientId;
    }

    public AccountRow(int stt, string username, string password, int server, string clientId)
    {
        STT = stt;
        Username = username;
        Password = password;
        Server = server;
        ClientId = clientId;
    }
}
