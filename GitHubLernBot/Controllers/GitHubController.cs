using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHubLernBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GitHubLernBot.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly GitHubOptions _options;

        public GitHubController(IOptions<GitHubOptions> options)
        {
            _options = options.Value;
        }
    }
}
