
using DSharpPlus.EventArgs;
using DSharpPlus;

namespace ReworkZenithBeep.Handler
{
    public class VoiceRoomsHandler
    {
        public static async Task OnRoomStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            // Check if the user joined a voice channel
            if (e.Before?.Channel == null && e.After?.Channel != null)
            {
                Console.WriteLine($"{e.User.Username} joined the voice channel {e.After.Channel.Name}");
            }
            // Check if the user left a voice channel
            else if (e.Before?.Channel != null && e.After?.Channel == null)
            {
                Console.WriteLine($"{e.User.Username} left the voice channel {e.Before.Channel.Name}");
            }
            // Check if the user switched voice channels
            else if (e.Before?.Channel != null && e.After?.Channel != null && e.Before.Channel != e.After.Channel)
            {
                Console.WriteLine($"{e.User.Username} switched from {e.Before.Channel.Name} to {e.After.Channel.Name}");
            }

            await Task.CompletedTask;
        }
    }
}
