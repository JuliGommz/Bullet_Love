/*
====================================================================
* CameraFollow - Cinemachine Camera Target Assignment
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
* - Cinemachine integration strategy
* - Owner-only camera follow logic
* 
* [AI-ASSISTED]
* - NetworkBehaviour pattern
* - Virtual Camera target assignment
* - Null-safety checks
* - Academic header formatting
* 
* [AI-GENERATED]
* - None
* 
* DEPENDENCIES:
* - FishNet.Object.NetworkBehaviour (FishNet package)
* - Unity.Cinemachine (Cinemachine package)
* 
* NOTES:
* - Attached to Player Prefab
* - Only owner's camera follows their player
* - Uses Cinemachine Virtual Camera for smooth follow
* - Non-owners ignore camera assignment
====================================================================
*/

using FishNet.Object;
using UnityEngine;
using Unity.Cinemachine;

public class CameraFollow : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        // Only set camera target for owner
        if (!IsOwner) return;
        
        // Find the Cinemachine Virtual Camera in scene
        CinemachineCamera virtualCamera = FindFirstObjectByType<CinemachineCamera>();
        
        if (virtualCamera != null)
        {
            // Set this player as the camera's follow target
            virtualCamera.Follow = transform;
            Debug.Log($"Camera now following owner player: {gameObject.name}");
        }
        else
        {
            Debug.LogError("CameraFollow: Cinemachine Virtual Camera not found in scene!");
        }
    }
}
