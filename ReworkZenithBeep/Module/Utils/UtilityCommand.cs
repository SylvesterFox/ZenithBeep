using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ReworkZenithBeep.MessageEmbeds;
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

        public static async Task BugReportCommmand(CommonContext ctx)
        {
            await ctx.DeferAsync();
            var embedSuccess = new EmbedTempalte.DetailedEmbedContent
            {
                Color = new DiscordColor("#fd5531"),
                Description = $"You can describe all problems and bugs noticed in the bot’s operation on the bot hub guide page [Issues link](https://github.com/SylvesterFox/ZenithBeep/issues)",
                Title = "Bug Report"
            };
            var embed = EmbedTempalte.DetaliedEmbed(embedSuccess);
            await ctx.RespondEmbedAsync(embed);
        }


    }
}
