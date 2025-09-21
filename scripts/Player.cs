using Godot;
using System;

public partial class Player : Node2D
{
    [Signal]
    public delegate void AddKillEventHandler();

    public Area2D Collision { get; private set; }
    public int Kills { get; private set; } = 0;

    public const double ShootCooldown = 0.8 * Manager.SecondsPerBeat;

    private PackedScene eggProjectile = ResourceLoader.Load<PackedScene>("scenes/egg-projectile.tscn");
    private AnimatedSprite2D animatedSprite;
    private GameUI gameUI;
    private bool canShoot = true;
    private Timer cooldownTimer;
    private Random random = new Random();

    public override void _Ready()
    {
        base._Ready();
        Collision = GetNode<Area2D>("Collision");
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite.Play("idle");
        AddKill += HandleAddKill;

        cooldownTimer = new Timer();
        cooldownTimer.OneShot = true;
        cooldownTimer.Timeout += HandleCooldown;
        AddChild(cooldownTimer);
    }

    private void HandleCooldown()
    {
        canShoot = true;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left && canShoot)
            {
                EggProjectile eggProjectileInstance = eggProjectile.Instantiate<EggProjectile>();
                eggProjectileInstance.Target = GetGlobalMousePosition();
                GetParent().AddChild(eggProjectileInstance);

                int launchAudioIndex = random.Next(3);
                Manager.Instance.EffectPlayer.Stream = Manager.Instance.LaunchAudios[launchAudioIndex];
                Manager.Instance.EffectPlayer.Play();

                canShoot = false;
                cooldownTimer.Start(ShootCooldown);
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
        Manager.Instance.SwitchScene(Manager.Scene.GameOver);
    }
}
