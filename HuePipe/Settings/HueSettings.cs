namespace HuePipe.Settings;

public class HueSettings
{
    public string BridgeIp { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public int[] LightIds { get; set; } = default!;
}