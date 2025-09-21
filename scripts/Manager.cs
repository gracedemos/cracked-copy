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
    public AudioStreamPlayer MusicPlayerB { get; private set; }
    public AudioStreamPlayer EffectPlayer { get; private set; }
    public MusicState ManagerMusicState { get; private set; }
    public Scene CurrentScene { get; private set; } = Scene.Intro;
    public AudioStreamWav AudioDamageEnemy { get; private set; } = ResourceLoader.Load<AudioStreamWav>("audio/Egg_Dmg_Enemy.wav");
    public AudioStreamWav AudioKillEnemy { get; private set; } = ResourceLoader.Load<AudioStreamWav>("audio/Egg_Kill_Enemy.wav");
    public AudioStreamWav[] LaunchAudios { get; private set; } = {
        ResourceLoader.Load<AudioStreamWav>("audio/Launch_1.wav"),
        ResourceLoader.Load<AudioStreamWav>("audio/Launch_2.wav"),
        ResourceLoader.Load<AudioStreamWav>("audio/Launch_3.wav")
    };
    public AudioStreamWav[] SpawnAudios { get; private set; } = {
        ResourceLoader.Load<AudioStreamWav>("audio/Spawn_1.wav"),
        ResourceLoader.Load<AudioStreamWav>("audio/Spawn_2.wav"),
        ResourceLoader.Load<AudioStreamWav>("audio/Spawn_3.wav"),
        ResourceLoader.Load<AudioStreamWav>("audio/Spawn_4.wav"),
        ResourceLoader.Load<AudioStreamWav>("audio/Spawn_5.wav")
    };

    private AudioStreamWav audioIntro = ResourceLoader.Load<AudioStreamWav>("audio/Credits_Track_STEREO.wav");
    private AudioStreamWav audioStart = ResourceLoader.Load<AudioStreamWav>("audio/Level_Start_1.wav");
    private AudioStreamWav audioGutteralInitial = ResourceLoader.Load<AudioStreamWav>("audio/Lose_Track_Gutteral_Initial.wav");
    private AudioStreamWav audioGutteralLoop = ResourceLoader.Load<AudioStreamWav>("audio/Lose_Track_Gutteral_Loop.wav");
    private AudioStreamWav audioDroneLoop = ResourceLoader.Load<AudioStreamWav>("audio/Lose_Track_Drone_Loop_Hopefully_Not.wav");
    private AudioStreamWav audioA1 = ResourceLoader.Load<AudioStreamWav>("audio/Level_A_1.wav");
    private AudioStreamWav audioA2 = ResourceLoader.Load<AudioStreamWav>("audio/Level_A_2.wav");
    private AudioStreamWav audioA3 = ResourceLoader.Load<AudioStreamWav>("audio/Level_A_3.wav");
    private AudioStreamWav audioB1 = ResourceLoader.Load<AudioStreamWav>("audio/Level_B_1.wav");
    private AudioStreamWav audioB2 = ResourceLoader.Load<AudioStreamWav>("audio/Level_B_2.wav");
    private AudioStreamWav audioB3 = ResourceLoader.Load<AudioStreamWav>("audio/Level_B_3.wav");
    private AudioStreamWav audioC1 = ResourceLoader.Load<AudioStreamWav>("audio/Level_C_1.wav");
    private AudioStreamWav audioC2 = ResourceLoader.Load<AudioStreamWav>("audio/Level_C_2.wav");
    private AudioStreamWav audioC3 = ResourceLoader.Load<AudioStreamWav>("audio/Level_BOSS_AFTER_B_3.wav");
    private int musicLoops = 0;
    private int musicCycle = 0;
    private EnemyManager enemyManager;
    private PackedScene gameScene = ResourceLoader.Load<PackedScene>("scenes/game.tscn");
    private PackedScene gameOverScene = ResourceLoader.Load<PackedScene>("scenes/game-over.tscn");
    private bool gameOverInitialDone = false;

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

        MusicPlayerB = new AudioStreamPlayer();
        MusicPlayerB.Finished += () => MusicPlayerB.Play();
        AddChild(MusicPlayerB);

        EffectPlayer = new AudioStreamPlayer();
        AddChild(EffectPlayer);

        Timer spawnTimer = new Timer();
        spawnTimer.WaitTime = SecondsPerBeat;
        spawnTimer.OneShot = true;
        AddChild(spawnTimer);
        enemyManager = new EnemyManager(spawnTimer);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("Quit"))
            GetTree().Root.QueueFree();
    }

    public void SwitchScene(Scene scene)
    {
        QueueFreeCurrentScene();
        switch (scene)
        {
            case Scene.Game:
                GetTree().Root.AddChild(gameScene.Instantiate<Node>());
                enemyManager.Init(GetEnemySpawners(), GetPlayer());
                Input.MouseMode = Input.MouseModeEnum.Hidden;
                SetMusicState(MusicState.Start);
                CurrentScene = Scene.Game;
                break;
            case Scene.GameOver:
                GetTree().Root.AddChild(gameOverScene.Instantiate<Node>());
                SetMusicState(MusicState.GameOver);
                MusicPlayerB.Stream = audioDroneLoop;
                MusicPlayerB.Play();
                CurrentScene = Scene.GameOver;
                break;
        }
    }

    private void Reset()
    {
        musicLoops = 0;
        musicCycle = 0;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        enemyManager.Reset();
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
                Reset();
                scene = GetTree().Root.GetNode<Node>("Game");
                break;
            case Scene.GameOver:
                MusicPlayerB.Stop();
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
                if (gameOverInitialDone)
                {
                    MusicPlayer.Stream = audioGutteralLoop;
                }
                else
                {
                    MusicPlayer.Stream = audioGutteralInitial;
                    gameOverInitialDone = true;
                }
                MusicPlayer.Play();
                break;
            case MusicState.A:
                switch (musicCycle)
                {
                    case 0:
                        MusicPlayer.Stream = audioA1;
                        break;
                    case 1:
                        MusicPlayer.Stream = audioA2;
                        break;
                    case 2:
                        MusicPlayer.Stream = audioA3;
                        break;
                }
                MusicPlayer.Play();
                break;
            case MusicState.B:
                switch (musicCycle)
                {
                    case 0:
                        MusicPlayer.Stream = audioB1;
                        break;
                    case 1:
                        MusicPlayer.Stream = audioB2;
                        break;
                    case 2:
                        MusicPlayer.Stream = audioB3;
                        break;
                }
                MusicPlayer.Play();
                break;
            case MusicState.C:
                switch (musicCycle)
                {
                    case 0:
                        MusicPlayer.Stream = audioC1;
                        break;
                    case 1:
                        MusicPlayer.Stream = audioC2;
                        break;
                    case 2:
                        MusicPlayer.Stream = audioC3;
                        break;
                }
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
        SetMusicState(MusicState.B);
    }

    private void HandleMusicLoopB()
    {
        SetMusicState(MusicState.C);
    }

    private void HandleMusicLoopC()
    {
        switch (musicCycle)
        {
            case 0:
                musicCycle++;
                SetMusicState(MusicState.A);
                break;
            case 1:
                musicCycle++;
                SetMusicState(MusicState.A);
                break;
            case 2:
                musicCycle = 0;
                SetMusicState(MusicState.A);
                break;
        }
    }
}
