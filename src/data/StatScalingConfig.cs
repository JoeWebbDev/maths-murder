using Godot;
using System;

public partial class StatScalingConfig : Resource
{
    [Export] public float HealthModifier;
    [Export] public float DamageModifier;
    [Export] public float DamageReductionModifier;
    [Export] public float SpeedModifier;
    [Export] public float DashMultiplier;
    [Export] public float StaminaRechargeSpeedModifier;
    [Export] public float ChipDamageModifier;
    
    public StatScalingConfig() : this(0, 0, 0, 0, 0, null) { }

    public StatScalingConfig(float healthModifier, float damageModifier, float damageReductionModifier, float speedModifier, float dashMultiplier, Godot.Collections.Array levelThresholds)
    {
        HealthModifier = healthModifier;
        DamageModifier = damageModifier;
        DamageReductionModifier = damageReductionModifier;
        SpeedModifier = speedModifier;
        DashMultiplier = dashMultiplier;
    }
}
