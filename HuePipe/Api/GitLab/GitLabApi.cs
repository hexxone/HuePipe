#region

using System.Text.Json;
using HuePipe.Settings;

#endregion

namespace HuePipe.Api.GitLab;

public class GitLabApi(ConnectionSettings settings)
{
    private readonly HttpClient _client = new();

    internal async Task<string?> GetLatestPipelineStatus()
    {
        var requestUri = $"https://{settings.GitLab.BaseUrl}/api/v4/projects/{settings.GitLab.ProjectId}/pipelines";

        // Adding filters
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(settings.GitLab.Branch)) queryParams.Add($"ref={settings.GitLab.Branch}");
        if (!string.IsNullOrEmpty(settings.GitLab.Tag)) queryParams.Add($"tag={settings.GitLab.Tag}");
        if (queryParams.Any()) requestUri += $"?{string.Join("&", queryParams)}";

        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("Private-Token", settings.GitLab.ApiToken);

        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var responseContent = await response.Content.ReadAsStringAsync();
        var pipelines = JsonSerializer.Deserialize<PipelineInfo[]>(responseContent);
        var test = pipelines?[0];
        return test?.status;
    }


    internal string GetColorForPipelineStatus(string? pipelineStatus)
    {
        return pipelineStatus switch
        {
            "running" => settings.Color.Running,
            "success" => settings.Color.Success,
            "failed" => settings.Color.Failed,
            _ => settings.Color.Unknown
        };
    }
}