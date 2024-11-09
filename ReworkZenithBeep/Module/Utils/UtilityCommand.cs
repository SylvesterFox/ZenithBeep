using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ReworkZenithBeep.Settings;


namespace ReworkZenithBeep.Module.Utils
{
    public partial class UtilityCommand
    {
        
        private static UtilityCommand instance;

        public static UtilityCommand GetInstance()
        {
            if (instance == null)
            {
                instance = new UtilityCommand();
            }
            return instance;
        }

        public static async Task TestCommand(CommonContext ctx)
        {
            await ctx.DeferAsync();
            await ctx.RespondTextAsync("Test");
            var message = await ctx.GetOriginalResponseAsync();
            await Task.Delay(1000);
            await message.DeleteAsync();
           
        }

        public static async Task PingCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder
            {
                Content = "Beep!"
            });
            await Task.Delay(6000);
            var message = ctx.GetOriginalResponseAsync();
            await message.Result.DeleteAsync();
        }


    }
}
