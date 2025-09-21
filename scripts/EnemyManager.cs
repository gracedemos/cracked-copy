using System;
using Godot;

public class EnemyManager
{
    public enum Wave
    {
        First,
        Second,
        Third,
        Fourth,
        Fifth,
        Sixth,
        Seventh,
        Eighth
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
    private bool spawnFifth = true;

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
            case Wave.Third:
                ThirdWave();
                break;
            case Wave.Fourth:
                FourthWave();
                break;
            case Wave.Fifth:
                FifthWave();
                break;
            case Wave.Sixth:
                SixthWave();
                break;
            case Wave.Seventh:
                SeventhWave();
                break;
            case Wave.Eighth:
                EighthWave();
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
        spawners[spawnerIndex].Spawn(0);
        spawnCount++;
        spawnTimer.Start();

        if (spawnCount >= 20)
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
        spawners[spawnerIndex].Spawn(0);
        spawnCount++;
        spawnTimer.Start();

        if (spawnCount >= 24)
        {
            spawnCount = 0;
            EnemyWave = Wave.Third;
        }
    }

    private void ThirdWave()
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
        spawners[spawnerIndex].Spawn(1);
        spawnCount++;
        spawnTimer.Start();

        if (spawnCount >= 8)
        {
            spawnCount = 0;
            EnemyWave = Wave.Fourth;
        }
    }

    private void FourthWave()
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
        spawners[spawnerIndex].Spawn(0);
        spawnCount++;
        spawnTimer.Start();

        if (spawnCount >= 8)
        {
            spawnCount = 0;
            EnemyWave = Wave.Fifth;
        }
    }

    private void FifthWave()
    {
        if (spawnFifth)
        {
            spawners[2].Spawn(2);
            spawnCount++;
            spawnFifth = false;
            spawnTimer.Start();
        }
        else
        {
            spawnCount++;
            spawnTimer.Start();
            if (spawnCount >= 6)
            {
                spawnCount = 0;
                spawnFifth = true;
                lastSpawnerIndex = -1;
                EnemyWave = Wave.Sixth;
            }
        }
    }

    private void SixthWave()
    {
        int spawnerIndex = random.Next(5);
        while (spawnerIndex == lastSpawnerIndex)
            spawnerIndex = random.Next(5);
        lastSpawnerIndex = spawnerIndex;
        if (spawnerIndex == BossSpawner)
            spawnerIndex = 5;
        spawners[spawnerIndex].Spawn(0);
        spawnCount++;
        spawnTimer.Start();

        if (spawnCount >= 32)
        {
            spawnCount = 0;
            EnemyWave = Wave.First;
        }
    }

    private void SeventhWave()
    {
        spawnCount++;
        spawnTimer.Start();
        if (spawnCount >= 4)
        {
            spawnCount = 0;
            EnemyWave = Wave.Eighth;
        }
    }

    private void EighthWave()
    {
        if (spawnFifth)
        {
            spawners[2].Spawn(2);
            spawnCount++;
            if (spawnCount >= 4)
                spawnFifth = false;
            spawnTimer.Start();
        }
        else
        {
            spawnCount++;
            spawnTimer.Start();
            if (spawnCount >= 8)
            {
                spawnCount = 0;
                spawnFifth = true;
                lastSpawnerIndex = -1;
                EnemyWave = Wave.First;
            }
        }
    }
}
