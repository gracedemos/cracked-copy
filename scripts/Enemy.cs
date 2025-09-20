using Godot;

public partial class Enemy : Node2D
{
    public enum EnemyState
    {
        PresetPath,
        PlayerPath
    }

    [Export]
    public float MovementSpeed = 60.0f;

    [Export]
    public int Health = 100;

    public EnemyState State { get; private set; } = EnemyState.PresetPath;
    public Vector2 SpawnDirection { get; set; } = Vector2.Down;

    private Area2D collision;
    private Area2D stateTrigger;
    private Player player;

    public override void _Ready()
    {
        base._Ready();
        collision = GetNode<Area2D>("Collision");
        stateTrigger = GetTree().Root.GetNode<Area2D>("Game/Triggers/EnemyStateTrigger");
        player = GetParent().GetNode<Player>("Player");

        collision.AreaEntered += HandleAreaEntered;

        if (!SpawnDirection.IsNormalized())
        {
            GD.PrintErr("Preset direction is not normalized");
            GetTree().Quit(-1);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        HandleMovement(delta);
    }

    private void HandleMovement(double delta)
    {
        switch (State)
        {
            case EnemyState.PresetPath:
                PathToPreset(delta);
                break;
            case EnemyState.PlayerPath:
                PathToPlayer(delta);
                break;
        }
    }

    private void PathToPreset(double delta)
    {
        Position = Position + SpawnDirection * MovementSpeed * (float)delta;
    }

    private void PathToPlayer(double delta)
    {
        Vector2 direction = (player.Position - Position).Normalized();
        Position = Position + direction * MovementSpeed * (float)delta;
    }

    public void HandleAreaEntered(Area2D area)
    {
        if (area == player.Collision)
        {
            player.KillPlayer();
        }
        else if (area == stateTrigger)
        {
            State = EnemyState.PlayerPath;
        }
    }
}
