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
        public static BotConfig CONFIG = SettingsManager.LoadFromEnv<BotConfig>();
        static void Main(string[] args)
        {
            var dataConfig = DataConfig.InitDataConfig();


            if (dataConfig == null)
            {
                Console.WriteLine("Data configuration is not loaded");
                return;
            }

            if (string.IsNullOrEmpty(CONFIG.TOKEN))
            {
                Console.WriteLine("Bot token is not provided");
                return;
            }

            var builder = new HostApplicationBuilder();
            
            if (CONFIG.NODB_MODE != true) {
                
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
                Token = CONFIG.TOKEN,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });

            if (CONFIG.AUDIO_SERVICES)
            {
                builder.Services.AddLavalink();
                builder.Services.ConfigureLavalink(options =>
                {
                    options.Passphrase = CONFIG.LAVALINK_PASSWORD;
                    options.BaseAddress = new Uri(CONFIG.LAVALINK_ADDRESS);
                    options.WebSocketUri = new Uri(CONFIG.LAVALINK_WEBSOCKET);
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
