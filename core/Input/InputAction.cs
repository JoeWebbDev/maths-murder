namespace MathsMurderSpike.Core.Input;

public static class InputAction
{
    public const string MoveLeft = "p_move_left";
    public const string MoveRight = "p_move_right";
    public const string Punch = "p_punch";
    public const string Block = "p_block";
    public const string Duck = "p_duck";
    public const string Kick = "p_kick";
    public const string AllAsHintString = $"{MoveLeft},{MoveRight},{Punch},{Block},{Duck},{Kick}";
}