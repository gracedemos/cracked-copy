using Godot;

public partial class EggProjectile : Node2D
{
    public Vector2 Target { get; set; }

    public const float MovementSpeed = 500.0f;
    public const int Damage = 100;

    private Area2D collision;

    public override void _Ready()
    {
        base._Ready();
        collision = GetNode<Area2D>("Collision");
        collision.AreaEntered += HandleAreaEntered;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        HandleMovement(delta);
    }

    private void HandleMovement(double delta)
    {
        Vector2 direction = (Target - Position);
        if (direction.Length() < 2.0f)
            QueueFree();
        direction = direction.Normalized();
        Position = Position + direction * MovementSpeed * (float)delta;
    }

    private void HandleAreaEntered(Area2D area)
    {
        Node areaParent = area.GetParent();
        if (areaParent is Enemy)
        {
            ((Enemy)areaParent).Damage(Damage);
            QueueFree();
        }
    }

}
