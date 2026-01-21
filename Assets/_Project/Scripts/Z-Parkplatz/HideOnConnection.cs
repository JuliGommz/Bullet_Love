/*
====================================================================
* HideOnConnection - Hide UI Canvas when game starts
====================================================================
* Project: Showroom_Tango (2-Player Top-Down Bullet-Hell)
* Course: PRG - Game & Multimedia Design
* Developer: Julian
* Date: 18.12.2025
* Version: 1.0
* 
* WICHTIG: KOMMENTIERUNG NICHT LOESCHEN!
* Diese detaillierte Authorship-Dokumentation ist fuer die akademische
* Bewertung erforderlich und darf nicht entfernt werden!
* 
* AUTHORSHIP CLASSIFICATION:
* 
* [HUMAN-AUTHORED]
* - Hide UI strategy
* 
* [AI-ASSISTED]
* - Scene load detection
* - Canvas deactivation timing
* 
* [AI-GENERATED]
* - None
* 
* DEPENDENCIES:
* - UnityEngine.SceneManagement
* 
* NOTES:
* - Hides Canvas when Game scene loads
* - Allows UI to be visible during connection setup
====================================================================
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class HideOnConnection : MonoBehaviour
{
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Hide UI when Game scene loads
        if (scene.name == "Game")
        {
            gameObject.SetActive(false);
            Debug.Log("HideOnConnection: Canvas hidden after Game scene loaded");
        }
    }
}
