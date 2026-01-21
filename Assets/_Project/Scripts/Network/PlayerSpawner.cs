/*
====================================================================
* PlayerSpawner - Automatic Player Spawning on Connection
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
* - Auto-spawn strategy on connection
* - Spawn point positioning
* 
* [AI-ASSISTED]
* - FishNet server connection event handling
* - Server-authority spawn pattern
* - Academic header formatting
* 
* [AI-GENERATED]
* - None
* 
* DEPENDENCIES:
* - FishNet.Managing.NetworkManager
* - FishNet.Connection.NetworkConnection
* 
* NOTES:
* - Spawns player automatically when client connects
* - Server-authority spawning (ADR-009)
* - Each connection gets their own player instance
* - Spawn positions offset for visibility
====================================================================
*/

using FishNet.Managing;
using FishNet.Connection;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 spawnOffset = new Vector3(2f, 0f, 0f);
    
    private NetworkManager networkManager;
    private int spawnedPlayers = 0;

    void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        
        if (networkManager == null)
        {
            Debug.LogError("PlayerSpawner: NetworkManager not found on same GameObject!");
            return;
        }
        
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerSpawner: Player Prefab not assigned!");
            return;
        }
        
        // Subscribe to client connection events
        networkManager.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
    }

    void OnDestroy()
    {
        if (networkManager != null)
        {
            networkManager.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
        }
    }

    private void OnRemoteConnectionState(NetworkConnection conn, FishNet.Transporting.RemoteConnectionStateArgs args)
    {
        // Only spawn when client connects
        if (args.ConnectionState != FishNet.Transporting.RemoteConnectionState.Started)
            return;
        
        // Calculate spawn position (offset each player)
        Vector3 spawnPosition = spawnOffset * spawnedPlayers;
        
        // Spawn player prefab
        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        
        // Give ownership to the connecting client
        networkManager.ServerManager.Spawn(playerInstance, conn);
        
        spawnedPlayers++;
        
        Debug.Log($"PlayerSpawner: Spawned player for connection {conn.ClientId} at position {spawnPosition}");
    }
}
