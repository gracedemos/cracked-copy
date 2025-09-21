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

    [Export]
    public string StateTriggerNode = "Game/Triggers/EnemyStateTrigger";

    public EnemyState State { get; private set; } = EnemyState.PresetPath;
    public Vector2 SpawnDirection { get; set; } = Vector2.Down;
    public bool Active { get; private set; } = true;

    private Area2D collision;
    private Area2D stateTrigger;
    private AnimatedSprite2D animatedSprite;
    private Player player;

    public override void _Ready()
    {
        base._Ready();
        collision = GetNode<Area2D>("Collision");
        stateTrigger = GetTree().Root.GetNode<Area2D>(StateTriggerNode);
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
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
        if (!Active)
            return;
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

    public void Damage(int amount)
    {
        Health = Health - amount;
        if (Health <= 0 && Active)
        {
            Active = false;
            player.EmitSignal(Player.SignalName.AddKill);
            animatedSprite.AnimationFinished += HandleDeathAnimationFinished;
            animatedSprite.Play("death");
            Manager.Instance.EffectPlayer.Stream = Manager.Instance.AudioKillEnemy;
            Manager.Instance.EffectPlayer.Play();
        }
        else
        {
            Manager.Instance.EffectPlayer.Stream = Manager.Instance.AudioDamageEnemy;
            Manager.Instance.EffectPlayer.Play();
        }
    }

    private void HandleDeathAnimationFinished()
    {
        QueueFree();
    }
}
