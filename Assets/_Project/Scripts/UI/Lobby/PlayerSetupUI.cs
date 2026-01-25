/*
====================================================================
* PlayerSetupUI - Individual Player Lobby Setup Panel
====================================================================
* Project: Showroom_Tango (2-Player Top-Down Bullet-Hell)
* Course: PRG - Game & Multimedia Design
* Developer: Julian
* Date: 2025-01-23
* Version: 1.0 - Initial player setup UI
*
* WICHTIG: KOMMENTIERUNG NICHT LOESCHEN!
*
* AUTHORSHIP CLASSIFICATION:
*
* [HUMAN-AUTHORED]
* - UI layout (TANGO 1/2 label, name input, color selection)
* - Name character limit (8 characters)
* - Color preset selection
* - Ready button interaction
*
* [AI-ASSISTED]
* - TMPro InputField integration
* - Color button grid
* - Ready state visual feedback
* - FishNet client-server communication
*
* [AI-GENERATED]
* - Complete UI controller implementation
*
* DEPENDENCIES:
* - TMPro (TextMeshPro UI components)
* - UnityEngine.UI (Button, Image components)
* - LobbyManager (network lobby controller)
*
* NOTES:
* - Owner-only controls (only local player can edit their setup)
* - Real-time synchronization via LobbyManager ServerRpcs
* - Color presets: Magenta, Cyan, Yellow, Green, Red, Blue
====================================================================
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet.Connection;

public class PlayerSetupUI : MonoBehaviour
{
    [Header("Player Identity")]
    [SerializeField] private int playerIndex = 0; // 0 = Player 1, 1 = Player 2
    [SerializeField] private TextMeshProUGUI tangoLabel; // "TANGO 1" or "TANGO 2"

    [Header("Name Input")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private int maxNameLength = 8;

    [Header("Color Selection")]
    [SerializeField] private Button[] colorButtons; // 6 color preset buttons
    [SerializeField] private Image selectedColorIndicator;

    [Header("Ready System")]
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI readyButtonText;
    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color notReadyColor = Color.gray;

    // Preset colors (matches project theme)
    private readonly Color[] colorPresets = new Color[]
    {
        new Color(0.667f, 0f, 0.784f, 1f),  // Magenta
        new Color(0f, 1f, 1f, 1f),           // Cyan
        new Color(1f, 1f, 0f, 1f),           // Yellow
        new Color(0f, 1f, 0f, 1f),           // Green
        new Color(1f, 0f, 0f, 1f),           // Red
        new Color(0f, 0.5f, 1f, 1f)          // Blue
    };

    private Color selectedColor;
    private bool isReady = false;
    private NetworkConnection localConnection;

    void Start()
    {
        // Set TANGO label
        if (tangoLabel != null)
        {
            tangoLabel.text = $"TANGO {playerIndex + 1}";
        }

        // Configure name input
        if (nameInputField != null)
        {
            nameInputField.characterLimit = maxNameLength;
            nameInputField.text = $"Player {playerIndex + 1}";
            nameInputField.onValueChanged.AddListener(OnNameChanged);
        }

        // Setup color buttons
        SetupColorButtons();

        // Setup ready button
        if (readyButton != null)
        {
            readyButton.onClick.AddListener(OnReadyButtonClicked);
        }

        // Default selection
        SelectColor(0); // Default to Magenta

        // Update initial state
        UpdateReadyVisuals();

        Debug.Log($"[PlayerSetupUI] Initialized for Player {playerIndex + 1}");
    }

    private void SetupColorButtons()
    {
        if (colorButtons == null || colorButtons.Length == 0) return;

        for (int i = 0; i < colorButtons.Length && i < colorPresets.Length; i++)
        {
            // Set button color
            Image buttonImage = colorButtons[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = colorPresets[i];
            }

            // Add click listener
            int index = i; // Capture for lambda
            colorButtons[i].onClick.AddListener(() => SelectColor(index));
        }
    }

    private void SelectColor(int colorIndex)
    {
        if (colorIndex < 0 || colorIndex >= colorPresets.Length) return;

        selectedColor = colorPresets[colorIndex];

        // Update visual indicator
        if (selectedColorIndicator != null)
        {
            selectedColorIndicator.color = selectedColor;
        }

        // Send to server
        if (LobbyManager.Instance != null && localConnection != null)
        {
            LobbyManager.Instance.UpdatePlayerColorServerRpc(localConnection, selectedColor);
        }

        Debug.Log($"[PlayerSetupUI] Color selected: {selectedColor}");
    }

    private void OnNameChanged(string newName)
    {
        // Send to server
        if (LobbyManager.Instance != null && localConnection != null)
        {
            LobbyManager.Instance.UpdatePlayerNameServerRpc(localConnection, newName);
        }

        Debug.Log($"[PlayerSetupUI] Name changed: {newName}");
    }

    private void OnReadyButtonClicked()
    {
        if (LobbyManager.Instance != null && localConnection != null)
        {
            LobbyManager.Instance.ToggleReadyServerRpc(localConnection);
            isReady = !isReady;
            UpdateReadyVisuals();

            Debug.Log($"[PlayerSetupUI] Ready state toggled: {isReady}");
        }
    }

    private void UpdateReadyVisuals()
    {
        // Update button text
        if (readyButtonText != null)
        {
            readyButtonText.text = isReady ? "CANCEL" : "READY";
        }

        // Update button color
        if (readyButton != null)
        {
            ColorBlock colors = readyButton.colors;
            colors.normalColor = isReady ? readyColor : notReadyColor;
            readyButton.colors = colors;
        }

        // Disable name/color selection when ready
        if (nameInputField != null)
        {
            nameInputField.interactable = !isReady;
        }

        if (colorButtons != null)
        {
            foreach (Button btn in colorButtons)
            {
                if (btn != null)
                {
                    btn.interactable = !isReady;
                }
            }
        }
    }

    /// <summary>
    /// Set local connection reference (called by LobbyUI)
    /// </summary>
    public void SetLocalConnection(NetworkConnection conn)
    {
        localConnection = conn;
        Debug.Log($"[PlayerSetupUI] Local connection set: {conn.ClientId}");
    }

    /// <summary>
    /// Update display from server data (called by LobbyUI)
    /// </summary>
    public void UpdateFromServerData(PlayerLobbyData data)
    {
        // Update name (without triggering onValueChanged)
        if (nameInputField != null)
        {
            nameInputField.onValueChanged.RemoveListener(OnNameChanged);
            nameInputField.text = data.playerName;
            nameInputField.onValueChanged.AddListener(OnNameChanged);
        }

        // Update color
        selectedColor = data.playerColor;
        if (selectedColorIndicator != null)
        {
            selectedColorIndicator.color = selectedColor;
        }

        // Update ready state
        isReady = data.isReady;
        UpdateReadyVisuals();
    }

    public Color GetSelectedColor() => selectedColor;
    public string GetPlayerName() => nameInputField != null ? nameInputField.text : "";
}
