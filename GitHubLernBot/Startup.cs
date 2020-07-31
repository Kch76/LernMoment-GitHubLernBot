using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHubLernBot.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octokit.Bot;

namespace GitHubLernBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Inject GitHub configuration wich should be read from user secrets
            services.Configure<GitHubOptions>(Configuration.GetSection(GitHubOptions.GitHub));

            services.AddControllers();

            // octokit.bot reads payload synchronously in the BindModelAsync method. This results in an exception.
            // Thus I try the following as per this suggestions: https://github.com/graphql-dotnet/graphql-dotnet/issues/1116#issuecomment-517444189
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // register webhook event handler
            services.AddScoped<IssueEventHandler>();

            // wire the handlers and corresponding events
            services.AddGitHubWebHookHandler(registry => registry
                .RegisterHandler<IssueEventHandler>("issues"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
