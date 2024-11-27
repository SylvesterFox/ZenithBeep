
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Lavalink4NET;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Handler;
using ReworkZenithBeep.Module.Music;
using ReworkZenithBeep.Module.RolesGet;
using ReworkZenithBeep.Module.Utils;
using ReworkZenithBeep.Services;
using ReworkZenithBeep.Settings;
using System.Reflection;



namespace ReworkZenithBeep
{
    internal class HostBotBase : BackgroundService
    {
        public static IAudioService AudioService { get; private set; }
        public static PaginationService Pagination { get; private set; }

        public static string versionString { get; private set; } = Assembly.GetExecutingAssembly()?
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion ?? "null version";

        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordClient _discordClient;
        private readonly BotConfig _botConfig;
        

        public HostBotBase(IServiceProvider serviceProvider, DiscordClient discord, DataBot dataBot)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(discord);

            this._serviceProvider = serviceProvider;
            this._discordClient = discord;
            _botConfig = Settings.SettingsManager.Instance.LoadedConfig;

            if (_botConfig.AUDIOSERICES != true)
            {
                AudioService = serviceProvider.GetRequiredService<IAudioService>();
            }
            
            Pagination = serviceProvider.GetRequiredService<PaginationService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            var slash = _discordClient
                .UseSlashCommands(new SlashCommandsConfiguration
                {
                    Services = _serviceProvider
                });


            var next = _discordClient
                .UseCommandsNext(new CommandsNextConfiguration
                {
                    StringPrefixes = ["$"],
                    Services = _serviceProvider
                });


            try
            {
                await _discordClient.ConnectAsync().ConfigureAwait(false);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(0);
            }

            var readyTaskCompletionSource = new TaskCompletionSource();

            

            async Task SetResult(DiscordClient client, ReadyEventArgs eventArgs)
            {
                var db = _serviceProvider.GetRequiredService<BotContext>();
                await db.Database.MigrateAsync();
                Console.WriteLine(" _____         _ _   _   _____            ");
                Console.WriteLine("|__   |___ ___|_| |_| |_| __  |___ ___ ___");
                Console.WriteLine("|   __| -_|   | |  _|   | __ -| -_| -_| . |");
                Console.WriteLine("|_____|___|_|_|_|_| |_|_|_____|___|___|  _|");
                Console.WriteLine("                                      |_|  ");


                Console.WriteLine($"Version: {versionString}");


            }

            // Prefix command
            next.RegisterCommands<UtilityNextCommand>();


            // Audio command
            if (_botConfig.AUDIOSERICES != true)
            {
                // Comannd prefix
                next.RegisterCommands<MusicNextCommand>();
                // Slash command
                slash.RegisterCommands<MusicSlashCommand>();

            }

            // Using database
            if (_botConfig.NODB_MODE != true)
            {
                // Command prefix
                next.RegisterCommands<UtilityForDataNextCommand>();
                // Slash command
                slash.RegisterCommands<UtilitySlashCommand>();
                slash.RegisterCommands<RoleSelectorsSlash>();
            }

            // Events
            var roleSelectorHandler = new RoleSelectorsHandler(_serviceProvider);
            var voiceRoomsHandler = new VoiceRoomsHandler();

            _discordClient.MessageReactionAdded += roleSelectorHandler.MessageReactionAdd;
            _discordClient.MessageReactionRemoved += roleSelectorHandler.MessageReactionRemove;
            _discordClient.VoiceStateUpdated += VoiceRoomsHandler.voiceRoomsHandler;

            _discordClient.Ready += SetResult;

            await readyTaskCompletionSource.Task.ConfigureAwait(false);
            _discordClient.Ready -= SetResult;


            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken).ConfigureAwait(false);
        }


    }
}
