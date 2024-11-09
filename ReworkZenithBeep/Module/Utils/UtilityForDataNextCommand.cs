
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ReworkZenithBeep.Data;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.Utils
{
    public class UtilityForDataNextCommand : BaseCommandModule
    {
        private readonly DataUtilityCommands _dataUtilityCommands;

        public UtilityForDataNextCommand(DataBot dataBot)
        {
            _dataUtilityCommands = new DataUtilityCommands(dataBot);
        }

        [Command("prefix")]
        public async Task PrefixAsyncCommand(CommandContext context, string? prefix = null)
        {
            await _dataUtilityCommands.PrefixCommand(new NextCommand(context), prefix);
        }
    }
}
