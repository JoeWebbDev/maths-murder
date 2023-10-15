using Godot;
using System;
using System.Linq;
using Godot.Collections;

/// <summary>
/// We can pass in an "AudioStreamOptions" object for more control over what the underlying AudioStreamPlayer does. For now,
/// we're only passing in volume, YAGNI and all that
/// </summary>
public partial class GlobalAudioManager : Node
{
    [ExportGroup("Internal properties")]
    [Export] private AudioStreamPlayer _musicPlayer;
    [Export] private Array<AudioStreamPlayer> _sfxPlayerPool;

    public void PlayMusic(AudioStream track, float db = 0f) => Play(track, _musicPlayer, db);

    public void PlaySfx(AudioStream sfx, float db = 0f) =>
        Play(sfx, _sfxPlayerPool.FirstOrDefault(asp => !asp.Playing), db);

    private void Play(AudioStream audioStream, AudioStreamPlayer player, float db)
    {
        if (player == null) return;
        player.Stream = audioStream;
        player.VolumeDb = db;
        player.Play();
    }
}
