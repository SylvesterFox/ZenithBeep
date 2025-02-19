
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.Utils
{
    public class UtilityNextCommand : BaseCommandModule
    {
        
        [Command("bugreport")]
        public async Task TestAsyncCommand(CommandContext context)
        {
            await UtilityCommand.BugReportCommmand(new NextCommand(context));
        }

      
    }
}
