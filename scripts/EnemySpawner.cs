using Godot;
using System;

public partial class EnemySpawner : Node2D
{
    [Export]
    public float MaxInterval = 5.0f;

    [Export]
    public Vector2 Direction = Vector2.Down;

    [Export]
    public bool On = false;

    private Timer timer;
    private Random random;
    private PackedScene enemy = ResourceLoader.Load<PackedScene>("scenes/enemy.tscn");

    public override void _Ready()
    {
        base._Ready();
        timer = new Timer();
        timer.OneShot = true;
        timer.Timeout += HandleTimeout;
        AddChild(timer);
        random = new Random();

        if (On)
            CallDeferred(nameof(HandleTimeout));
    }

    private void HandleTimeout()
    {
        Enemy enemyInstance = enemy.Instantiate<Enemy>();
        enemyInstance.Position = Position;
        enemyInstance.SpawnDirection = Direction;
        GetParent().AddChild(enemyInstance);
        double interval = random.NextDouble() * MaxInterval;

        timer.Start(interval);
    }
}
