using Godot;

public partial class GameOver : Node
{
    private TextureButton restartButton;

    public override void _Ready()
    {
        base._Ready();
        restartButton = GetNode<TextureButton>("VBoxContainer/RestartButton");
        restartButton.Pressed += HandleRestartButtonPressed;
    }

    private void HandleRestartButtonPressed()
    {
        CallDeferred(nameof(ReloadGameScene));
    }

    private void ReloadGameScene()
    {
        GetTree().ChangeSceneToFile("scenes/game.tscn");
    }
}
