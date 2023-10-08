using Godot;
using System;
using System.Threading.Tasks;

public partial class Bracket : Node
{
    [Signal] public delegate void BracketAnimationsCompleteEventHandler();
    [ExportGroup("Bracket graphics properties")]
    [Export] private PackedScene _bracketFighterBoxScene;
    [Export] private Vector2 _ladderSpawnStartPosition;
    [Export] private Vector2 _distanceBetweenBoxes;
    [Export] private Vector2 _distanceBetweenPlayerBoxAndLadderBoxes;
    [Export] private Texture2D _defaultUpcomingFighterTexture;
    [Export] private Texture2D _lineTexture;
    [Export] private Texture2D _defeatedOpponentsFillTexture;
    [Export] private float _lineWidth;
    [Export] private Node _lineParent;
    [ExportGroup("Camera control properties")]
    [Export] private CinematicCamera _camera;

    [Export] private Vector2 _cameraStartPosition;
    [Export] private Vector2 _cameraStartZoom;
    [Export] private Vector2 _cameraFocusOnPlayerZoom;
    [Export] private Vector2 _cameraOffsetFromPlayer;
    [Export] private int _cameraInitialTweenDelayInMs;

    private PlayerData _playerData;
    private BracketFighterBox _playerBox;

    public override void _Ready()
    {
        _playerData = GetNode<GameDataManager>("/root/GameDataManager").GetPlayerData();
        CreateBracketGraphics();
        _camera.Init(_cameraStartPosition, _cameraStartZoom);
    }

    public async void PlayCameraTweens()
    {
        GodotLogger.LogDebug("Playing camera tweens");
        await Task.Delay(_cameraInitialTweenDelayInMs);
        var positionTask = _camera.SendToPosition(_playerBox.GlobalPosition + _cameraOffsetFromPlayer, 1);
        var zoomTask = _camera.SendToZoomValue(_cameraFocusOnPlayerZoom, 1);
        await Task.WhenAll(positionTask, zoomTask);
        await Task.Delay(500);
        var positionToMovePlayerBoxTo = _playerBox.GlobalPosition + _distanceBetweenBoxes;
        var tween = GetTree().CreateTween();
        tween.TweenProperty(_playerBox, "global_position", positionToMovePlayerBoxTo, 1)
            .SetTrans(Tween.TransitionType.Linear);
        positionTask = _camera.SendToPosition(positionToMovePlayerBoxTo + _cameraOffsetFromPlayer, 1);
        await ToSignal(tween, "finished");
        await positionTask;
        await Task.Delay(500);
        EmitSignal(SignalName.BracketAnimationsComplete);
    }

    private void CreateBracketGraphics()
    {
        var previousBracketFighterBoxPos = Vector2.Inf;
        for (var i = 0; i < _playerData.TotalNumberOfFightsPerGame; i++)
        {
            // Spawn next fighter box
            var upcomingBracketFighterBox = _bracketFighterBoxScene.Instantiate<BracketFighterBox>();
            upcomingBracketFighterBox.GlobalPosition = _ladderSpawnStartPosition + (_distanceBetweenBoxes * i);
            upcomingBracketFighterBox.FighterSprite.Texture = GetEnemyFighterSpriteForFighterBox(i);
            if (i < _playerData.DefeatedOpponents.Count)
            {
                upcomingBracketFighterBox.Fill.Texture = _defeatedOpponentsFillTexture;
            }
            AddChild(upcomingBracketFighterBox);
            
            // If this isn't the first box, spawn connecting line
            // Using Vector2.Inf > null to reduce complexity converting between nullable/non-nullable Vector2
            if (previousBracketFighterBoxPos != Vector2.Inf)
            {
                var line = new Line2D();
                if (_lineTexture != null)
                {
                    line.Texture = _lineTexture;
                    line.TextureMode = Line2D.LineTextureMode.Tile;
                    line.TextureRepeat = CanvasItem.TextureRepeatEnum.Enabled;
                }
                line.Points = new[]
                {
                    previousBracketFighterBoxPos,
                    upcomingBracketFighterBox.GlobalPosition
                };
                line.Width = _lineWidth;
                _lineParent.AddChild(line);
            }

            previousBracketFighterBoxPos = upcomingBracketFighterBox.GlobalPosition;
        }

        _playerBox = _bracketFighterBoxScene.Instantiate<BracketFighterBox>();
        var pos = _ladderSpawnStartPosition + (_distanceBetweenBoxes * _playerData.DefeatedOpponents.Count) - _distanceBetweenBoxes + _distanceBetweenPlayerBoxAndLadderBoxes;
        _playerBox.GlobalPosition = pos;
        _playerBox.FighterSprite.Texture = _playerData.FighterData.NumberTexture;
        AddChild(_playerBox);
    }

    // If the box being drawn is the next fight, we return the current opponent's sprite
    // If it's the final box, we return 9 (the boss)
    // Otherwise, we return the default 
    private Texture2D GetEnemyFighterSpriteForFighterBox(int boxIndex)
    {
        if (boxIndex < _playerData.DefeatedOpponents.Count)
        {
            
            return _playerData.DefeatedOpponents[boxIndex].NumberTexture;
        }
        
        if (boxIndex == _playerData.DefeatedOpponents.Count)
        {
            return _playerData.CurrentOpponent.NumberTexture;
        }

        if (boxIndex == _playerData.TotalNumberOfFightsPerGame - 1)
        {
            // return finalBossNumberTexture;
        }

        return _defaultUpcomingFighterTexture;
    }
}
