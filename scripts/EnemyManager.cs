using System;
using Godot;

public class EnemyManager
{
    public enum Wave
    {
        First,
        Second
    }

    private const int BossSpawner = 2;

    public Wave EnemyWave { get; private set; } = Wave.First;

    private Random random;
    private EnemySpawner[] spawners;
    private Player player;
    private Timer spawnTimer;
    private int lastSpawnerIndex = -1;
    private int spawnCount = 0;
    private bool doSpawn = true;

    public EnemyManager(Timer spawnTimer)
    {
        random = new Random();
        this.spawnTimer = spawnTimer;
        this.spawnTimer.Timeout += SpawnEnemies;
    }

    public void SpawnEnemies()
    {
        switch (EnemyWave)
        {
            case Wave.First:
                FirstWave();
                break;
            case Wave.Second:
                SecondWave();
                break;
        }
    }

    public void Reset()
    {
        spawnTimer.Stop();
        EnemyWave = Wave.First;
        spawners = null;
        player = null;
        lastSpawnerIndex = -1;
        spawnCount = 0;
        doSpawn = true;
    }

    public void Init(EnemySpawner[] spawners, Player player)
    {
        this.spawners = spawners;
        this.player = player;
    }

    private void FirstWave()
    {
        if (!doSpawn)
        {
            doSpawn = true;
            spawnTimer.Start();
            return;
        }
        doSpawn = false;

        int spawnerIndex = random.Next(5);
        while (spawnerIndex == lastSpawnerIndex)
            spawnerIndex = random.Next(5);
        lastSpawnerIndex = spawnerIndex;
        if (spawnerIndex == BossSpawner)
            spawnerIndex = 5;
        spawners[spawnerIndex].Spawn();
        spawnCount++;
        spawnTimer.Start();

        if (spawnCount >= 32)
        {
            spawnCount = 0;
            EnemyWave = Wave.Second;
        }
    }

    private void SecondWave()
    {
        int spawnerIndex = random.Next(5);
        while (spawnerIndex == lastSpawnerIndex)
            spawnerIndex = random.Next(5);
        lastSpawnerIndex = spawnerIndex;
        if (spawnerIndex == BossSpawner)
            spawnerIndex = 5;
        spawners[spawnerIndex].Spawn();
        spawnCount++;
        spawnTimer.Start();

        if (spawnCount >= 16)
        {
            spawnCount = 0;
            EnemyWave = Wave.First;
        }
    }
}
