using Godot;
using System;
using Godot.Collections;
using MonoCustomResourceRegistry;

[RegisteredType(nameof(GenericSpriteCollection), "", nameof(Resource))]
public partial class GenericSpriteCollection : Resource
{
    [Export] public Array<Texture2D> Sprites { get; private set; }
}
