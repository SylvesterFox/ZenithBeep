
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using Lavalink4NET;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Handler;
using ReworkZenithBeep.Module.Music;
using ReworkZenithBeep.Module.RolesGet;
using ReworkZenithBeep.Module.Rooms;
using ReworkZenithBeep.Module.Utils;
using ReworkZenithBeep.Services;
using ReworkZenithBeep.Settings;
using System.Reflection;
using DSharpPlus.Interactivity.Extensions;



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
        private readonly BotConfig CONFIG = Program.CONFIG;
        
        private RoleSelectorsHandler? _roleSelectorHandler;
        private VoiceRoomsHandler? _voiceRoomsHandler;

        
        public HostBotBase(IServiceProvider serviceProvider, DiscordClient discord, DataBot dataBot)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(discord);

            _serviceProvider = serviceProvider;
            _discordClient = discord;


            if (CONFIG.AUDIO_SERVICES)
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


            var interactivity = _discordClient.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(5)
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
                await Status.UpdateStatus(client);
            }

            // Prefix command
            next.RegisterCommands<UtilityNextCommand>();


            // Audio command
            if (CONFIG.AUDIO_SERVICES)
            {
                // Comannd prefix
                next.RegisterCommands<MusicNextCommand>();
                // Slash command
                slash.RegisterCommands<MusicSlashCommand>();

            }

            // Using database
            if (CONFIG.NODB_MODE != true)
            {
                // Command prefix
                next.RegisterCommands<UtilityForDataNextCommand>();
                // Slash command
                slash.RegisterCommands<UtilitySlashCommand>();
                slash.RegisterCommands<RoleSelectorsSlash>();
                slash.RegisterCommands<RoomsSelectorsSlash>();
                // Events
                _roleSelectorHandler = new RoleSelectorsHandler(_serviceProvider);
                _voiceRoomsHandler = new VoiceRoomsHandler(_serviceProvider);
                

                _discordClient.MessageReactionAdded += _roleSelectorHandler.MessageReactionAdd;
                _discordClient.MessageReactionRemoved += _roleSelectorHandler.MessageReactionRemove;
                _discordClient.VoiceStateUpdated += _voiceRoomsHandler.OnRoomStateUpdated;
                
            }  

            _discordClient.Ready += SetResult;
            _discordClient.GuildCreated += GuildHandler.OnGuildAvailble;
            _discordClient.GuildDeleted += GuildHandler.OnGuildUavailble;


            stoppingToken.Register(async () =>
            {
                Console.WriteLine("Shutting down...");


                if (_roleSelectorHandler != null)
                {
                    _discordClient.MessageReactionAdded -= _roleSelectorHandler.MessageReactionAdd;
                    _discordClient.MessageReactionRemoved -= _roleSelectorHandler.MessageReactionRemove;
                }

                if (_voiceRoomsHandler != null)
                {
                    _discordClient.VoiceStateUpdated -= _voiceRoomsHandler.OnRoomStateUpdated;
                }

                _discordClient.Ready -= SetResult;
                _discordClient.GuildCreated -= GuildHandler.OnGuildAvailble;
                _discordClient.GuildDeleted -= GuildHandler.OnGuildUavailble;
                await _discordClient.DisconnectAsync().ConfigureAwait(false);
                _discordClient.Dispose();
                var db = _serviceProvider.GetRequiredService<BotContext>();
                await db.DisposeAsync();

                Console.WriteLine("Shutdown complete.");
                
            });



            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                // Expected when the service is stopping
            }
            
        }

        
    }
}
