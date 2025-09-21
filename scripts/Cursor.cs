using Godot;

public partial class Cursor : Node2D
{
    public override void _Process(double delta)
    {
        base._Process(delta);

        Position = GetGlobalMousePosition();
        Rotation = Mathf.Atan2(Position.Y, Position.X);
    }
}
