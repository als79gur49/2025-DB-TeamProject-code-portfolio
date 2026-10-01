using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemySpawner : EntitySpawner
{
    [SerializeField] 
    private Enemy[] enemyPrefabs;
    private MemoryPool<Enemy> memoryPool;

    // 이벤트
    public UnityEvent<Enemy> onEnemySpawned;
    public UnityEvent<Enemy> onEnemyDeath;

    // 남은 수에 따라 적 생성
    private int maxEnemyCount = 10;
    private int currentEnemyCount;
    private float timer;
    [SerializeField]
    private float spawnMultiplier = 1f;
    private void Awake()

    {
        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            memoryPool = new MemoryPool<Enemy>(enemyPrefabs[0], this.transform, 5);
        }
    }

    public override void Setup(DamagePopupManager damagePopupManager, KillLogManager killLogManager, ScoreBlockSpawner scoreBlockSpawner,
        EntityBasicData basicData)
    {
        base.Setup(damagePopupManager, killLogManager, scoreBlockSpawner, basicData);

        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            memoryPool = new MemoryPool<Enemy>(enemyPrefabs[0], this.transform, 5);
        }
    }

    public Enemy SpawnEnemy()
    {
        ValidateSetup();

        if (memoryPool == null)
        {
            Debug.LogError("MemoryPool이 없음.");

            return null;
        }

        Enemy clone = memoryPool.ActivatePoolItem();
        // Enemy MemoryPool이기에, GetInstanceID() 같은 값 나옴. 
        string name = $"Enemy_{clone.GetInstanceID()}_{Random.Range(0, 100)}";
        
        // 위치 변경하려면 NavMesh 일시중지
        clone.GetComponent<NavMeshAgent>().isStopped = true;
        Vector3 spawnPoint = GetRandomSpawnPoint();
        clone.transform.position = spawnPoint;
        clone.GetComponent<NavMeshAgent>().isStopped = false;

        clone.Setup(new EntityInfo(name, "Test_Image"),
                    new EntityData(entityBasicData.level, entityBasicData.hp, entityBasicData.damage, entityBasicData.defense),
                    memoryPool, damagePopupManager, killLogManager, scoreBlockSpawner);

        //FSM 기본 상태 지정
        clone.ChangeState(EntityStates.IdleState);

        // 스킨 적용
        ApplyRandomSkin(clone);

        // 사망 이벤트 구독
        clone.onDeath.AddListener(() => HandleEnemyDeath(clone));

        onEnemySpawned?.Invoke(clone);

        return clone;
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        onEnemyDeath?.Invoke(enemy);
    }

    private bool CheckEnemyCount()
    {
        GameStatusInfo gameInfo = EntityGameManager.GetCurrentGameStatus();
        currentEnemyCount = gameInfo.AliveEntities - gameInfo.PlayerEntities;

        return gameInfo.IsActive;
    }
    private void Update()
    {
        if(! CheckEnemyCount())
        {
            return;
        }
        // 적의 숫자가 많을수록 생성 간격이 길어진다
        float spawnDelay = currentEnemyCount / spawnMultiplier;

        if (Time.time > timer + spawnDelay)
        {
            SpawnEnemy();

            timer = Time.time;
        }
    }
}

/*
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : EntitySpawner
{
    [SerializeField]
    private Enemy[] enemy;

    private MemoryPool<Enemy> memoryPool;

    private void Awake()
    {
        memoryPool = new MemoryPool<Enemy>(enemy[0], this.transform, 5);
    }

    public void Setup(DamagePopupManager damagePopupManager, KillLogManager killLogManager)
    {
        this.damagePopupManager = damagePopupManager;
        this.killLogManager = killLogManager;

        memoryPool = new MemoryPool<Enemy>(enemy[0], this.transform, 5);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            //Entity clone = GameObject.Instantiate(enemy[Random.Range(0, enemy.Length)]);
            Enemy clone = memoryPool.ActivatePoolItem();

            string name = "Test_Enemy_01_" + Random.Range(0, 10000);


            clone.Setup(new EntityInfo(name, "Test_Image"), new EntityData(1, 100, 10, 1), memoryPool, damagePopupManager, killLogManager, scoreBlockSpawner);
            clone.AddScore(5678 + Random.Range(0, 6789));
            clone.ChangeState(EntityStates.IdleState);

            if (skinnedMesh != null || material != null)
            {
                clone.SetSkin(skinnedMesh[Random.Range(0, skinnedMesh.Length)],
                                         material[Random.Range(0, material.Length)]);
            }
        }
    }
}

 */ 