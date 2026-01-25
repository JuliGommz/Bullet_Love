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
    private int lastObservedWave = 0;
    private bool lastWaveActiveState = false;
    private bool countdownShown = false;

    void Start()
    {
        // Validate Inspector references
        if (countdownPanel == null)
        {
            Debug.LogError("[WaveTransitionUI] SETUP REQUIRED: 'Countdown Panel' not assigned in Inspector! Select WaveCountdownPanel GameObject in Hierarchy → Inspector → assign countdownPanel field");
            enabled = false;
            return;
        }

        if (countdownText == null)
        {
            Debug.LogError("[WaveTransitionUI] SETUP REQUIRED: 'Countdown Text' not assigned in Inspector! Select WaveCountdownPanel GameObject in Hierarchy → Inspector → assign countdownText field (TextMeshProUGUI component)");
            enabled = false;
            return;
        }

        // Initialize UI
        countdownPanel.SetActive(false);
        countdownText.color = textColor;
        Debug.Log("[WaveTransitionUI] Initialized - countdown panel hidden, waiting for EnemySpawner");
    }

    void Update()
    {
        // Lazy-find EnemySpawner (handles network spawn timing)
        if (enemySpawner == null)
        {
            enemySpawner = FindAnyObjectByType<EnemySpawner>();
            if (enemySpawner != null)
            {
                Debug.Log("[WaveTransitionUI] EnemySpawner found! Starting wave monitoring");
                lastObservedWave = enemySpawner.GetCurrentWave();
                lastWaveActiveState = enemySpawner.IsWaveActive();
            }
            return;
        }

        // Monitor wave state changes
        int currentWave = enemySpawner.GetCurrentWave();
        bool currentWaveActive = enemySpawner.IsWaveActive();

        // DETECTION LOGIC: Wave just finished (active → inactive transition)
        if (lastWaveActiveState == true && currentWaveActive == false)
        {
            Debug.Log($"[WaveTransitionUI] WAVE {currentWave} COMPLETED! (active→inactive detected)");

            // Show countdown if not the final wave
            if (currentWave < 3)
            {
                Debug.Log($"[WaveTransitionUI] ✅ Triggering countdown for next wave ({currentWave + 1})");
                StartCoroutine(ShowWaveTransition(currentWave + 1));
                countdownShown = true;
            }
            else
            {
                Debug.Log($"[WaveTransitionUI] ℹ️ Final wave complete - no countdown needed");
            }
        }

        // DETECTION LOGIC: New wave started (wave number increased)
        if (currentWave > lastObservedWave)
        {
            Debug.Log($"[WaveTransitionUI] NEW WAVE STARTED: {lastObservedWave} → {currentWave}");
            countdownShown = false; // Reset for next transition
        }

        // Update tracking variables
        lastObservedWave = currentWave;
        lastWaveActiveState = currentWaveActive;
    }

    private IEnumerator ShowWaveTransition(int nextWave)
    {
        Debug.Log($"[WaveTransitionUI] ━━━ COUNTDOWN START ━━━ Next Wave: {nextWave}");

        // Show panel
        countdownPanel.SetActive(true);
        Debug.Log($"[WaveTransitionUI] Panel visible: {countdownPanel.activeSelf}");

        // Countdown from duration to 1
        for (int i = (int)countdownDuration; i > 0; i--)
        {
            countdownText.text = $"NEXT WAVE IN {i}...";
            Debug.Log($"[WaveTransitionUI] 🔢 Display: \"{countdownText.text}\" (Panel active: {countdownPanel.activeSelf})");
            yield return new WaitForSeconds(1f);
        }

        // Hide panel
        countdownPanel.SetActive(false);
        Debug.Log($"[WaveTransitionUI] ━━━ COUNTDOWN END ━━━ Panel hidden");
    }

    /// <summary>
    /// Manually trigger countdown (for testing or special cases)
    /// </summary>
    public void TriggerCountdown(int nextWaveNumber)
    {
        StartCoroutine(ShowWaveTransition(nextWaveNumber));
    }
}
