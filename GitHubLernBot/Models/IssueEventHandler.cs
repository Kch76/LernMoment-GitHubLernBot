using Octokit;
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
                    var client = eventContext.InstallationContext.Client;
                    var payload = eventContext.WebHookEvent.GetPayload();

                    var creatorName = (string)payload.issue.user.login;
                    var respositoryName = (string)payload.repository.name;
                    var ownerName = (string)payload.repository.owner.login;

                    var allIssuesForUser = new RepositoryIssueRequest
                    {
                        Creator = creatorName,
                        State = ItemStateFilter.All,
                        Filter = IssueFilter.All
                    };
                    var issues = await client.Issue.GetAllForRepository(ownerName, respositoryName, allIssuesForUser);
                    var issueCountForCreator = issues.Where(i => i.PullRequest == null).Count();
                    if (issueCountForCreator == 1)
                    {
                        var issueNumber = (int)payload.issue.number;
                        var repositoryId = (long)payload.repository.id;
                        _ = await eventContext.InstallationContext
                                                .Client
                                                .Issue.Comment
                                                .Create(repositoryId, issueNumber, "Willkommen bei LernMoment und auf GitHub!");
                        Debug.WriteLine("Habe einen Willkommens-Kommentar gepostet!");
                    }
                    else
                    {
                        Debug.WriteLine("Der Ersteller ist kein First-Time-Contributor!");
                    }

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
