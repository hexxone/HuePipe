namespace HuePipe.Settings;

public class ColorSettings
{
    public string Success { get; set; } = "lime";
    public string Failed { get; set; } = "lime";
    public string Running { get; set; } = "blue";
    public string Unknown { get; set; } = "purple";

    public int NormalAlpha { get; set; } = 254; // [0 - 254]

    public int BlinkCount { get; set; } = 3;
    public int BlinkAlpha { get; set; } = 127; // [0 - 254]
    public int BlinkInterval { get; set; } = 500;

    public bool RevertAfterDelay { get; set; } = true;
    public int RevertDelay { get; set; } = 10000;
}