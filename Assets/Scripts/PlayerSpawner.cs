using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawner : EntitySpawner
{
    [SerializeField]
    private Player playerPrefab;

    // 플레이어 전용 의존성
    private UIController uiController;
    private CinemachineVirtualCamera virtualCamera;
    private SkillIconManager skillIconManager;

    // 이벤트
    public System.Action<Player> OnPlayerSpawned;
    public System.Action<Player> OnPlayerDeath;
    public UnityEvent<Player> onPlayerSpawned;
    public UnityEvent<Player> onPlayerDeath;
    private Player currentPlayer;
    private string playerName;

    public void SetupPlayer(UIController uiController, CinemachineVirtualCamera virtualCamera, SkillIconManager skillIconManager, string name)
    {
        this.uiController = uiController;
        this.virtualCamera = virtualCamera;
        this.skillIconManager = skillIconManager;

        playerName = name;
    }

    public Player SpawnPlayer()
    {
        ValidateSetup();

        if (currentPlayer != null)
        {
            Debug.LogWarning("이미 플레이어가 존재합니다.");
            return currentPlayer;
        }

        Vector3 spawnPoint = GetRandomSpawnPoint();

        Player clone = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
        clone.Setup(new EntityInfo(playerName, "Test_Image"),
                    new EntityData(entityBasicData.level, entityBasicData.hp, entityBasicData.damage, entityBasicData.defense),
                    damagePopupManager, killLogManager, scoreBlockSpawner);
        //clone.AddScore(55555 + Random.Range(0, 1000)); //테스트 영상용
        // 스킨 적용
        ApplyRandomSkin(clone);

        // 사망 이벤트 구독
        clone.onDeath.AddListener(() => HandlePlayerDeath(clone));

        currentPlayer = clone;
        onPlayerSpawned?.Invoke(clone);

        return clone;
    }

    private void HandlePlayerDeath(Player player)
    {
        onPlayerDeath?.Invoke(player);
        currentPlayer = null;
    }
}

/*
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerSpawner : EntitySpawner
{
    // 플레이어 프리팹
    [SerializeField]
    private Player player;
    // mainScene의 중요UI들 의존 필요한 것들
    [SerializeField]
    private UIController uiController;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    private SkillIconManager skillIconManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //Entity clone = GameObject.Instantiate(enemy[Random.Range(0, enemy.Length)]);
            Player clone = Instantiate(player, Vector3.zero, Quaternion.identity);
        
            string name = PlayerDataManager.Instance?.PlayerName;
            if(name == "" || name == null)
            {
                name = "Test_Player_" + Random.Range(0, 10000);
            }
            Debug.Log($"Player Name: {name}");

            clone.Setup(new EntityInfo(name, "Test_Image"), new EntityData(1, 100, 10, 1), damagePopupManager, killLogManager, scoreBlockSpawner);

            if(skinnedMesh != null || material != null) 
            {
                clone.SetSkin(skinnedMesh[Random.Range(0, skinnedMesh.Length)],
                                         material[Random.Range(0, material.Length)]);
            }

            clone.onDeath.AddListener(SampleOnDeath);

            if (virtualCamera != null)
            {
                virtualCamera.Follow = clone.transform;
            }

            // 사용자가 이름을 입력하고 게임 시작 버튼 클릭 시
            var playerName = clone.Info.EntityName;
            var currentSession = EntityGameManager.StartNewGame(playerName);
            
            if (currentSession != null)
            {
                Debug.Log($"게임 시작! 세션 ID: {currentSession.SessionID}");

                // 플레이어 GameObject 매핑 등록
                int playerInstanceId = GetInstanceID(); // 실제 GameObject
                EntityGameManager.RegisterPlayerEntity(playerInstanceId);

                uiController.Setup(clone, playerName, currentSession.SessionID);
            }
        }
    }

    /// <summary>
    /// 테스트 (죽었을 때 게임 종료) - 아직 확실한 process 미구현
    /// </summary>
    private void SampleOnDeath()
    {
        Debug.Log("SampleOnDeath : SampleOnDeath : SampleOnDeath : SampleOnDeath");
        //GameSessionRepository.EndSession(currentSession.SessionID, currentSession.Score, currentSession.Level, currentSession.EnemiesKilled, currentSession.DeathCount);
        GameSessionManager.OnPlayerDeath();
    }
}
 */
