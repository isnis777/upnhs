using System.Diagnostics;
using System.Text.Json.Serialization;

namespace QLTK;

public class AccountRow
{
    public int STT { get; set; }
    public string UserAo { get; set; } = string.Empty;
    public string CharacterName { get; set; } = string.Empty;
    public int Server { get; set; } = 1;
    public string Planet { get; set; } = string.Empty;
    public string Power { get; set; } = string.Empty;
    public string Gold { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PID { get; set; } = string.Empty;

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
}
