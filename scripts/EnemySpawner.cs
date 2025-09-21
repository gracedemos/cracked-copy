using Godot;

public partial class EnemySpawner : Node2D
{
    [Export]
    public Vector2 Direction = Vector2.Down;

    private PackedScene enemy = ResourceLoader.Load<PackedScene>("scenes/enemy.tscn");

    public void Spawn()
    {
        Enemy enemyInstance = enemy.Instantiate<Enemy>();
        enemyInstance.Position = Position;
        enemyInstance.SpawnDirection = Direction;
        GetParent().AddChild(enemyInstance);
    }
}
