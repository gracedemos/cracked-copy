using Godot;
using System;

public partial class EnemySpawner : Node2D
{
    [Export]
    public Vector2 Direction = Vector2.Down;

    private PackedScene enemy = ResourceLoader.Load<PackedScene>("scenes/enemy.tscn");
    private PackedScene enemy1 = ResourceLoader.Load<PackedScene>("scenes/enemy-1.tscn");
    private PackedScene enemy2 = ResourceLoader.Load<PackedScene>("scenes/enemy-2.tscn");
    private Random random = new Random();

    public void Spawn(int enemyIndex)
    {
        Enemy enemyInstance = null;
        switch (enemyIndex)
        {
            case 0:
                enemyInstance = enemy.Instantiate<Enemy>();
                break;
            case 1:
                enemyInstance = enemy1.Instantiate<Enemy>();
                break;
            case 2:
                enemyInstance = enemy2.Instantiate<Enemy>();
                break;
            default:
                break;
        }
        enemyInstance.Position = Position;
        enemyInstance.SpawnDirection = Direction;
        GetParent().AddChild(enemyInstance);
    }

}
