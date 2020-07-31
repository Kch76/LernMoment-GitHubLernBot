namespace Octokit.Bot
{
    public class GitHubOptions
    {
        public const string GitHub = "GitHub";

        public string WebHookSecret { get; set; }

        public int AppId { get; set; }

        public string PrivateKey { get; set; }

        public string AppName { get; set; }
    }
}