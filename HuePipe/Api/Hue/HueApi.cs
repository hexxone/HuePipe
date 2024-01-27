#region

using System.Drawing;
using System.Text;
using System.Text.Json;
using HuePipe.Settings;

#endregion

namespace HuePipe.Api.Hue;

public class HueApi(ConnectionSettings settings)
{
    private readonly HttpClient _client = new();

    internal async Task PrintHueLights()
    {
        var requestUri = $"http://{settings.Hue.BridgeIp}/api/{settings.Hue.ApiKey}/lights";
        var response = await _client.GetAsync(requestUri);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var lights = JsonSerializer.Deserialize<JsonElement>(responseContent);

            foreach (var light in lights.EnumerateObject())
            {
                var id = light.Name;
                var name = light.Value.GetProperty("name").GetString();

                Console.WriteLine($"ID: {id}, Name: {name}");
            }
        }
        else
        {
            Console.WriteLine("Error getting Hue light infos. Check you Bridge Ip and ApiKey. Exiting..");
            Environment.Exit(1);
        }
    }

    internal async Task SetHueStatus(Color color, int normalAlpha = 254, int blinkCount = 3, int blinkInterval = 500,
        int blinkAlpha = 127, bool revertAfterDelay = true, int revertDelay = 10000)
    {
        var originalStates = new Dictionary<int, string>();

        async Task UpdateLambda(int lightId)
        {
            // Store the original state
            var requestUri = $"http://{settings.Hue.BridgeIp}/api/{settings.Hue.ApiKey}/lights/{lightId}";
            var response = await _client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var light = JsonSerializer.Deserialize<JsonElement>(content);
                originalStates[lightId] = light.GetProperty("state").GetRawText();
            }

            Console.WriteLine($"Setting Light Id '{lightId}' to Color '{color.ToArgb()}'");

            // Blinking effect
            for (var i = 0; i < blinkCount; i++)
            {
                await SetLightColor(lightId, color, normalAlpha);
                await Task.Delay(blinkInterval);
                await SetLightColor(lightId, color, blinkAlpha);
                await Task.Delay(blinkInterval);
            }

            await SetLightColor(lightId, color, normalAlpha);
        }

        var parallelTasks = settings.Hue.LightIds.Select(UpdateLambda);
        await Task.WhenAll(parallelTasks);

        if (revertAfterDelay)
        {
            await Task.Delay(revertDelay);
            foreach (var lightId in settings.Hue.LightIds)
            {
                if (!originalStates.TryGetValue(lightId, out var state)) continue;
                var stringContent = new StringContent(state, Encoding.UTF8, "application/json");
                var requestUri = $"http://{settings.Hue.BridgeIp}/api/{settings.Hue.ApiKey}/lights/{lightId}/state";
                var resetResponse = await _client.PutAsync(requestUri, stringContent);
                if (resetResponse.IsSuccessStatusCode)
                    Console.WriteLine($"Reset Light Id '{lightId}' to previous Color.");
            }
        }
    }

    private async Task SetLightColor(int lightId, Color color, int bri)
    {
        var xy = GetColorToHueXy(color);

        var content = JsonSerializer.Serialize(new { on = true, xy, bri });
        var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

        var requestUri = $"http://{settings.Hue.BridgeIp}/api/{settings.Hue.ApiKey}/lights/{lightId}/state";
        await _client.PutAsync(requestUri, stringContent);
    }


    /// <summary>
    ///     https://stackoverflow.com/a/22649803
    /// </summary>
    private static double[] GetColorToHueXy(Color c)
    {
        // For the hue bulb the corners of the triangle are:
        // -Red: 0.675, 0.322
        // -Green: 0.4091, 0.518
        // -Blue: 0.167, 0.04
        var normalizedToOne = new double[3];
        float cRed = c.R;
        float cGreen = c.G;
        float cBlue = c.B;
        normalizedToOne[0] = cRed / 255;
        normalizedToOne[1] = cGreen / 255;
        normalizedToOne[2] = cBlue / 255;
        float red, green, blue;

        // Make red more vivid
        if (normalizedToOne[0] > 0.04045)
            red = (float)Math.Pow((normalizedToOne[0] + 0.055) / (1.0 + 0.055), 2.4);
        else
            red = (float)(normalizedToOne[0] / 12.92);

        // Make green more vivid
        if (normalizedToOne[1] > 0.04045)
            green = (float)Math.Pow((normalizedToOne[1] + 0.055) / (1.0 + 0.055), 2.4);
        else
            green = (float)(normalizedToOne[1] / 12.92);

        // Make blue more vivid
        if (normalizedToOne[2] > 0.04045)
            blue = (float)Math.Pow((normalizedToOne[2] + 0.055) / (1.0 + 0.055), 2.4);
        else
            blue = (float)(normalizedToOne[2] / 12.92);

        var lX = red * 0.649926 + green * 0.103455 + blue * 0.197109;
        var lY = red * 0.234327 + green * 0.743075 + blue * 0.022598;
        var lZ = red * 0.0000000 + green * 0.053077 + blue * 1.035763;
        var x = lX / (lX + lY + lZ);
        var y = lY / (lX + lY + lZ);

        return [x, y];
    }
}