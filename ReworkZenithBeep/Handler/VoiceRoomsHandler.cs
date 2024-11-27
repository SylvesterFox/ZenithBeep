
using DSharpPlus.EventArgs;
using DSharpPlus;

namespace ReworkZenithBeep.Handler
{
    public class VoiceRoomsHandler
    {
        public static async Task voiceRoomsHandler(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            if (e.Before == null && e.After != null) // User joined a voice channel
            {
                Console.WriteLine($"{e.User.Username} joined voice channel {e.After.Channel.Name}");
            }
            else if (e.Before != null && e.After == null) // User left a voice channel
            {
                Console.WriteLine($"{e.User.Username} left voice channel {e.Before.Channel.Name}");
            }
            else if (e.Before?.Channel != e.After?.Channel) // User switched channels
            {
                Console.WriteLine($"{e.User.Username} moved from {e.Before?.Channel?.Name} to {e.After?.Channel?.Name}");
            }

            await Task.CompletedTask;
        }
    }
}
