using ReworkZenithBeep.Data;
using ReworkZenithBeep.Settings;


namespace ReworkZenithBeep.Module.Utils
{
    public partial class DataUtilityCommands
    {
        private readonly DataBot _dbContext;
        private static DataUtilityCommands instance;

        public DataUtilityCommands(DataBot dataBot)
        {
            this._dbContext = dataBot;
        }

        public static DataUtilityCommands GetInstance(DataBot dataBot)
        {
            if (instance == null)
            {
                instance = new DataUtilityCommands(dataBot);
            }

            return instance;
        }

        public async Task PrefixCommand(CommonContext ctx, string? prefix = null)
        {
            await ctx.DeferAsync();
            var responsDb = await _dbContext.GetOrCreateGuild(ctx.Guild);
            if (prefix == null)
            {
                await ctx.RespondTextAsync($"The prefix of this guild is {responsDb.Prefix}");
                return;
            }

            await _dbContext.UpdatePrefix(ctx.Guild, prefix);
        }

    }
}
