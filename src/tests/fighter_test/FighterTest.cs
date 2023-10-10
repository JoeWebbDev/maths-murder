using Godot;
using System;
using System.Text;

public partial class FighterTest : Node
{
	[Export] private FighterData _fighterData;
	[Export] private Fighter _fighter;
	[Export] private SpinBox _legsYSpinBox;
	private Node2D _armsGroup;
	private Node2D _legsGroup;
	private Node2D _spriteGroup;
	private Vector2 _punchColliderObjectOriginalPosition;
	private Vector2 _bodyColliderOriginalPosition;
	private Vector2 _armsGroupOriginalPosition;
	private Vector2 _legsGroupOriginalPosition;
	private float _legsOriginalGlobalYPosition;
	private Vector2 _bodyOriginalPosition;
	private Vector2 _numRefOriginalPosition;
	private Vector2 _spriteGroupOriginalPosition;
	private Vector2 _legsOffset;
	private Vector2 _armsOffset;
	private Vector2 _bodyOffset;
	private Vector2 _spriteOffset;

	public override void _Ready()
	{
		
		_fighter.InitFighter(_fighterData);
		_spriteGroup = _fighter.GetNode<Node2D>("ScalableChildren/SpriteGroup");
		_armsGroup = _fighter.GetNode<Node2D>("ScalableChildren/ArmsGroup");
		_legsGroup = _fighter.GetNode<Node2D>("ScalableChildren/LegsGroup");
		_armsGroupOriginalPosition = _armsGroup.Position;
		_legsGroupOriginalPosition = _legsGroup.Position;
		_legsOriginalGlobalYPosition = _legsGroup.GlobalPosition.Y;
		_bodyOriginalPosition = _fighter.Position;
		_spriteGroupOriginalPosition = _spriteGroup.Position;
	}

	private void LogFighterPositions()
	{
		var sb = new StringBuilder();
		sb.AppendLine("legsGroup position: " + _legsGroup.GlobalPosition);
		sb.AppendLine("armsGroup position: " + _armsGroup.GlobalPosition);
		sb.AppendLine("body position: " + _fighter.GlobalPosition);
		sb.AppendLine("spriteGroup position: " + _spriteGroup.GlobalPosition);
		GodotLogger.LogDebug(sb.ToString());
	}

	public void OverrideSpritePositions()
	{
		_armsGroup.Position = _armsGroupOriginalPosition + _armsOffset;
		_legsGroup.Position = _legsGroupOriginalPosition + _legsOffset;
		_fighter.Position = _bodyOriginalPosition + _bodyOffset;
		_spriteGroup.Position = _spriteGroupOriginalPosition + _spriteOffset;
	}

	public void OnLegsXValueChanged(float value)
	{
		_legsOffset = new Vector2(value, _legsOffset.Y);
		OverrideSpritePositions();
	}
	
	public void OnArmsXValueChanged(float value)
	{
		_armsOffset = new Vector2(value, _armsOffset.Y);
		OverrideSpritePositions();
	}
	
	public void OnArmsYValueChanged(float value)
	{
		_armsOffset = new Vector2(_armsOffset.X, value);
		OverrideSpritePositions();
	}
	
	public void OnBodyXValueChanged(float value)
	{
		_bodyOffset = new Vector2(value, _bodyOffset.Y);
		OverrideSpritePositions();
	}
	
	public void OnBodyYValueChanged(float value)
	{
		_bodyOffset = new Vector2(_bodyOffset.X,value);
		// Keeping feet on the floor by clamping Y pos of legs when body Y pos changes
		_legsOffset = new Vector2(_legsOffset.X, -value/2);
		_legsYSpinBox.SetValueNoSignal(_legsOffset.Y);
		OverrideSpritePositions();
	}
	
	public void OnSpriteXValueChanged(float value)
	{
		_spriteOffset = new Vector2(value, _spriteOffset.Y);
		OverrideSpritePositions();
	}
	
	public void OnSpriteYValueChanged(float value)
	{
		_spriteOffset = new Vector2(_spriteOffset.X, value);
		OverrideSpritePositions();
	}

	public void OnButtonPressed()
	{
		LogFighterPositions();
	}
}
