using DSharpPlus.Entities;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Protocol.Payloads.Events;
using Lavalink4NET.Tracks;
using ReworkZenithBeep.Settings;


namespace ReworkZenithBeep.Player
{
    public sealed record class ZenithPlayerOptions : VoteLavalinkPlayerOptions
    {
        public CommonContext Context { get; init; }
    }

    public class ZenithPlayer : VoteLavalinkPlayer
    {
        public DiscordChannel Channel => _channel;


        private readonly DiscordChannel _channel;
        private DiscordMessage? message;
        private System.Timers.Timer? _progressTimer;

        private ZenithPlayer(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> properties, CancellationToken cancellation = default) : base(properties)
        {
            _channel = properties.Options.Value.Context.Member.VoiceState.Channel;
        }

        public static ValueTask<ZenithPlayer> CreatePlayerAsync(IPlayerProperties<ZenithPlayer, ZenithPlayerOptions> properties, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(nameof(properties));

            return ValueTask.FromResult(new ZenithPlayer(properties));
        }

        protected override async ValueTask NotifyTrackStartedAsync(ITrackQueueItem track, CancellationToken cancellationToken = default)
        {
            await base
                .NotifyTrackStartedAsync(track, cancellationToken)
                .ConfigureAwait(false);

            var embedPlaying = EmbedsPlayer.NowPlayingEmbed(track?.Track, TimeSpan.Zero);
            if (message != null) 
            {
                await message.ModifyAsync(embedPlaying);
            } else 
            {
                message = await _channel.SendMessageAsync(embedPlaying);
            }
            
            StartProgressTimer(track?.Track);
        }

        protected override async ValueTask NotifyTrackEndedAsync(ITrackQueueItem trackQueue, TrackEndReason reason, CancellationToken cancellationToken = default) {
            await base.NotifyTrackEndedAsync(trackQueue, reason);
        
            switch (reason) {
                case TrackEndReason.Finished:
                    if (Queue.IsEmpty && CurrentTrack == null)
                    {
                        if (message != null)
                        {
                            await message.DeleteAsync();
                            await DisconnectAsync();
                            return;
                        }

                        await DisconnectAsync();
                        return;
                    }
                break;
            }
        }

        private void StartProgressTimer(LavalinkTrack? track)
        {
            if (track == null) return;

            _progressTimer?.Dispose();
            _progressTimer = new System.Timers.Timer(1000);
            _progressTimer.Elapsed += async (sender, args) => await UpdateProgress(track);
            _progressTimer.Start();
        }

        private async Task UpdateProgress(LavalinkTrack track)
        {
            if (message == null || State != PlayerState.Playing)
            {
                _progressTimer?.Stop();
                return;
            }

            if (_channel == null) {
                return;
            }

            var position = Position.Value.Position; 
            var duration = track.Duration;

            var embedUpdated = EmbedsPlayer.NowPlayingEmbed(track, position);

            try
            {
                await message.ModifyAsync(embedUpdated);
            } catch (DSharpPlus.Exceptions.NotFoundException)
            {
                message = null;
            }
        }

        public async Task ControlPauseAsync()
        {
            if (IsPaused)
            {
                await ResumeAsync();
                return;
            }

            await PauseAsync();
            await SeekAsync(new TimeSpan(0, 0, -3), SeekOrigin.Current).ConfigureAwait(false);
        }
         
    }
}
