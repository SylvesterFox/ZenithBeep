using DSharpPlus;
using Lavalink4NET.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Handler;
using ReworkZenithBeep.Services;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep
{
    internal class Program
    {
        public static BotConfig _botConfig;
        static void Main(string[] args)
        {
            var dataConfig = DataConfig.InitDataConfig();

            if (!SettingsManager.Instance.LoadConfiguration())
            {
                Console.WriteLine("Configuration is not loaded\nPress any key to exit...");
                return;
            }

            if (dataConfig == null)
            {
                Console.WriteLine("Data configuration is not loaded");
                return;
            }

            _botConfig = SettingsManager.Instance.LoadedConfig;

            var builder = new HostApplicationBuilder();
            
            if (_botConfig.NODB_MODE != true) {
                
                builder.Services.AddDbContextFactory<BotContext>(o => o.UseNpgsql(dataConfig, x =>
                    x.MigrationsAssembly("ReworkZenithBeep.Data.Migrations")));
                builder.Services.AddSingleton<DataBot>();
                builder.Services.AddSingleton<RepositoryRooms>();
            }
           
            builder.Services.AddHostedService<HostBotBase>();
            builder.Services.AddSingleton<DiscordClient>();
            builder.Services.AddSingleton(new DiscordConfiguration
            {
                AutoReconnect = true,
                Token = _botConfig.TOKEN,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });

            if (_botConfig.AUDIO_SERVICES != true)
            {
                builder.Services.AddLavalink();
                builder.Services.ConfigureLavalink(options =>
                {
                    options.Passphrase = _botConfig.LAVALINK_PASSWORD;
                    options.BaseAddress = new Uri(_botConfig.LAVALINK_ADDRESS);
                    options.WebSocketUri = new Uri(_botConfig.LAVALINK_WEBSOCKET);
                    options.ReadyTimeout = TimeSpan.FromSeconds(10);
                });
            }
            
            builder.Services.AddSingleton<PaginationService>();
            builder.Services.AddSingleton<RoleSelectorsHandler>();


            builder.Services.AddLogging(s => s.AddConsole()
            #if DEBUG
            .SetMinimumLevel(LogLevel.Trace)
            #else            
            .SetMinimumLevel(LogLevel.Information)
            #endif
            );
            

            var host = builder.Build();
            AppDomain.CurrentDomain.ProcessExit += (object? _, EventArgs _) => { host.Dispose(); };
            host.Run();
        }
    }
}
