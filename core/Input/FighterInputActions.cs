using System.Linq;
using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.Input;

// Originally did this to restrict references to magic strings. Since evolved as a way to cut down on all the "if this input then do this" code
public static class FighterInputActions
{
    private static FighterInputAction[] _actions = new[]
    {
        new FighterInputAction("punch", new PunchCommand(), null),
        new FighterInputAction("block", new BlockCommand(), new BlockCommand(true)),
        new FighterInputAction("move_left", new WalkCommand(Vector2.Left), new WalkCommand(Vector2.Left, true)),
        new FighterInputAction("move_right", new WalkCommand(Vector2.Right), new WalkCommand(Vector2.Right, true)),
    };

    public static FighterCommand TryGetCommand(InputEvent @event)
    {
        return _actions.Select(fighterInputAction => fighterInputAction.GetCommand(@event)).FirstOrDefault(cmd => cmd != null);
    }

    public static WalkCommand TryGetWalkCommand()
    {
        // Will replace with better references than an array index if we decide we must do it this way
        var (leftCommand, rightCommand) = (_actions[2].GetCommandDirectFromInput() as WalkCommand, _actions[3].GetCommandDirectFromInput() as WalkCommand);
        return leftCommand ?? rightCommand;
    }
}