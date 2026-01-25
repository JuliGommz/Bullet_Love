/*
====================================================================
* LobbyUI - Main Lobby Screen Controller
====================================================================
* Project: Showroom_Tango (2-Player Top-Down Bullet-Hell)
* Course: PRG - Game & Multimedia Design
* Developer: Julian
* Date: 2025-01-23
* Version: 1.0 - Initial lobby UI implementation
*
* WICHTIG: KOMMENTIERUNG NICHT LOESCHEN!
*
* AUTHORSHIP CLASSIFICATION:
*
* [HUMAN-AUTHORED]
* - Lobby layout (2 player panels + center status)
* - Countdown display ("Starting in 3... 2... 1...")
* - Rankings button placement
* - Waiting message
*
* [AI-ASSISTED]
* - LobbyManager event subscription
* - Countdown visual updates
* - Player panel synchronization
* - FishNet local player detection
*
* [AI-GENERATED]
* - Complete UI orchestration logic
*
* DEPENDENCIES:
* - TMPro (TextMeshPro)
* - LobbyManager (network lobby state)
* - PlayerSetupUI (individual player panels)
* - FishNet.Managing.NetworkManager
*
* NOTES:
* - Updates in real-time based on LobbyManager events
* - Countdown uses same style as WaveTransitionUI
* - Rankings button placeholder for future PHP integration
====================================================================
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet.Managing;

public class LobbyUI : MonoBehaviour
{
    [Header("Player Setup Panels")]
    [SerializeField] private PlayerSetupUI player1SetupUI;
    [SerializeField] private PlayerSetupUI player2SetupUI;

    [Header("Center Status Display")]
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private Color countdownColor = new Color(0.667f, 0f, 0.784f, 1f); // Magenta

    [Header("Rankings")]
    [SerializeField] private Button rankingsButton;
    [SerializeField] private GameObject rankingsPlaceholderPanel;

    private NetworkManager networkManager;

    void Start()
    {
        Debug.Log("[LobbyUI] Initializing lobby UI...");

        // Get NetworkManager
        networkManager = FindAnyObjectByType<NetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("[LobbyUI] NetworkManager not found!");
            return;
        }

        // Setup countdown visuals
        if (countdownText != null)
        {
            countdownText.color = countdownColor;
        }

        // Hide countdown initially
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }

        // Setup rankings button
        if (rankingsButton != null)
        {
            rankingsButton.onClick.AddListener(OnRankingsButtonClicked);
        }

        if (rankingsPlaceholderPanel != null)
        {
            rankingsPlaceholderPanel.SetActive(false);
        }

        // Subscribe to LobbyManager events
        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.OnLobbyStateChanged += UpdateLobbyDisplay;
            LobbyManager.Instance.OnCountdownTick += UpdateCountdownDisplay;
        }
        else
        {
            Debug.LogWarning("[LobbyUI] LobbyManager not found yet - will retry");
            Invoke(nameof(RetryLobbyManagerConnection), 1f);
        }

        // Initial update
        UpdateLobbyDisplay();

        Debug.Log("[LobbyUI] Initialization complete");
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.OnLobbyStateChanged -= UpdateLobbyDisplay;
            LobbyManager.Instance.OnCountdownTick -= UpdateCountdownDisplay;
        }

        if (rankingsButton != null)
        {
            rankingsButton.onClick.RemoveListener(OnRankingsButtonClicked);
        }
    }

    private void RetryLobbyManagerConnection()
    {
        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.OnLobbyStateChanged += UpdateLobbyDisplay;
            LobbyManager.Instance.OnCountdownTick += UpdateCountdownDisplay;
            UpdateLobbyDisplay();
            Debug.Log("[LobbyUI] Successfully connected to LobbyManager");
        }
        else
        {
            Debug.LogError("[LobbyUI] LobbyManager still not found!");
        }
    }

    private void UpdateLobbyDisplay()
    {
        if (LobbyManager.Instance == null) return;

        int playerCount = LobbyManager.Instance.GetPlayerCount();
        bool countdownActive = LobbyManager.Instance.IsCountdownActive();

        Debug.Log($"[LobbyUI] Updating display - Players: {playerCount}, Countdown: {countdownActive}");

        // Update status text
        if (statusText != null)
        {
            if (countdownActive)
            {
                statusText.text = "";
            }
            else if (playerCount < 2)
            {
                statusText.text = $"Waiting for players... ({playerCount}/2)";
            }
            else
            {
                statusText.text = "Waiting for players to ready up...";
            }
        }

        // Show/hide countdown panel
        if (countdownPanel != null && statusPanel != null)
        {
            countdownPanel.SetActive(countdownActive);
            statusPanel.SetActive(!countdownActive);
        }

        // Update player panels from server data
        UpdatePlayerPanels();
    }

    private void UpdatePlayerPanels()
    {
        if (LobbyManager.Instance == null) return;

        var playerData = LobbyManager.Instance.GetPlayerData();

        foreach (var kvp in playerData)
        {
            PlayerLobbyData data = kvp.Value;

            // Update correct panel based on player index
            if (data.playerIndex == 0 && player1SetupUI != null)
            {
                player1SetupUI.UpdateFromServerData(data);
            }
            else if (data.playerIndex == 1 && player2SetupUI != null)
            {
                player2SetupUI.UpdateFromServerData(data);
            }
        }
    }

    private void UpdateCountdownDisplay(int secondsRemaining)
    {
        if (countdownText != null)
        {
            if (secondsRemaining > 0)
            {
                countdownText.text = $"Starting in {secondsRemaining}...";
                Debug.Log($"[LobbyUI] Countdown: {secondsRemaining}");
            }
            else
            {
                countdownText.text = "GO!";
                Debug.Log("[LobbyUI] Countdown: GO!");
            }
        }
    }

    private void OnRankingsButtonClicked()
    {
        Debug.Log("[LobbyUI] Rankings button clicked");

        // Show placeholder panel
        if (rankingsPlaceholderPanel != null)
        {
            rankingsPlaceholderPanel.SetActive(true);
        }

        // TODO: Future PHP database integration for rankings
        // Will fetch leaderboard data from server
    }

    /// <summary>
    /// Close rankings placeholder (called by close button)
    /// </summary>
    public void CloseRankingsPlaceholder()
    {
        if (rankingsPlaceholderPanel != null)
        {
            rankingsPlaceholderPanel.SetActive(false);
        }
    }
}
