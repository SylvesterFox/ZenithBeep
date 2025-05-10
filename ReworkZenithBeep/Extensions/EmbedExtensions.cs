

using DSharpPlus.Entities;

namespace ReworkZenithBeep.Extensions
{
    public static class EmbedExtensions
    {
        public static IEnumerable<DiscordEmbedBuilder> GetRolesPage(this Dictionary<ulong, string> roles)
        {
            const int MaxFields = 25;
            const int MaxTotalLength = 6000;
            const int MaxFieldNameLength = 256;
            const int MaxFieldValueLength = 1024;

            List<DiscordEmbedBuilder> pages = new List<DiscordEmbedBuilder>();

            // Проверка на пустой словарь
            if (roles == null || roles.Count == 0)
            {
                // Возвращаем хотя бы один пустой эмбед, чтобы избежать проблем с PaginationMessage
                pages.Add(new DiscordEmbedBuilder()
                    .AddField("No roles found", "There are no role selectors configured for this server or message."));
                return pages;
            }

            DiscordEmbedBuilder currentEmbed = new DiscordEmbedBuilder();
            int currentFieldCount = 0;
            int currentTotalLength = 0;

            foreach (var role in roles)
            {
                string fieldName = $"Message ID: {role.Key}";
                string fieldValue = role.Value;

                // Проверка превышения длины по Discord лимитам
                if (fieldName.Length > MaxFieldNameLength)
                {
                    fieldName = fieldName.Substring(0, MaxFieldNameLength - 6) + "...";
                }

                if (fieldValue.Length > MaxFieldValueLength)
                {
                    // Разбиваем длинное значение на части
                    int chunkSize = MaxFieldValueLength - 6; // Оставляем место для "..."
                    for (int i = 0; i < fieldValue.Length; i += chunkSize)
                    {
                        string chunk = fieldValue.Substring(i, Math.Min(chunkSize, fieldValue.Length - i));

                        // Если это не первый чанк, добавляем многоточие в начало
                        if (i > 0)
                        {
                            chunk = "...\n" + chunk;
                        }

                        // Если это не последний чанк, добавляем многоточие в конец
                        if (i + chunkSize < fieldValue.Length)
                        {
                            chunk += "...";
                        }

                        // Создаем новую страницу для каждого чанка
                        if (currentFieldCount > 0 || i > 0)
                        {
                            pages.Add(currentEmbed);
                            currentEmbed = new DiscordEmbedBuilder();
                            currentFieldCount = 0;
                            currentTotalLength = 0;
                        }

                        currentEmbed.AddField(fieldName + (i > 0 ? " (continued)" : ""), chunk, false);
                        currentFieldCount++;
                        currentTotalLength += fieldName.Length + chunk.Length;

                        // Добавляем страницу сразу, чтобы не потерять последний чанк
                        if (i + chunkSize >= fieldValue.Length)
                        {
                            pages.Add(currentEmbed);
                            currentEmbed = new DiscordEmbedBuilder();
                            currentFieldCount = 0;
                            currentTotalLength = 0;
                        }
                    }

                    continue;
                }

                int estimatedFieldLength = fieldName.Length + fieldValue.Length;

                // Создаём новую страницу, если достигнут лимит
                if (currentFieldCount >= MaxFields || currentTotalLength + estimatedFieldLength > MaxTotalLength)
                {
                    pages.Add(currentEmbed);
                    currentEmbed = new DiscordEmbedBuilder();
                    currentFieldCount = 0;
                    currentTotalLength = 0;
                }

                currentEmbed.AddField(fieldName, fieldValue, false);
                currentFieldCount++;
                currentTotalLength += estimatedFieldLength;
            }

            // Добавляем последнюю страницу, если она не пуста
            if (currentFieldCount > 0)
            {
                pages.Add(currentEmbed);
            }

            // Если после всех операций список страниц пуст, добавляем хотя бы одну страницу
            if (pages.Count == 0)
            {
                pages.Add(new DiscordEmbedBuilder()
                    .AddField("No roles found", "There are no role selectors configured for this server or message."));
            }

            return pages;
        }

        public static List<DiscordEmbedBuilder> PageFildRoleEmbed(Dictionary<ulong, string> itemRole)
        {
            List<DiscordEmbedBuilder> embedBuilders = GetRolesPage(itemRole).ToList();
            return embedBuilders;
        }
    }
}
