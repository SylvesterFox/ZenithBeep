﻿

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Lavalink4NET;
using Microsoft.Extensions.DependencyInjection;
using ReworkZenithBeep.Settings;

namespace ReworkZenithBeep.Module.Music
{
    
    public class MusicNextCommand : BaseCommandModule
    {
        private readonly MusicCommand musicCommand;

        public MusicNextCommand(IServiceProvider serviceProvider) {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            musicCommand = MusicCommand.GetInstance(serviceProvider.GetRequiredService<IAudioService>(), serviceProvider);
        }

        [Command("join")]
        public async Task JoinCommandAsync(CommandContext ctx)
        {
            await musicCommand.JoinAsync(new NextCommand(ctx));
        }

        [Command("leave")]
        public async Task LeaveCommandAsync(CommandContext ctx)
        {
            await musicCommand.LeaveAsync(new NextCommand(ctx));
        }

        [Command("play")]
        public async Task PlayCommandAsync(CommandContext ctx, string query)
        {
            await musicCommand.PlayAsync(new NextCommand(ctx), query);
        }

        [Command("skip")]
        public async Task SkipCommandAsync(CommandContext ctx, long count = 1)
        {
            await musicCommand.SkipAsync(new NextCommand(ctx), count);
        }

        [Command("queue")]
        public async Task QueueCommandAsync(CommandContext ctx)
        {
            await musicCommand.QueueAsync(new NextCommand(ctx));
        }

        [Command("remove")]
        public async Task RemoveCommandAsync(CommandContext ctx, long position)
        {
            await musicCommand.RemoveAsync(new NextCommand(ctx), position);
        }

        [Command("looptrack")]
        public async Task LoopCommandAsync(CommandContext ctx)
        {
            await musicCommand.LoopAsync(new NextCommand(ctx));
        }

        [Command("pause")]
        public async Task PauseCommandAsync(CommandContext ctx)
        {
            await musicCommand.PauseAsync(new NextCommand(ctx));
        }

        [Command("clear")]
        public async Task ClearCommandAsync(CommandContext ctx)
        {
            await musicCommand.ClearAsync(new NextCommand(ctx));
        }

        [Command("seek")]
        public async Task SeekCommandAsync(CommandContext ctx, string timeCode)
        {
            await musicCommand.SeekCommand(new NextCommand(ctx), timeCode);
        }

        [Command("move")]
        public async Task MoveCommandAsync(CommandContext ctx, int position)
        {
            await musicCommand.MoveTrackToTop(new NextCommand(ctx), position);
        }
    }
}
