using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Legacy;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class ScoreBlockSpawner : MonoBehaviour
{
    private Vector3 standardPosition = new Vector3(-40, 0.5f, -40);

    private float xSize = 20 * 5;
    private float ySize = 20 * 5;

    private float distance = 5f;

    [SerializeField]
    private ScoreBlock scoreBlockPrefab;
    private MemoryPool<ScoreBlock> memoryPool;

    private Queue<Vector3> respawnPoints;
    private float respawnTime;

    [Header("큰 경험치")]
    [SerializeField]
    private Color bigBlockColor = Color.cyan;
    [SerializeField]
    private int bigBlockScore = 200;

    [Header("중간 경험치")]
    [SerializeField]
    private Color mediumBlockColor = Color.white;
    [SerializeField]
    private int mediumBlockScore = 100;

    [Header("작은 경험치")]
    [SerializeField]
    private Color smallBlockColor = Color.blue;
    [SerializeField]
    private int smallBlockScore = 50;

    public void Setup(Vector3 standardPosition, float xSize, float ySize,
        float respawnTime)
    {
        this.standardPosition = standardPosition;

        this.xSize = xSize;
        this.ySize = ySize;

        memoryPool = new MemoryPool<ScoreBlock>(scoreBlockPrefab, this.transform, 5);

        respawnPoints = new Queue<Vector3>();
        this.respawnTime = respawnTime;
        
        // 고정된 위치에 최초 생성
        SpawnScoreBlocks();
        // 재생성 가능 객체들 일정시간 마다 생성
        RespawnByTime(respawnTime);
    }


    void Start()
    {


        Setup(new Vector3(-40, 0.5f, -40), 20 * 5, 20 * 5, 3f);
    }
    public void SpawnScoreBlocks()
    {
        for(float i = 0; i < ySize; i+=distance)
        {
            for(float j = 0; j < xSize; j+=distance)
            {
                Vector3 addedAmount = new Vector3(j, 0, i);
                if(! Physics.CheckBox(standardPosition + addedAmount, new Vector3(0.5f, 0.3f, 0.5f), Quaternion.identity))
                {
                    ScoreBlock clone = Spawn(standardPosition + addedAmount, true, this);
                    clone.YoYoMoving();
                    clone.gameObject.transform.SetParent(transform, true);
                }
            }
        }
    }

    public void SpawnScoreBlocksByKilling(Vector3 spawnPosition, int amount)
    {
        for (int i = 0;i < amount; ++i)
        {
            ScoreBlock clone = Spawn(spawnPosition, false);
            clone.LaunchUpwards();
            clone.gameObject.transform.SetParent(transform, true);
        }
    }

    private ScoreBlock Spawn(Vector3 spawnPoint, bool canRespawn, ScoreBlockSpawner spawner = null)
    {
        ScoreBlock clone = memoryPool.ActivatePoolItem();
        clone.transform.position = spawnPoint;
        clone.transform.rotation = Quaternion.identity;

        float randomValue = Random.Range(0, 1f);
        if(randomValue > 0.8f)
        {
            clone.Setup(bigBlockScore, bigBlockColor, 2f, memoryPool, canRespawn, spawner);
        }
        else if(randomValue > 0.5f)
        {
            clone.Setup(mediumBlockScore, mediumBlockColor, 1.5f, memoryPool, canRespawn, spawner);
        }
        else if(randomValue >= 0f)
        {
            clone.Setup(smallBlockScore, smallBlockColor, 0.8f, memoryPool, canRespawn, spawner);
        }

        return clone;
    }

    public void EnQueuePosition(Vector3 position)
    {
        respawnPoints.Enqueue(position);
    }

    public void RespawnByTime(float time)
    {
        StartCoroutine(respawnByTime(time));
    }

    private IEnumerator respawnByTime(float time)
    {
        while(true)
        {
            if (respawnPoints.Count <= 0)
            {
                yield return null;

                continue;
            }

            // 저장된 위치 가져오기
            Vector3 position = respawnPoints.Dequeue();

            // 해당 위치에 재생성
            ScoreBlock clone = Spawn(position, true, this);
            clone.YoYoMoving();
            clone.gameObject.transform.SetParent(transform, true);

            yield return new WaitForSeconds(time);
        }
    }
}
