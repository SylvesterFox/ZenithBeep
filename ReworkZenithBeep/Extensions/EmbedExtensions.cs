

using DSharpPlus.Entities;
using System.Text;


namespace ReworkZenithBeep.Extensions
{
    public static class EmbedExtensions
    {
       
        public static List<DiscordEmbedBuilder> PageFildRoleEmbed(Dictionary<ulong, string> itemRole, ulong guildID, ulong channelId)
        {
            List<DiscordEmbedBuilder> embedBuilders = GetRolesPage(itemRole, 15, guildID, channelId).ToList();
            return embedBuilders;
        }

        private static List<DiscordEmbedBuilder> GetRolesPage(Dictionary<ulong, string> itemRole, int itemsPerPage, ulong guildId, ulong channelId)
        {
            List<DiscordEmbedBuilder> pages = new List<DiscordEmbedBuilder>();
            DiscordEmbedBuilder currentEmbed = new DiscordEmbedBuilder();
            var descriptionBuilder = new StringBuilder();
            int lineCount = 0;

            foreach (var role in itemRole)
            {
                var messageId = role.Key;
                var messageLink = $"https://discord.com/channels/{guildId}/{channelId}/{messageId}";

                // Разделим role.Value по строкам
                var lines = role.Value.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    descriptionBuilder.AppendLine($"[Message ID: {messageId}]({messageLink})\n{line}");
                    lineCount++;

                    if (lineCount == itemsPerPage)
                    {
                        var embed = new DiscordEmbedBuilder()
                            .WithDescription(descriptionBuilder.ToString());

                        pages.Add(embed);
                        descriptionBuilder.Clear();
                        lineCount = 0;
                    }
                }
            }

            // Добавляем остатки
            if (descriptionBuilder.Length > 0)
            {
                var embed = new DiscordEmbedBuilder()
                    .WithDescription(descriptionBuilder.ToString());

                pages.Add(embed);
            }

            return pages;
        }
    }
}
