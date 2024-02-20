using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using LiveMap.Models;
using Microsoft.Extensions.Logging;

namespace LiveMap.Utils
{
    public class UpdateChecker
    {
        private static string Url = "https://api.github.com/repos/TGRHavoc/live_map/releases/latest";
        private static Version CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version;

        private static ILogger<UpdateChecker> _logger = LiveMap.CreateLogger<UpdateChecker>();

        private static string GetUpdateMessage
            => @"
            |===================================================|
            |             Live Map Update Available             |
            |===================================================|
            | Current Version: {CurrentVersion}                 |
            | New Version: {NewVersion}                         |
            | Download at: https://github.com/TGRHavoc/live_map |
            |===================================================|";

        public static async Task CheckForUpdates()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            // Spoof the user agent to avoid 403
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");
            try
            {
                var response = await client.GetAsync(Url);
                response.EnsureSuccessStatusCode();
                var releaseString = await response.Content.ReadAsStringAsync();
                var release = JsonSerializer.Deserialize<GithubRelease>(releaseString);
                var latestVersion = new Version(release.TagName.Replace("v", ""));

                if (latestVersion.CompareTo(CurrentVersion) > 0)
                {
                    _logger.LogWarning(@"
            |===================================================|
            |             Live Map Update Available             |
            |===================================================|
            Current Version: {CurrentVersion}
            New Version: {NewVersion}
            Download at: https://github.com/TGRHavoc/live_map/releases/latest", CurrentVersion, latestVersion);

                    //API.SendNuiMessage("{\"type\": \"updateAvailable\", \"version\": \"" + latestVersion + "\"}");
                }
                else
                {
                    _logger.LogInformation("LiveMap is up to date");
                }
            }
            catch (Exception e)
            {
                //API.SendNuiMessage("{\"type\": \"updateError\", \"message\": \"" + e.Message + "\"}");
                _logger.LogError(e, "Error checking for updates");
            }
        }
    }
}