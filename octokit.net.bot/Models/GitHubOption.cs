namespace Octokit.Bot
{
    public class GitHubOption
    {
        public string WebHookSecret { get; set; }

        public int AppIdentifier { get; set; }

        public string PrivateKeyFileName { get; set; }

        public string AppName { get; set; }
    }
}