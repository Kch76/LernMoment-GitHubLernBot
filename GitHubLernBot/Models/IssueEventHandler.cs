using Octokit.Bot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GitHubLernBot.Models
{
    public class IssueEventHandler : IHookHandler
    {
        public async Task Handle(EventContext eventContext)
        {
            if (!eventContext.WebHookEvent.IsMessageAuthenticated)
            {
                // message is not issued by GitHub. Possibly from a malucious attacker.
                // log it and return;
                Debug.WriteLine("ERROR: IssueEvent could not be authenticated!");
                return;
            }

            var action = (string)eventContext.WebHookEvent.GetPayload().action;
            var title = (string)eventContext.WebHookEvent.GetPayload().issue.title;
            var repository = (string)eventContext.WebHookEvent.GetPayload().repository.name;

            Debug.WriteLine($"Issue with title '{title}' on repository '{repository}' was '{action}'");

            if (action.Contains("opened"))
            {
                try
                {
                    var issueNumber = (int)eventContext.WebHookEvent.GetPayload().issue.number;
                    var repositoryId = (long)eventContext.WebHookEvent.GetPayload().repository.id;
                    var commentResponse = await eventContext.InstallationContext
                                           .Client
                                           .Issue.Comment
                                           .Create(repositoryId, issueNumber, "Willkommen bei LernMoment und auf GitHub!");

                    Debug.WriteLine($"Habe folgenden Kommentar gepostet: {commentResponse}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception gefangen: {ex}");
                    throw;
                }
            }
        }
    }
}
