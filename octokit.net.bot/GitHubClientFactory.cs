using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Octokit.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Octokit.Bot
{
    public static class GitHubClientFactory
    {
        public static  GitHubClient CreateGitHubAppClient(GitHubOptions option)
        {
            return GetAppClient(option, option.AppName);

        }

        public static async Task<InstallationContext> CreateGitHubInstallationClient(GitHubClient appClient, long installationId, string appName)
        {
            return await GetInstallationContext(appClient, installationId, appName);
        }

        public static async Task<InstallationContext> CreateGitHubInstallationClient(GitHubOptions option,long installationId)
        {
            return await CreateGitHubInstallationClient(CreateGitHubAppClient(option), installationId, option.AppName);
        }

        private static async Task<InstallationContext> GetInstallationContext(GitHubClient appClient, long installationId, string appName)
        {
            var accessToken = await appClient.GitHubApps.CreateInstallationToken(installationId);

            var installationClient = new ResilientGitHubClientFactory()
                .Create(new ProductHeaderValue($"{appName}-Installation{installationId}"), new Credentials(accessToken.Token), new InMemoryCacheProvider());

            return new InstallationContext(installationClient,accessToken);
        }

        private static GitHubClient GetAppClient(GitHubOptions option, string appName)
        {
            // Use GitHubJwt library to create the GitHubApp Jwt Token using our private certificate PEM file
            var generator = new GitHubJwt.GitHubJwtFactory(
                new GitHubJwt.StringPrivateKeySource(option.PrivateKey),
                new GitHubJwt.GitHubJwtFactoryOptions
                {
                    AppIntegrationId = option.AppId, // The GitHub App Id
                    ExpirationSeconds = 600 // 10 minutes is the maximum time allowed
                }
            );
            var jwtToken = generator.CreateEncodedJwtToken();

            return GetGitHubClient(appName, jwtToken);
        }

        private static GitHubClient GetGitHubClient(string appName, string jwtToken)
        {
            return new ResilientGitHubClientFactory()
                .Create(new ProductHeaderValue(appName), new Credentials(jwtToken, AuthenticationType.Bearer), new InMemoryCacheProvider());
        }
    }
}
