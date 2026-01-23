/*
====================================================================
* WaveTransitionUI - Wave Transition Countdown Display
====================================================================
* Project: Showroom_Tango (2-Player Top-Down Bullet-Hell)
* Course: PRG - Game & Multimedia Design
* Developer: Julian
* Date: 2025-01-22
* Version: 1.0 - Wave transition countdown implementation
*
* WICHTIG: KOMMENTIERUNG NICHT LOESCHEN!
* Diese detaillierte Authorship-Dokumentation ist fuer die akademische
* Bewertung erforderlich und darf nicht entfernt werden!
*
* AUTHORSHIP CLASSIFICATION:
*
* [HUMAN-AUTHORED]
* - Requirement for wave transition feedback
* - 5-second countdown matching wave delay
* - Text-only approach
*
* [AI-ASSISTED]
* - Countdown coroutine implementation
* - UI fade and text updates
* - Event subscription pattern
* - Academic header formatting
*
* [AI-GENERATED]
* - None
*
* DEPENDENCIES:
* - UnityEngine.UI (Unity UI system)
* - TMPro (TextMeshPro)
* - EnemySpawner (for wave events)
*
* NOTES:
* - Displays "NEXT WAVE IN X..." countdown for 5 seconds
* - Matches existing 5-second delay between waves
* - Uses project color palette (Magenta/Cyan)
* - No changes to wave timing
====================================================================
*/

using System.Collections;
using UnityEngine;
using TMPro;

public class WaveTransitionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Settings")]
    [SerializeField] private float countdownDuration = 5f; // Match EnemySpawner delay
    [SerializeField] private Color textColor = new Color(0.667f, 0f, 0.784f, 1f); // Magenta

    private EnemySpawner enemySpawner;
    private int lastWaveNumber = 0;

    void Start()
    {
        // Hide countdown initially
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
            Debug.Log("[WaveTransitionUI] Countdown panel found and hidden");
        }
        else
        {
            Debug.LogError("[WaveTransitionUI] Countdown panel is NULL! Not assigned in Inspector!");
        }

        // Set text color
        if (countdownText != null)
        {
            countdownText.color = textColor;
            Debug.Log($"[WaveTransitionUI] Countdown text found, color set to {textColor}");
        }
        else
        {
            Debug.LogError("[WaveTransitionUI] Countdown text is NULL! Not assigned in Inspector!");
        }

        // Find EnemySpawner
        enemySpawner = FindAnyObjectByType<EnemySpawner>();
        if (enemySpawner != null)
        {
            Debug.Log("[WaveTransitionUI] EnemySpawner found successfully");
        }
        else
        {
            Debug.LogError("[WaveTransitionUI] EnemySpawner NOT FOUND!");
        }
    }

    void Update()
    {
        if (enemySpawner == null) return;

        int currentWave = enemySpawner.GetCurrentWave();
        bool waveActive = enemySpawner.IsWaveActive();

        // Detect wave completion: wave was active, now it's not (and not the final wave)
        if (lastWaveNumber == currentWave && waveActive == false && currentWave < 3)
        {
            // Wave just finished, trigger countdown once
            if (lastWaveNumber > 0) // Don't trigger on game start
            {
                Debug.LogWarning($"[WaveTransitionUI] Wave {currentWave} completed! Showing countdown for wave {currentWave + 1}");
                StartCoroutine(ShowWaveTransition(currentWave + 1));
                lastWaveNumber = -1; // Prevent re-triggering
            }
        }

        // Reset tracking when wave becomes active
        if (waveActive && lastWaveNumber < 0)
        {
            lastWaveNumber = currentWave;
            Debug.Log($"[WaveTransitionUI] Wave {currentWave} now active, reset tracking");
        }
    }

    private IEnumerator ShowWaveTransition(int nextWave)
    {
        if (countdownPanel == null || countdownText == null)
        {
            Debug.LogError("[WaveTransitionUI] Cannot show countdown - panel or text is null!");
            yield break;
        }

        Debug.LogWarning($"[WaveTransitionUI] SHOWING COUNTDOWN for wave {nextWave}!");

        // Show panel
        countdownPanel.SetActive(true);

        // Countdown from 5 to 1
        for (int i = (int)countdownDuration; i > 0; i--)
        {
            countdownText.text = $"NEXT WAVE IN {i}...";
            Debug.Log($"[WaveTransitionUI] Countdown: {i}");
            yield return new WaitForSeconds(1f);
        }

        // Hide panel
        countdownPanel.SetActive(false);
        Debug.Log("[WaveTransitionUI] Countdown finished, panel hidden");
    }

    /// <summary>
    /// Manually trigger countdown (for testing or special cases)
    /// </summary>
    public void TriggerCountdown(int nextWaveNumber)
    {
        StartCoroutine(ShowWaveTransition(nextWaveNumber));
    }
}
