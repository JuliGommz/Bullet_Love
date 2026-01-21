/*
====================================================================
* PlayerController - Player Movement and Network Synchronization
====================================================================
* Project: Showroom_Tango (2-Player Top-Down Bullet-Hell)
* Course: PRG - Game & Multimedia Design
* Developer: Julian
* Date: 2025-01-20
* Version: 1.3 - Removed manual shooting (uses WeaponManager auto-fire)
*
* WICHTIG: KOMMENTIERUNG NICHT LOESCHEN!
* Diese detaillierte Authorship-Dokumentation ist fuer die akademische
* Bewertung erforderlich und darf nicht entfernt werden!
*
* AUTHORSHIP CLASSIFICATION:
*
* [HUMAN-AUTHORED]
* - Component-First architecture (Prefab created before script)
* - Owner-only input handling strategy
* - SyncVar selection (PlayerName, CurrentHP, PlayerColor)
* - Mouse rotation approach
*
* [AI-ASSISTED]
* - NetworkBehaviour implementation
* - Input System integration (Unity New Input System)
* - Server-authority movement pattern
* - FishNet 4.x SyncVar<T> syntax
* - Academic header formatting
* - Dead code removal (manual shooting system)
*
* [AI-GENERATED]
* - None
*
* DEPENDENCIES:
* - FishNet.Object.NetworkBehaviour (FishNet package)
* - FishNet.Object.Synchronizing.SyncVar<T> (FishNet 4.x)
* - UnityEngine.InputSystem (Unity Input System package)
* - Rigidbody2D (Unity Physics2D)
* - WeaponManager (handles shooting via auto-fire)
*
* NOTES:
* - Uses Unity New Input System (ADR-007)
* - Server-authority for position (ADR-009)
* - Owner-only input processing (FishNet [IsOwner])
* - Rotation follows mouse position in world space
* - FishNet 4.x syntax: SyncVar<Type> instead of [SyncVar] attribute
* - Color control removed (handled by NeonGlowController)
* - Shooting removed (handled by WeaponManager auto-fire system)
====================================================================
*/

using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Synchronized Network Variables")]
    private readonly SyncVar<string> playerName = new SyncVar<string>("Player");
    private readonly SyncVar<int> currentHP = new SyncVar<int>(100);
    private readonly SyncVar<Color> playerColor = new SyncVar<Color>(Color.white);

    // Component references
    private Rigidbody2D rb;
    private Camera mainCamera;
    private PlayerHealth playerHealth;

    // Input System references
    private PlayerInput playerInput;
    private InputAction moveAction;

    // Movement state
    private Vector2 moveInput;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        // Subscribe to SyncVar changes
        playerName.OnChange += OnPlayerNameChanged;
        currentHP.OnChange += OnHPChanged;
        playerColor.OnChange += OnColorChanged;
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();

        // Unsubscribe from SyncVar changes
        playerName.OnChange -= OnPlayerNameChanged;
        currentHP.OnChange -= OnHPChanged;
        playerColor.OnChange -= OnColorChanged;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Only setup for owner
        if (!IsOwner)
        {
            // Disable PlayerInput component for non-owners
            PlayerInput input = GetComponent<PlayerInput>();
            if (input != null)
            {
                input.enabled = false;
            }
            return;
        }

        // Owner-specific initialization
        InitializeInput();
    }

    void Awake()
    {
        // Get component references with null checks
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();

        if (rb == null)
        {
            Debug.LogError("PlayerController: Rigidbody2D component not found!");
        }

        if (playerHealth == null)
        {
            Debug.LogError("PlayerController: PlayerHealth component not found!");
        }
    }

    void Start()
    {
        // Get main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("PlayerController: Main Camera not found!");
        }
    }

    private void InitializeInput()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Movement"];

            if (moveAction == null)
            {
                Debug.LogError("PlayerController: Movement action not found in Input Actions!");
            }
        }
        else
        {
            Debug.LogError("PlayerController: PlayerInput component not found!");
        }
    }

    void Update()
    {
        // Only process input for owner
        if (!IsOwner) return;

        // Read input
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }

        // Handle rotation toward mouse
        RotateTowardsMouse();
    }

    void FixedUpdate()
    {
        // Only process movement for owner
        if (!IsOwner) return;

        // Apply movement
        if (rb != null)
        {
            Vector2 movement = moveInput * moveSpeed;
            rb.linearVelocity = movement;

            // Clamp position to boundaries
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -15f, 15f);
            pos.y = Mathf.Clamp(pos.y, -10f, 10f);
            transform.position = pos;
        }
    }

    private void RotateTowardsMouse()
    {
        if (mainCamera == null) return;

        // Get mouse position in world space
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // Calculate direction
        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        // Calculate angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation (subtract 90 because sprite faces up by default)
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    // SyncVar change callbacks
    private void OnPlayerNameChanged(string prev, string next, bool asServer)
    {
        Debug.Log($"Player name changed: {prev} -> {next}");
    }

    private void OnHPChanged(int prev, int next, bool asServer)
    {
        Debug.Log($"Player HP changed: {prev} -> {next}");
    }

    private void OnColorChanged(Color prev, Color next, bool asServer)
    {
        // Color now handled by NeonGlowController - do not override
    }

    // Server-authority methods for setting synchronized variables
    [ServerRpc]
    private void SetPlayerColorServerRpc(Color color)
    {
        playerColor.Value = color;
    }

    [ServerRpc]
    public void SetPlayerNameServerRpc(string name)
    {
        playerName.Value = name;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        // Forward directly to PlayerHealth (no duplicate HP tracking)
        if (playerHealth != null)
        {
            playerHealth.ApplyDamage(damage);
        }
    }

    // Public getters for other systems
    public string GetPlayerName() => playerName.Value;
    public int GetCurrentHP() => currentHP.Value;
    public Color GetPlayerColor() => playerColor.Value;
}