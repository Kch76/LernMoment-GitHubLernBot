using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubLernBot.Models
{
    public class GitHubOptions
    {
        public const string GitHub = "GitHub";

        public int AppId { get; set; }
        public string PrivateKey { get; set; }
        public string Secret { get; set; }

    }
}
