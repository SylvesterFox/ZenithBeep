﻿using DSharpPlus.SlashCommands;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Settings;


namespace ReworkZenithBeep.Module.Utils
{
    public class UtilitySlashCommand : ApplicationCommandModule
    {
        private readonly BotContext _botContext;
        public UtilitySlashCommand(BotContext context) 
        {
            _botContext = context;
        }


        [SlashCommand("beep", "ping command!")]
        public async Task PingAsync(InteractionContext ctx)
        {
            await UtilityCommand.PingCommand(ctx);
        }

        [SlashCommand("bugreport", "Bug report command")]
        public async Task TestAsync(InteractionContext ctx)
        {
            await UtilityCommand.BugReportCommmand(new SlashContext(ctx));
            
        }

        [SlashCommand("help", "Get list commands")]
        public async Task HelpAsync(InteractionContext ctx) {
           await UtilityCommand.HelpCommand(ctx);
        }

    }
}
