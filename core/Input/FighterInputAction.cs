using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.Input;

public class FighterInputAction
{
    private StringName _actionName;
    private FighterCommand _onPressCmd;
    private FighterCommand _onReleaseCmd;
    
    public FighterInputAction(string actionName, FighterCommand onPressCmd, FighterCommand? onReleaseCmd)
    {
        _actionName = new StringName(actionName);
        _onPressCmd = onPressCmd;
        _onReleaseCmd = onReleaseCmd;
    }

    public FighterCommand GetCommand(InputEvent @event)
    {
        if (@event.IsActionPressed(_actionName)) return _onPressCmd;
        return @event.IsActionReleased(_actionName) ? _onReleaseCmd : null;
    }
}