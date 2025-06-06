﻿using System.Reflection;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ReworkZenithBeep.MessageEmbeds;
using ReworkZenithBeep.Settings;


namespace ReworkZenithBeep.Module.Utils
{
    public partial class UtilityCommand
    {  
        private static UtilityCommand instance;
        
        public static UtilityCommand GetInstance()
        {
            if (instance == null)
            {
                instance = new UtilityCommand();
            }
            return instance;
        }


        public static async Task PingCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder
            {
                Content = "Beep!"
            });
            await Task.Delay(6000);
            var message = ctx.GetOriginalResponseAsync();
            await message.Result.DeleteAsync();
            
        }

        public static async Task BugReportCommmand(CommonContext ctx)
        {
            await ctx.DeferAsync();
            var embedSuccess = new EmbedTempalte.DetailedEmbedContent
            {
                Color = new DiscordColor("#fd5531"),
                Description = $"You can describe all problems and bugs noticed in the bot’s operation on the bot github page [Issues link](https://github.com/SylvesterFox/ZenithBeep/issues)",
                Title = "Bug Report"
            };
            var embed = EmbedTempalte.DetaliedEmbed(embedSuccess);
            await ctx.RespondEmbedAsync(embed);
        }

        public static async Task HelpCommand(InteractionContext ctx) {

             await ctx.DeferAsync();

            var embedContent = new DiscordEmbedBuilder
            {
                Title = "Full list of slash commands",
                Description = "**List of commands available to you:**",
                Color = DiscordColor.Blurple
            };

            var commandModules = Assembly.GetExecutingAssembly()
                                     .GetTypes()
                                     .Where(t => t.IsSubclassOf(typeof(ApplicationCommandModule)))
                                     .ToList();

            foreach (var module in commandModules) {
                var contentCommand = module.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                    .Where(m => m.GetCustomAttributes(typeof(SlashCommandAttribute), false).Any())
                                    .ToList();

                foreach (var command in contentCommand) {
                    var commandAttr = (SlashCommandAttribute)command.GetCustomAttributes(typeof(SlashCommandAttribute), false).FirstOrDefault();
                    var descripton = commandAttr?.Description ?? "Нет описание";
                    var name = commandAttr?.Name;
                    embedContent.AddField($"{descripton}", $"`/{name}`", true);
                }
            }

            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedContent));
        }

        public static async Task CleanCommand(InteractionContext ctx, int count, DiscordUser member = null)
        {
            await ctx.DeferAsync(true);

            var embedSuccess = new EmbedTempalte.DetailedEmbedContent
            {
                Color = new DiscordColor("#fd5531"),
                Description = $"You can delete {count} messages",
                Title = "Clean Command"
            };
            var embed = EmbedTempalte.DetaliedEmbed(embedSuccess);

            IReadOnlyList<DiscordMessage> messageToDelete;

            if (member != null)
            {
                var userMessages = await ctx.Channel.GetMessagesAsync(250);
                messageToDelete = userMessages
                    .Where(m => m.Author.Id == member.Id)
                    .Take(count)
                    .ToList();
            }
            else
            {
                var allMessages = await ctx.Channel.GetMessagesAsync(count);
                messageToDelete = allMessages
                    .Take(count)
                    .ToList();
            }

            if (messageToDelete.Count > 0)
            {
                foreach (var message in messageToDelete)
                {
                    try
                    {
                        await message.DeleteAsync();
                        await Task.Delay(1000); // задержка, чтобы избежать rate-limit
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при удалении: {ex.Message}");
                    }

                }
            }

            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));

        }

    }
}
