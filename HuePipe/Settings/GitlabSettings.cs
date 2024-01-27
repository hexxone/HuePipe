namespace HuePipe.Settings;

public class GitlabSettings
{
    public string BaseUrl { get; set; } = default!;
    public string ApiToken { get; set; } = default!;
    public string ProjectId { get; set; } = default!;
    public string? Branch { get; set; }
    public string? Tag { get; set; }
}