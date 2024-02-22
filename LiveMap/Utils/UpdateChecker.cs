using System.Net;
using System.Text.Json;
using LiveMap.Models;
using Microsoft.Extensions.Logging;

namespace LiveMap.Utils;

public class UpdateChecker
{
    private readonly ILogger<UpdateChecker> _logger;

    public UpdateChecker(ILogger<UpdateChecker> logger)
    {
        _logger = logger;
    }

    public async Task CheckForUpdates()
    {
        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
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
                _logger.LogWarning(@"
            |===================================================|
            |             Live Map Update Available             |
            |===================================================|
            Current Version: {CurrentVersion}
            New Version: {NewVersion}
            Download at: https://github.com/TGRHavoc/live_map/releases/latest", Constants.LiveMapVersion,
                    latestVersion);
            else
                _logger.LogInformation("LiveMap_OLD is up to date");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error checking for updates");
        }
    }
}