using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Octokit.Bot;

namespace GitHubLernBot.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly WebHookHandlerRegistry _registry;

        public GitHubController(WebHookHandlerRegistry registry)
        {
            _registry = registry;
        }

        [HttpPost]
        public async Task<ActionResult> HandleGitHubHooks(WebHookEvent webhookEvent)
        {
            await _registry.Handle(webhookEvent);

            return Ok();
        }
    }
}
