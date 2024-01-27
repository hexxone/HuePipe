namespace HuePipe.Settings;

public class ConnectionSettings
{
    public string Name { get; set; } = "Backend";
    public int Interval { get; set; } = 10000;
    public GitlabSettings GitLab { get; set; } = new();
    public HueSettings Hue { get; set; } = new();
    public ColorSettings Color { get; set; } = new();
}