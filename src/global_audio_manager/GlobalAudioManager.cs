using Godot;
using System;
using System.Linq;
using Godot.Collections;
using MathsMurderSpike.core.Audio;

/// <summary>
/// We can pass in an "AudioStreamOptions" object for more control over what the underlying AudioStreamPlayer does. For now,
/// we're only passing in volume, YAGNI and all that
/// </summary>
public partial class GlobalAudioManager : Node
{
    [ExportGroup("Internal properties")]
    [Export] private AudioStreamPlayer _musicPlayer;
    [Export] private Array<AudioStreamPlayer> _sfxPlayerPool;

    public override void _Ready()
    {
        _musicPlayer.Bus = AudioBusName.Music;
        foreach (var sfxPlayer in _sfxPlayerPool)
        {
            sfxPlayer.Bus = AudioBusName.Sfx;
        }
    }

    public AudioStreamPlayer PlayMusic(AudioStream track, float db = 0f) => Play(track, _musicPlayer, db);

    public AudioStreamPlayer PlaySfx(AudioStream sfx, float db = 0f) =>
        Play(sfx, _sfxPlayerPool.FirstOrDefault(asp => !asp.Playing), db);

    private AudioStreamPlayer Play(AudioStream audioStream, AudioStreamPlayer player, float db)
    {
        if (player == null) return null;
        player.Stream = audioStream;
        player.VolumeDb = db;
        player.Play();
        return player;
    }
}
