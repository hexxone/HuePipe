#region

using System.Drawing;
using HuePipe.Api.GitLab;
using HuePipe.Api.Hue;
using HuePipe.Settings;
using Microsoft.Extensions.Configuration;

#endregion

namespace HuePipe;

internal class App
{
    private readonly IConfigurationRoot _configuration;

    internal App()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
    }

    internal async Task Start()
    {
        Console.WriteLine("Running HuePipe - by hexx.one");

        var connections = _configuration.GetSection("Connections").Get<ConnectionSettings[]>() ??
                          throw new Exception("Settings not found");

        // parallel processing for each connection
        var tasks = connections.Select(Run);
        await Task.WhenAll(tasks);
    }

    private static async Task Run(ConnectionSettings connectionSettings)
    {
        var hueApi = new HueApi(connectionSettings);
        var gitApi = new GitLabApi(connectionSettings);

        await hueApi.PrintHueLights();

        string? lastPipelineStatus = null;
        while (true)
        {
            var pipelineStatus = await gitApi.GetLatestPipelineStatus();

            if (pipelineStatus != lastPipelineStatus)
            {
                Console.WriteLine($"{connectionSettings.Name}: New Pipeline Status: '{pipelineStatus}'");

                var pipeLineColor = gitApi.GetColorForPipelineStatus(pipelineStatus);
                var translatedColor = ColorTranslator.FromHtml(pipeLineColor);

                await hueApi.SetHueStatus(translatedColor, connectionSettings.Color.NormalAlpha,
                    connectionSettings.Color.BlinkCount,
                    connectionSettings.Color.BlinkInterval, connectionSettings.Color.BlinkAlpha,
                    connectionSettings.Color.RevertAfterDelay,
                    connectionSettings.Color.RevertDelay);

                lastPipelineStatus = pipelineStatus;
            }

            await Task.Delay(Math.Max(100, connectionSettings.Interval));
        }
    }
}