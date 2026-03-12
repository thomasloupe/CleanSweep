using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Octo = Octokit;

namespace CleanSweep.Classes
{
    internal class UpdateCheck
    {
        private static Octo.GitHubClient _octoClient;

        public static async Task CheckForUpdates()
        {
            if (_octoClient == null)
            {
                _octoClient = new Octo.GitHubClient(new Octo.ProductHeaderValue("CleanSweep"));
            }

            try
            {
                var version = CleanSweepVersion.Version;
                var releases = await _octoClient.Repository.Release.GetAll("thomasloupe", "CleanSweep").ConfigureAwait(false);
                Octo.Release latest = releases.OrderByDescending(r => r.PublishedAt).FirstOrDefault();

                if (latest == null || version == latest.TagName)
                {
                    MessageBox.Show($"You are on the latest version: {version}.", "Version Check");
                    return;
                }

                if (version != latest.TagName)
                {
                    // Show message with a single link to get the latest version
                    CustomMessageBox.Show(
                        $"A new version is available: {latest.TagName}.\nYou are currently on version: {version}.",
                        "Version Update Available",
                        ("Get the latest version", $"https://github.com/thomasloupe/CleanSweep/releases/tag/{latest.TagName}")
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to check for updates: {ex.Message}", "Error");
            }
        }
    }
}
