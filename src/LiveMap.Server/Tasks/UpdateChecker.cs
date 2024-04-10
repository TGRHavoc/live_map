using System.Net;
using System.Net.Http;
using System.Text.Json;
using CitizenFX.Core;
using LiveMap.Server.Models;
using LiveMap.Server.Utils;
using Microsoft.Extensions.Logging;

namespace LiveMap.Server.Tasks;

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public class UpdateChecker(ILogger<UpdateChecker> logger, LiveMap liveMap) : ITask
{
    public async Coroutine Execute()
    {
        ServicePointManager.ServerCertificateValidationCallback += (_,_,_,_) => true;
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        // Spoof the user agent to avoid 403
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");
        try
        {
            var response = await client.GetAsync(Constants.LiveMapUpdateUrl);
            response.EnsureSuccessStatusCode();
            var releaseString = await response.Content.ReadAsStringAsync();
            var release = JsonSerializer.Deserialize<GithubRelease>(releaseString);
            var latestVersion = new Version(release.TagName.Replace("v", ""));

            if (latestVersion.CompareTo(Constants.LiveMapVersion) > 0)
                logger.LogWarning(@"
            |===================================================|
            |             Live Map Update Available             |
            |===================================================|
            Current Version: {CurrentVersion}
            New Version: {NewVersion}
            Download at: https://github.com/TGRHavoc/live_map/releases/latest", Constants.LiveMapVersion,
                    latestVersion);
            else
                logger.LogInformation("LiveMap is up to date");

            // We only need to check for updates once
            await Coroutine.Delay(0);
            liveMap.UnregisterTick(Execute);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error checking for updates");
        }
    }
}