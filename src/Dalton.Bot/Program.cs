using System.Reflection;
using Dalton.Bot.Models;
using Dalton.Bot.Services;
using Dalton.Bot.Services.Hosted;
using Discord;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dalton.Bot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);

            // Cancel if the user presses CTRL+C.
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (_, _) =>
            {
                cancellationTokenSource.Cancel();
            };

            var consoleTask = builder.RunConsoleAsync(cancellationTokenSource.Token);
            consoleTask.Wait(cancellationTokenSource.Token);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                config.AddJsonFile("appsettings.json", false);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices(ConfigureServices);

        public static void ConfigureServices(
            HostBuilderContext hostContext,
            IServiceCollection services)
        {
            services.AddMediatR(typeof(Program));
            services.AddOptions();
            services.Configure<Settings>(hostContext.Configuration.GetSection(nameof(Settings)));
            services.AddMemoryCache();

            var socketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                GatewayIntents = GatewayIntents.DirectMessages |
                                 GatewayIntents.GuildIntegrations |
                                 GatewayIntents.GuildMembers |
                                 GatewayIntents.GuildMessageReactions |
                                 GatewayIntents.GuildMessages |
                                 GatewayIntents.GuildPresences |
                                 GatewayIntents.Guilds,
            };

            services.AddSingleton<Random>();
            services.AddSingleton(new DiscordSocketClient(socketConfig));
            services.AddSingleton(new Discord.Commands.CommandService());
            services.AddSingleton<CommandService>();
            services.AddSingleton<StartupService>();
            services.AddSingleton<GuildInteractionService>();
            services.AddHostedService<DiscordBotService>();
        }
    }
}