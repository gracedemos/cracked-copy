using Godot;

public partial class Player : Node2D
{
    public Area2D Collision { get; private set; }

    private PackedScene eggProjectile = ResourceLoader.Load<PackedScene>("scenes/egg-projectile.tscn");
    private AnimatedSprite2D animatedSprite;

    public override void _Ready()
    {
        base._Ready();
        Collision = GetNode<Area2D>("Collision");
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite.Play("idle");
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

    private void KillPlayerDeferred()
    {
        GetTree().ChangeSceneToFile("scenes/game-over.tscn");
    }
}
