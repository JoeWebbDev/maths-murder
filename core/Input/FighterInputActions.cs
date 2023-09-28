using System.Linq;
using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.Input;

// Originally did this to restrict references to magic strings. Since evolved as a way to cut down on all the "if this input then do this" code
public static class FighterInputActions
{
    private static FighterInputAction[] _combatActions = new[]
    {
        new FighterInputAction("punch", new PunchCommand(), null),
        new FighterInputAction("block", new BlockCommand(), new BlockCommand(true)),
    };

    public static FighterCommand TryGetCommand(InputEvent @event)
    {
        return _combatActions.Select(fighterInputAction => fighterInputAction.GetCommand(@event)).FirstOrDefault(cmd => cmd != null);
    }
}