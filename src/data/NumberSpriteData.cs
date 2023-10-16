using Godot;
using System;
using MonoCustomResourceRegistry;

[RegisteredType(nameof(NumberSpriteData), "", nameof(Resource))]
public partial class NumberSpriteData : Resource
{
    [Export] public Texture2D NumberTexture { get; set; }
    [Export] public Vector2 ArmsOffset;
    [Export] public Vector2 LegsOffset;
    [Export] public Vector2 BodyOffset;
    [Export] public Vector2 SpriteOffset;
    [Export] public AudioStream AnnouncerAudio;

    public NumberSpriteData() : this(null, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero) { }
        
    public NumberSpriteData(Texture2D numberTexture, Vector2 armsOffset, Vector2 legsOffset, Vector2 bodyOffset, Vector2 spriteOffset)
    {
        NumberTexture = numberTexture;
        ArmsOffset = armsOffset;
        LegsOffset = legsOffset;
        BodyOffset = bodyOffset;
        SpriteOffset = spriteOffset;
    }
}
