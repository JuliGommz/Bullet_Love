/*
====================================================================
* HUDManager.cs - In-Game HUD Display Controller
====================================================================
* Project: Showroom_Tango
* Course: PRG - Game & Multimedia Design SRH Hochschule
* Developer: Julian Gomez
* Date: 2025-01-20
* Version: 1.0
*
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
*
* [HUMAN-AUTHORED]
* - HUD layout concept (HP bars top left/right, score center)
* - Game Over/Victory screen requirements
*
* [AI-ASSISTED]
* - Unity UI integration
* - Event subscription pattern
* - Dynamic player HP tracking
*
* [AI-GENERATED]
* - Complete UI update logic
====================================================================
*/

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Score Display")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Wave Display")]
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Player 1 HUD")]
    [SerializeField] private Slider player1HPBar;
    [SerializeField] private TextMeshProUGUI player1NameText;

    [Header("Player 2 HUD")]
    [SerializeField] private Slider player2HPBar;
    [SerializeField] private TextMeshProUGUI player2NameText;

    [Header("Game Over/Victory")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    void Start()
    {
        // Subscribe to managers
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScore;
            UpdateScore(ScoreManager.Instance.GetTeamScore());
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged += HandleGameStateChange;
        }

        // Hide end screens
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        // Start updating player HP
        InvokeRepeating(nameof(UpdatePlayerHP), 0.5f, 0.2f); // Update 5x per second
        InvokeRepeating(nameof(UpdateWave), 1f, 1f); // Update every second
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScore;
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged -= HandleGameStateChange;
        }
    }

    private void UpdateScore(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {newScore}";
        }
    }

    private void UpdateWave()
    {
        EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        if (spawner != null && waveText != null)
        {
            waveText.text = $"WAVE {spawner.GetCurrentWave()} / 3";
        }
    }

    private void UpdatePlayerHP()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length >= 1)
        {
            UpdateSinglePlayerHP(players[0], player1HPBar, player1NameText);
        }

        if (players.Length >= 2)
        {
            UpdateSinglePlayerHP(players[1], player2HPBar, player2NameText);
        }
    }

    private void UpdateSinglePlayerHP(GameObject playerObj, Slider hpBar, TextMeshProUGUI nameText)
    {
        if (hpBar == null) return;

        PlayerHealth health = playerObj.GetComponent<PlayerHealth>();
        PlayerController controller = playerObj.GetComponent<PlayerController>();

        if (health != null)
        {
            int currentHP = health.GetCurrentHealth();
            int maxHP = health.GetMaxHealth();
            float hpRatio = (float)currentHP / maxHP;

            Debug.Log($"[HUDManager] Player: {playerObj.name} | HP: {currentHP}/{maxHP} | Ratio: {hpRatio} | Bar: {hpBar.name}");

            hpBar.value = hpRatio;

            if (nameText != null && controller != null)
            {
                nameText.text = controller.GetPlayerName();
            }
        }
        else
        {
            Debug.LogWarning($"[HUDManager] No PlayerHealth on {playerObj.name}");
        }
    }

    private void HandleGameStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.GameOver:
                ShowGameOver();
                break;
            case GameState.Victory:
                ShowVictory();
                break;
        }
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            UpdateFinalScore();
        }
    }

    private void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            UpdateFinalScore();
        }
    }

    private void UpdateFinalScore()
    {
        if (finalScoreText != null && ScoreManager.Instance != null)
        {
            finalScoreText.text = $"FINAL SCORE: {ScoreManager.Instance.GetTeamScore()}";
        }
    }

    /// <summary>
    /// Called by Restart button in UI
    /// </summary>
    public void OnRestartButtonPressed()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.RequestRestartServerRpc();
        }
    }
}
