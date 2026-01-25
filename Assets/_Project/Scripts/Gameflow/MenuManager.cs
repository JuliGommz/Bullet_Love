/*
====================================================================
* MenuManager - Main Menu Controller
====================================================================
* Project: Showroom_Tango (2-Player Top-Down Bullet-Hell)
* Course: PRG - Game & Multimedia Design
* Developer: Julian
* Date: 2025-01-23
* Version: 1.1 - Updated scene flow (Menu → Lobby → Game)
*
* WICHTIG: KOMMENTIERUNG NICHT LOESCHEN!
*
* AUTHORSHIP CLASSIFICATION:
*
* [HUMAN-AUTHORED]
* - Menu layout (Story popup, Preferences)
* - Scene flow update (Lobby integration)
*
* [AI-ASSISTED]
* - Scene loading logic
* - Popup management
*
* NOTES:
* - Start Game button loads Lobby scene (player setup)
* - Lobby scene handles network connection and player customization
* - Game scene loads after both players ready
====================================================================
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject storyPopup;
    public GameObject preferencesPopup;

    /// <summary>
    /// Start Game - Load Lobby scene for player setup
    /// </summary>
    public void StartGame()
    {
        Debug.Log("[MenuManager] Loading Lobby scene...");
        SceneManager.LoadScene("Lobby");
    }

    /// <summary>
    /// Show story/lore popup
    /// </summary>
    public void ShowStoryPopup()
    {
        if (storyPopup != null)
        {
            storyPopup.SetActive(true);
            Debug.Log("[MenuManager] Story popup opened");
        }
    }

    /// <summary>
    /// Show preferences/settings popup
    /// </summary>
    public void ShowPreferencesPopup()
    {
        if (preferencesPopup != null)
        {
            preferencesPopup.SetActive(true);
            Debug.Log("[MenuManager] Preferences popup opened");
        }
    }

    /// <summary>
    /// Close any popup
    /// </summary>
    public void ClosePopup(GameObject popup)
    {
        if (popup != null)
        {
            popup.SetActive(false);
            Debug.Log($"[MenuManager] Closed popup: {popup.name}");
        }
    }
}
