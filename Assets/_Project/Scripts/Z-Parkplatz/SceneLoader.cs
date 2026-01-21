/*
====================================================================
* SceneLoader - Network Scene Management
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
* - Scene transition strategy
* - Load on connection approach
* 
* [AI-ASSISTED]
* - FishNet SceneManager integration
* - Server-authority scene loading
* - Academic header formatting
* 
* [AI-GENERATED]
* - None
* 
* DEPENDENCIES:
* - FishNet.Managing.Scened (FishNet Scene Management)
* - UnityEngine.SceneManagement
* 
* NOTES:
* - Loads Game scene when server starts
* - Server controls scene transitions (authority)
* - Clients follow server's scene automatically
====================================================================
*/

using FishNet.Managing.Scened;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    
    private FishNet.Managing.NetworkManager networkManager;

    void Awake()
    {
        networkManager = GetComponent<FishNet.Managing.NetworkManager>();
        
        if (networkManager == null)
        {
            Debug.LogError("SceneLoader: NetworkManager not found on same GameObject!");
            return;
        }
        
        // Subscribe to server start event
        networkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
    }

    void OnDestroy()
    {
        if (networkManager != null)
        {
            networkManager.ServerManager.OnServerConnectionState -= OnServerConnectionState;
        }
    }

    private void OnServerConnectionState(FishNet.Transporting.ServerConnectionStateArgs args)
    {
        // Only load scene when server starts
        if (args.ConnectionState == FishNet.Transporting.LocalConnectionState.Started)
        {
            LoadGameScene();
        }
    }

    private void LoadGameScene()
    {
        if (networkManager == null) return;
        
        // Use FishNet's scene manager for network scene loading
        SceneLoadData sld = new SceneLoadData(gameSceneName);
        sld.ReplaceScenes = ReplaceOption.None; // Keep Bootstrap scene loaded
        
        networkManager.SceneManager.LoadGlobalScenes(sld);
        
        Debug.Log($"SceneLoader: Loading game scene '{gameSceneName}'");
    }
}
