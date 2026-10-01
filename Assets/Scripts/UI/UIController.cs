using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private LvExpUI levelExpPanel;

    [SerializeField]
    private GameOverUI gameOverPanel;

    [SerializeField]
    private LiveRankingUI liveRankingPanel;

    private Entity player;
    private string playerName;
    private int currentSessionId;
    public void Setup(Entity player, string playerName, int currentSessionId)
    {
        this.player = player;
        this.playerName = playerName;
        this.currentSessionId = currentSessionId;

        gameOverPanel?.Setup(playerName, currentSessionId);
        liveRankingPanel?.Setup(playerName);

        player.onDeath.AddListener(ShowGameOverPanel);
        player.onLevelup.AddListener(UpdateLevelHUD);
        player.onAddExperience.AddListener(UpdateExperinceHUD);

        UpdateLevelHUD(player.Data.Level);
        UpdateExperinceHUD(player.Data.Score % player.LevelupAmount, player.LevelupAmount);
    }

    public void ShowGameOverPanel()
    {   
        if(gameOverPanel != null)
        {
            gameOverPanel.gameObject.SetActive(true);
            gameOverPanel.ShowGameOver();
        }
    }
    public void UpdateLevelHUD(int level)
    {
        levelExpPanel.SetLevel(level);
    }
    public void UpdateExperinceHUD(int current, int max)
    {
        levelExpPanel.SetExperience(current, max);
    }
}
