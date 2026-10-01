using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Timeline.Actions;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField playerNameInput;
    [SerializeField] 
    private Button startGameButton;
    [SerializeField]
    private string mainSceneName;
    private void Start()
    {
        startGameButton.onClick.AddListener(OnStartGame);
    }

    private void OnStartGame()
    {
        string playerName = playerNameInput.text.Trim();
        Debug.Log(playerName);
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("플레이어 이름을 입력해주세요!");
            return;
        }

        // PlayerDataManager에 저장
        PlayerDataManager.Instance.SetPlayerName(playerName);

        // 메인 씬으로 이동
        SceneManager.LoadScene(mainSceneName);
    }
}