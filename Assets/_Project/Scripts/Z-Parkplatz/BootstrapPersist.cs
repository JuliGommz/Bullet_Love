/*
====================================================================
* BootstrapPersist - NetworkManager Scene Persistence
====================================================================
* Project: Showroom_Tango (2-Player Top-Down Bullet-Hell)
* Course: PRG - Game & Multimedia Design
* Developer: Julian
* Date: 17.12.2025
* Version: 1.1
* 
* WICHTIG: KOMMENTIERUNG NICHT LOESCHEN!
* Diese detaillierte Authorship-Dokumentation ist fuer die akademische
* Bewertung erforderlich und darf nicht entfernt werden!
* 
* AUTHORSHIP CLASSIFICATION:
* 
* [HUMAN-AUTHORED]
* - Bootstrap pattern selection
* - DontDestroyOnLoad strategy
* 
* [AI-ASSISTED]
* - Initial implementation
* - Academic header formatting
* 
* [AI-GENERATED]
* - None
* 
* DEPENDENCIES:
* - UnityEngine (Unity Core)
* 
* NOTES:
* - Ensures NetworkManager survives scene reloads
* - Prevents connection loss during scene transitions
* - Must be attached to NetworkManager GameObject
* - Bootstrap scene pattern (persistent scene approach)
====================================================================
*/

using UnityEngine;

public class BootstrapPersist : MonoBehaviour
{
    void Awake()
    {
        // Prevent NetworkManager destruction during scene loads
        DontDestroyOnLoad(gameObject);
    }
}
