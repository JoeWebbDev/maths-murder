using Godot;
using System;
using System.Threading.Tasks;

public partial class FightVersus : Node2D
{
    [ExportGroup("Internal properties")]
    [Export] private Fighter _playerFighter;
    [Export] private Sprite2D _playerBackground;
    [Export] private Fighter _enemyFighter;
    [Export] private Sprite2D _enemyBackground;
    [Export] private AudioStream _versusAnnouncerAudio;

    private GlobalAudioManager _audioManager;

    public override void _Ready()
    {
        _audioManager = GetNode<GlobalAudioManager>("/root/GlobalAudioManager");
    }

    public async Task PlayPreFightVersusSequence(FighterData playerData, FighterData enemyData, Texture2D backgroundTexture)
    {
        var playerSpriteData = playerData.GetSpriteData();
        var enemySpriteData = enemyData.GetSpriteData();
        _playerFighter.InitFighter(playerData);
        _playerFighter.Position = new Vector2(_playerFighter.Position.X,
            _playerFighter.Position.Y - playerSpriteData.SpriteOffset.Y);
        _playerBackground.Texture = backgroundTexture;
        _enemyFighter.InitFighter(enemyData);
        _enemyFighter.Position = new Vector2(_enemyFighter.Position.X,
            _enemyFighter.Position.Y - enemySpriteData.SpriteOffset.Y);
        _enemyBackground.Texture = backgroundTexture;
        Show();
        var playerNumberAudio = playerSpriteData.AnnouncerAudio;
        var enemyNumberAudio = enemySpriteData.AnnouncerAudio;
        var audioPlayer = _audioManager.PlaySfx(playerNumberAudio);
        audioPlayer.Finished += () => { GodotLogger.LogDebug("Finished playing"); };
        await ToSignal(audioPlayer, "finished");
        audioPlayer = _audioManager.PlaySfx(_versusAnnouncerAudio);
        await ToSignal(audioPlayer, "finished");
        audioPlayer = _audioManager.PlaySfx(enemyNumberAudio);
        await ToSignal(audioPlayer, "finished");
        Hide();
    }
}
