/*
====================================================================
* NetworkUIManager - UI Controller for Host/Client Network Connections
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
* - UI toggle pattern design (Start/Stop functionality)
* - Color feedback strategy
* - Test-friendly approach (buttons remain visible)
* 
* [AI-ASSISTED]
* - Initial implementation structure
* - Button reference caching in Awake()
* - Null-safety validation improvements
* - Academic header formatting
* 
* [AI-GENERATED]
* - None
* 
* DEPENDENCIES:
* - FishNet.Managing.NetworkManager (FishNet package)
* - UnityEngine.UI.Button (Unity UI)
* - TMPro.TMP_Text (TextMeshPro)
* 
* NOTES:
* - Buttons remain visible for testing purposes
* - Color change provides visual feedback for connection state
* - Stop functionality allows reconnection without restart
====================================================================
*/

using FishNet.Managing;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkUIManager : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Color connectedColor = Color.green;
    [SerializeField] private Color disconnectedColor = Color.white;
    
    private TMP_Text hostButtonText;
    private TMP_Text clientButtonText;
    private Image hostButtonImage;
    private Image clientButtonImage;
    private bool hostRunning = false;
    private bool clientRunning = false;

    void Awake()
    {
        // Get references to button texts and images with null checks
        if (hostButton != null)
        {
            hostButtonText = hostButton.GetComponentInChildren<TMP_Text>();
            hostButtonImage = hostButton.GetComponent<Image>();
            
            if (hostButtonText == null)
            {
                Debug.LogError("NetworkUIManager: Host button text component not found!");
            }
            if (hostButtonImage == null)
            {
                Debug.LogError("NetworkUIManager: Host button image component not found!");
            }
        }
        else
        {
            Debug.LogError("NetworkUIManager: Host button reference is null!");
        }
        
        if (clientButton != null)
        {
            clientButtonText = clientButton.GetComponentInChildren<TMP_Text>();
            clientButtonImage = clientButton.GetComponent<Image>();
            
            if (clientButtonText == null)
            {
                Debug.LogError("NetworkUIManager: Client button text component not found!");
            }
            if (clientButtonImage == null)
            {
                Debug.LogError("NetworkUIManager: Client button image component not found!");
            }
        }
        else
        {
            Debug.LogError("NetworkUIManager: Client button reference is null!");
        }
        
        if (networkManager == null)
        {
            Debug.LogError("NetworkUIManager: NetworkManager reference is null!");
        }
    }

    public void ToggleHost()
    {
        if (!hostRunning)
        {
            StartHost();
        }
        else
        {
            StopHost();
        }
    }

    public void ToggleClient()
    {
        if (!clientRunning)
        {
            StartClient();
        }
        else
        {
            StopClient();
        }
    }

    private void StartHost()
    {
        if (networkManager != null)
        {
            networkManager.ServerManager.StartConnection();
            networkManager.ClientManager.StartConnection();
            Debug.Log("Host started!");
            
            hostRunning = true;
            if (hostButtonText != null) 
            {
                hostButtonText.text = "Stop Host";
            }
            if (hostButtonImage != null) 
            {
                hostButtonImage.color = connectedColor;
            }
        }
    }

    private void StopHost()
    {
        if (networkManager != null)
        {
            networkManager.ServerManager.StopConnection(true);
            networkManager.ClientManager.StopConnection();
            Debug.Log("Host stopped!");
            
            hostRunning = false;
            if (hostButtonText != null) 
            {
                hostButtonText.text = "Start Host";
            }
            if (hostButtonImage != null) 
            {
                hostButtonImage.color = disconnectedColor;
            }
        }
    }

    private void StartClient()
    {
        if (networkManager != null)
        {
            networkManager.ClientManager.StartConnection();
            Debug.Log("Client started!");
            
            clientRunning = true;
            if (clientButtonText != null) 
            {
                clientButtonText.text = "Stop Client";
            }
            if (clientButtonImage != null) 
            {
                clientButtonImage.color = connectedColor;
            }
        }
    }

    private void StopClient()
    {
        if (networkManager != null)
        {
            networkManager.ClientManager.StopConnection();
            Debug.Log("Client stopped!");
            
            clientRunning = false;
            if (clientButtonText != null) 
            {
                clientButtonText.text = "Start Client";
            }
            if (clientButtonImage != null) 
            {
                clientButtonImage.color = disconnectedColor;
            }
        }
    }
}
