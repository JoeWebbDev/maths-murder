﻿using System.Threading.Tasks;
using Godot;
using MathsMurderSpike.core.Commands;

namespace MathsMurderSpike.Core.FighterStates;

// Going with abstract class > interface for now. We may want some common functionality between all states down the line!
public abstract class FighterState
{
    public virtual void Enter(Fighter fighter)
    {
        GodotLogger.LogDebug($"Fighter {fighter.PlayerNumber} entering {GetType().Name}");
    }
    public abstract bool HandleCommand(Fighter fighter, FighterCommand cmd);

    public virtual void Process(Fighter fighter, double delta) { }
    public virtual void OnHit(Fighter fighter, Fighter target) { }

    public async virtual Task Exit(Fighter fighter)
    {
        GodotLogger.LogDebug($"Fighter {fighter.PlayerNumber} exiting {GetType().Name}");
    }
}