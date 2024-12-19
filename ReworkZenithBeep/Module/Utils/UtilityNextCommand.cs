

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.Utils
{
    public class UtilityNextCommand : BaseCommandModule
    {
        private readonly UtilityCommand _Uticommand;
        public UtilityNextCommand()
        {
            _Uticommand = UtilityCommand.GetInstance();
        }

        [Command("bugreport")]
        public async Task TestAsyncCommand(CommandContext context)
        {
            await UtilityCommand.BugReportCommmand(new NextCommand(context));
        }

      
    }
}
