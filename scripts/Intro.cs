using Godot;

public partial class Intro : Node
{
    public enum IntroState
    {
        Wait,
        CreditsFadeIn,
        Credits,
        CreditsFadeOut,
        MiddleWait,
        StoryFadeIn,
        Story,
        StoryFadeOut
    }

    private Sprite2D creditsSprite;
    private Sprite2D storySprite;
    private Sprite2D backgroundSprite;
    private Timer timer;
    private IntroState state = IntroState.Wait;

    public override void _Ready()
    {
        base._Ready();
        creditsSprite = GetNode<Sprite2D>("CreditsSprite");
        storySprite = GetNode<Sprite2D>("StorySprite");
        backgroundSprite = GetNode<Sprite2D>("BackgroundSprite");
        SetupSprites();

        timer = new Timer();
        timer.OneShot = true;
        timer.Timeout += HandleTimeout;
        AddChild(timer);
        HandleTimeout();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("Space"))
            HandleFinished();
    }

    private void SetupSprites()
    {
        Color creditsModulate = creditsSprite.Modulate;
        creditsModulate.A = 0.0f;
        creditsSprite.Modulate = creditsModulate;
        Color storyModulate = storySprite.Modulate;
        storyModulate.A = 0.0f;
        storySprite.Modulate = storyModulate;
    }

    private void HandleTimeout()
    {
        Tween tween;
        switch (state)
        {
            case IntroState.Wait:
                state = IntroState.CreditsFadeIn;
                timer.Start(1.0);
                break;
            case IntroState.CreditsFadeIn:
                state = IntroState.Credits;
                tween = GetTree().CreateTween();
                tween.Finished += HandleTimeout;
                tween.TweenProperty(creditsSprite, "modulate:a", 1.0f, 2.0f);
                break;
            case IntroState.Credits:
                state = IntroState.CreditsFadeOut;
                timer.Start(5.0);
                break;
            case IntroState.CreditsFadeOut:
                state = IntroState.MiddleWait;
                tween = GetTree().CreateTween();
                tween.Finished += HandleTimeout;
                tween.TweenProperty(creditsSprite, "modulate:a", 0.0f, 2.0f);
                break;
            case IntroState.MiddleWait:
                state = IntroState.StoryFadeIn;
                timer.Start(1.0);
                break;
            case IntroState.StoryFadeIn:
                state = IntroState.Story;
                tween = GetTree().CreateTween();
                tween.Finished += HandleTimeout;
                tween.TweenProperty(storySprite, "modulate:a", 1.0f, 2.0f);
                break;
            case IntroState.Story:
                state = IntroState.StoryFadeOut;
                timer.Start(10.0);
                break;
            case IntroState.StoryFadeOut:
                tween = GetTree().CreateTween();
                tween.TweenProperty(storySprite, "modulate:a", 0.0f, 2.0f);
                timer.Timeout -= HandleTimeout;
                timer.Timeout += HandleFinished;
                timer.Start(7.0);
                break;
        }
    }

    private void HandleFinished()
    {
        CallDeferred(nameof(SwitchToGameDeferred));
    }

    private void SwitchToGameDeferred()
    {
        Manager.Instance.SwitchScene(Manager.Scene.Game);
    }
}
