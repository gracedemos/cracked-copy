using Godot;

public partial class GameUI : Node
{
    private Player player;
    private Label killLabel;

    public override void _Ready()
    {
        base._Ready();
        player = GetTree().Root.GetNode<Player>("Game/Entities/Player");
        killLabel = GetNode<Label>("KillLabel");

        player.AddKill += HandleAddKill;
    }

    private void HandleAddKill()
    {
        killLabel.Text = $"KILLS: {player.Kills}";
    }
}
