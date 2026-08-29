using System.Text.RegularExpressions;
using DisCatSharp;
using DisCatSharp.Entities;

namespace ZenithBeep.DisCatSharp;

/// <summary>Converts user input from a slash command into a Discord emoji.</summary>
internal static partial class EmojiParser
{
    // Discord copies custom emojis as <:name:123> or <a:name:123>.
    [GeneratedRegex("^<a?:[A-Za-z0-9_]+:(?<id>\\d+)>$")]
    private static partial Regex CustomEmojiPattern();

    public static bool TryParse(DiscordClient client, string input, out DiscordEmoji? emoji)
    {
        emoji = null;
        input = input.Trim();
        if (string.IsNullOrEmpty(input)) return false;

        var match = CustomEmojiPattern().Match(input);
        if (match.Success && ulong.TryParse(match.Groups["id"].Value, out var emojiId))
        {
            emoji = DiscordEmoji.FromGuildEmote(client, emojiId);
            return true;
        }

        try
        {
            // :name: is resolved from guild/application emojis; all other input is Unicode.
            emoji = input.StartsWith(':') && input.EndsWith(':')
                ? DiscordEmoji.FromName(client, input)
                : DiscordEmoji.FromUnicode(client, input);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
