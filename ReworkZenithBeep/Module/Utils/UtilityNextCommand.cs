

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

        [Command("test")]
        public async Task TestAsyncCommand(CommandContext context)
        {
            await UtilityCommand.TestCommand(new NextCommand(context));
        }

      
    }
}
