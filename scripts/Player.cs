using Godot;

public partial class Player : Node2D
{
    [Signal]
    public delegate void AddKillEventHandler();

    public Area2D Collision { get; private set; }
    public int Kills { get; private set; } = 0;

    private PackedScene eggProjectile = ResourceLoader.Load<PackedScene>("scenes/egg-projectile.tscn");
    private AnimatedSprite2D animatedSprite;
    private GameUI gameUI;

    public override void _Ready()
    {
        base._Ready();
        Collision = GetNode<Area2D>("Collision");
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite.Play("idle");

        AddKill += HandleAddKill;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                EggProjectile eggProjectileInstance = eggProjectile.Instantiate<EggProjectile>();
                eggProjectileInstance.Target = GetGlobalMousePosition();
                GetParent().AddChild(eggProjectileInstance);
            }
        }
    }

    public void KillPlayer()
    {
        CallDeferred(nameof(KillPlayerDeferred));
    }

    public void HandleAddKill()
    {
        Kills++;
    }

    private void KillPlayerDeferred()
    {
        GetTree().ChangeSceneToFile("scenes/game-over.tscn");
    }
}
