using Godot;

public partial class Manager : Node
{
    public enum MusicState
    {
        Intro,
        Start,
        GameOver,
        A,
        B,
        C
    }

    public enum Scene
    {
        Intro,
        Game,
        GameOver
    }

    public static Manager Instance { get; private set; }

    public const double BPM = 137.0;
    public const double SecondsPerBeat = 60.0 / 137.0;

    public AudioStreamPlayer MusicPlayer { get; private set; }
    public AudioStreamPlayer EffectPlayer { get; private set; }
    public MusicState ManagerMusicState { get; private set; }
    public Scene CurrentScene { get; private set; } = Scene.Intro;

    private AudioStreamWav audioIntro = ResourceLoader.Load<AudioStreamWav>("audio/Credits_Track.wav");
    private AudioStreamWav audioStart = ResourceLoader.Load<AudioStreamWav>("audio/Level_Start_1.wav");
    private AudioStreamWav audioA = ResourceLoader.Load<AudioStreamWav>("audio/Level_A_1.wav");
    private AudioStreamWav audioB = ResourceLoader.Load<AudioStreamWav>("audio/Level_B_1.wav");
    private AudioStreamWav audioC = ResourceLoader.Load<AudioStreamWav>("audio/Level_C_1.wav");
    private int musicLoops = 0;
    private bool continueFromB = false;
    private EnemyManager enemyManager;
    private PackedScene gameScene = ResourceLoader.Load<PackedScene>("scenes/game.tscn");
    private PackedScene gameOverScene = ResourceLoader.Load<PackedScene>("scenes/game-over.tscn");

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        MusicPlayer = new AudioStreamPlayer();
        ManagerMusicState = MusicState.Intro;
        MusicPlayer.Stream = audioIntro;
        MusicPlayer.Finished += HandleMusicFinished;
        AddChild(MusicPlayer);
        MusicPlayer.Play();

        Timer spawnTimer = new Timer();
        spawnTimer.WaitTime = SecondsPerBeat;
        spawnTimer.OneShot = true;
        AddChild(spawnTimer);
        enemyManager = new EnemyManager(spawnTimer);
    }

    public void SwitchScene(Scene scene)
    {
        QueueFreeCurrentScene();
        switch (scene)
        {
            case Scene.Game:
                GetTree().Root.AddChild(gameScene.Instantiate<Node>());
                enemyManager.Init(GetEnemySpawners(), GetPlayer());
                SetMusicState(MusicState.Start);
                CurrentScene = Scene.Game;
                break;
            case Scene.GameOver:
                GetTree().Root.AddChild(gameOverScene.Instantiate<Node>());
                SetMusicState(MusicState.GameOver);
                CurrentScene = Scene.GameOver;
                break;
        }
    }

    private void QueueFreeCurrentScene()
    {
        Node scene = null;
        switch (CurrentScene)
        {
            case Scene.Intro:
                scene = GetTree().Root.GetNode<Node>("Intro");
                break;
            case Scene.Game:
                enemyManager.Reset();
                scene = GetTree().Root.GetNode<Node>("Game");
                break;
            case Scene.GameOver:
                scene = GetTree().Root.GetNode<Node>("GameOver");
                break;
        }

        GetTree().Root.RemoveChild(scene);
        scene.QueueFree();
    }

    private EnemySpawner[] GetEnemySpawners()
    {
        EnemySpawner[] enemySpawners = {
            GetTree().Root.GetNode<EnemySpawner>("Game/Entities/Spawner0"),
            GetTree().Root.GetNode<EnemySpawner>("Game/Entities/Spawner1"),
            GetTree().Root.GetNode<EnemySpawner>("Game/Entities/Spawner2"),
            GetTree().Root.GetNode<EnemySpawner>("Game/Entities/Spawner3"),
            GetTree().Root.GetNode<EnemySpawner>("Game/Entities/Spawner4"),
            GetTree().Root.GetNode<EnemySpawner>("Game/Entities/Spawner5")
        };
        return enemySpawners;
    }

    private Player GetPlayer()
    {
        return GetTree().Root.GetNode<Player>("Game/Entities/Player");
    }

    private void SetMusicState(MusicState state)
    {
        switch (state)
        {
            case MusicState.Start:
                MusicPlayer.Stream = audioStart;
                MusicPlayer.Play();
                break;
            case MusicState.GameOver:
                MusicPlayer.Stream = audioStart;
                MusicPlayer.Play();
                break;
            case MusicState.A:
                MusicPlayer.Stream = audioA;
                MusicPlayer.Play();
                break;
            case MusicState.B:
                MusicPlayer.Stream = audioB;
                MusicPlayer.Play();
                break;
            case MusicState.C:
                MusicPlayer.Stream = audioC;
                MusicPlayer.Play();
                break;
        }
        ManagerMusicState = state;
    }

    private void HandleMusicFinished()
    {
        switch (ManagerMusicState)
        {
            case MusicState.Start:
                enemyManager.SpawnEnemies();
                SetMusicState(MusicState.A);
                break;
            case MusicState.GameOver:
                HandleMusicLoopGameOver();
                break;
            case MusicState.A:
                HandleMusicLoopA();
                break;
            case MusicState.B:
                HandleMusicLoopB();
                break;
            case MusicState.C:
                HandleMusicLoopC();
                break;
        }
    }

    private void HandleMusicLoopGameOver()
    {
        SetMusicState(MusicState.GameOver);
    }

    private void HandleMusicLoopA()
    {
        SetMusicState(MusicState.A);
        musicLoops++;
        if (musicLoops >= 2)
        {
            musicLoops = 0;
            SetMusicState(MusicState.B);
        }

    }

    private void HandleMusicLoopB()
    {
        if (continueFromB)
        {
            musicLoops = 0;
            SetMusicState(MusicState.C);
        }
        else
        {
            SetMusicState(MusicState.B);
            musicLoops++;
            if (musicLoops >= 2)
            {
                musicLoops = 0;
                continueFromB = true;
                SetMusicState(MusicState.A);
            }
        }
    }

    private void HandleMusicLoopC()
    {
        SetMusicState(MusicState.C);
    }
}
