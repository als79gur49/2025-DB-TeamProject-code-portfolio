using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
[System.Serializable]
public class EntityBasicData
{
    public int level = 1;
    public int hp = 100;
    public int damage = 10;
    public int defense = 1;
}
public class SessionManager : MonoBehaviour
{
    // Spawner와 UnityEvent로 연결

    [Header("핵심 매니저들")]
    [SerializeField]
    private DamagePopupManager damagePopupManager; // 공격받을 경우 데미지 출력
    [SerializeField]
    private KillLogManager killLogManager; // 죽을 경우 킬로그 출력
    [SerializeField]
    private ScoreBlockSpawner scoreBlockSpawner; // 죽을 경우 점수블럭 스폰
    [SerializeField]
    private UIController uiController; // 플레이어 UI, 랭킹관련 UI
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera; // 플레이어에 연결될 카메라
    [SerializeField]
    private SkillIconManager skillIconManager; // 스킬 저장소 // 작동X

    [Header("Spawners")]
    [SerializeField]
    private PlayerSpawner playerSpawner;
    [SerializeField]
    private EnemySpawner enemySpawner;

    [Header("Game Session")]
    private GameSessionModel currentSession;
    private bool isGameActive = false;

    [Header("Entity 기본 수치")]
    public EntityBasicData entityBasicData;

    public GameSessionModel CurrentSession => currentSession;
    public bool IsGameActive => isGameActive;

    private string playerName;
    private void Awake()
    {
        ValidateComponents(); // 핵심 매니저 할당되었는지 검증
        InitializeSpawners();
    }

    private void Start()
    {
        // 게임 시작
        StartGame();
    }


    private void ValidateComponents()
    {
        if (damagePopupManager == null || killLogManager == null || scoreBlockSpawner == null)
        {
            Debug.LogError("Core Manager 참조가 없습니다.");
            return;
        }

        if (playerSpawner == null || enemySpawner == null)
        {
            Debug.LogError("Player, EnemySpawner 참조가 없습니다.");
            return;
        }

        if (uiController == null || virtualCamera == null)
        {
            Debug.LogError("UI 또는 Camera 참조가 없습니다.");
            return;
        }
    }

    private void InitializeSpawners()
    {
        // 의존성 주입
        playerSpawner.Setup(damagePopupManager, killLogManager, scoreBlockSpawner, entityBasicData);
        enemySpawner.Setup(damagePopupManager, killLogManager, scoreBlockSpawner, entityBasicData);

        // 플레이어 스포너 의존성 주입
        playerSpawner.SetupPlayer(uiController, virtualCamera, skillIconManager, GetPlayerName());

        // 이벤트 구독
        playerSpawner.onPlayerSpawned.AddListener(OnPlayerSpawned);
        playerSpawner.onPlayerDeath.AddListener(OnPlayerDeath);

        enemySpawner.onEnemySpawned.AddListener(OnEnemySpawned);
        enemySpawner.onEnemyDeath.AddListener(OnEnemyDeath);
    }


    private string GetPlayerName()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            // 타이틀씬에서 입력된 이름
            playerName = PlayerDataManager.Instance?.PlayerName;
            if (string.IsNullOrEmpty(playerName))
            {
                playerName = $"Test_Player_{Random.Range(0, 1000)}";
            }
        }
        return playerName;
    }

    public void StartGame()
    {
        if (isGameActive)
        {
            Debug.LogWarning("이미 게임이 진행 중입니다.");
            return;
        }

        // 전 게임 랭킹을 확인하기 위해 새로운 게임 시작 시 랭킹 보드 삭제
        RankingManager.ClearAllSessionRanking();

        string playerName = GetPlayerName();
        currentSession = EntityGameManager.StartNewGame(playerName);

        if (currentSession != null)
        {
            isGameActive = true;
            Debug.Log($"게임 시작. 게임세션 ID: {currentSession.SessionID}");

            // 플레이어 스폰
            playerSpawner.SpawnPlayer();
        }
        else
        {
            Debug.LogError("게임 세션 생성에 실패했습니다.");
        }
    }

    public void EndGame()
    {
        if (!isGameActive)
        {
            Debug.LogWarning("종료할 게임이 없습니다.");
            return;
        }

        isGameActive = false;
        EntityGameManager.OnPlayerDeath();

        Debug.Log("게임 종료");
    }

    private void OnPlayerSpawned(Player player)
    {
        if (currentSession != null)
        {
            // UI 설정
            uiController.Setup(player, player.Info.EntityName, currentSession.SessionID);

            // 카메라 설정
            if (virtualCamera != null)
            {
                virtualCamera.Follow = player.transform;
            }

            // 플레이어 엔티티 등록
            int playerInstanceId = player.GetInstanceID();
            EntityGameManager.RegisterPlayerEntity(playerInstanceId);
        }
    }

    private void OnPlayerDeath(Player player)
    {
        EndGame();
    }


    private void OnEnemySpawned(Enemy enemy)
    {
        // 적 스폰 시 필요한 로직
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        // 적 사망 시 필요한 로직
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (playerSpawner != null)
        {
            playerSpawner.onPlayerSpawned.RemoveAllListeners();
            playerSpawner.onPlayerDeath.RemoveAllListeners();
        }

        if (enemySpawner != null)
        {
            enemySpawner.onEnemySpawned.RemoveAllListeners();
            enemySpawner.onEnemyDeath.RemoveAllListeners();
        }
    }
}