namespace QLTK;

public class AppSettings
{
    public int Threads { get; set; } = 1;
    public int Server { get; set; } = 1;
    public bool RandomName { get; set; } = true;
    public bool DyTest { get; set; }
    public string ScreenSize { get; set; } = "320 x 480";
    public int OpenTabDelaySeconds { get; set; } = 3;
    public bool AutoTileWindows { get; set; } = true;
    public int InactiveTimeoutSeconds { get; set; } = 60;
    public int ReopenDelaySeconds { get; set; } = 30;
    public bool OptimizeRam { get; set; } = true;
    public bool BlackScreen { get; set; } = false;
    public int TargetFps { get; set; } = 20;
}
