using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    // TitleScene에서 선택된 데이터들 메인씬에서 유지용도

    public static PlayerDataManager Instance { get; private set; }

    public string PlayerName { get; private set; } = "";
    public int SelectedSkinIndex { get; private set; } = 0;

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
        Debug.Log($"플레이어 이름 설정: {PlayerName}");
    }

    public void SetPlayerSkin(int skinIndex)
    {
        SelectedSkinIndex = skinIndex;
    }


    public void ResetGameData()
    {
        PlayerName = "";
        SelectedSkinIndex = 0;
    }
}