# Lobby System Setup Guide

**Project:** Showroom_Tango v2.0
**Date:** 2025-01-23
**Feature:** Player Customization & Lobby System

---

## 📋 Overview

The lobby system allows 2 players to:
- Choose custom names (max 8 characters)
- Select player colors (6 presets)
- Ready up before game starts
- See 3-second countdown before Game scene loads

**Scene Flow:**
```
Menue → Lobby → Game → Victory/GameOver → Lobby (restart)
```

---

## 🎯 Scripts Created

### **1. LobbyManager.cs** (`Assets/_Project/Scripts/UI/Lobby/`)
**Purpose:** Server-authority lobby controller
**Key Features:**
- Player registration (max 2 players)
- Name/color synchronization via `SyncDictionary<int, PlayerLobbyData>`
- Ready-up system
- Countdown trigger when both players ready
- Scene transition to Game

**Network Methods:**
- `RegisterPlayerServerRpc()` - Register player in lobby
- `UpdatePlayerNameServerRpc()` - Sync name changes
- `UpdatePlayerColorServerRpc()` - Sync color changes
- `ToggleReadyServerRpc()` - Handle ready button

**Events:**
- `OnLobbyStateChanged` - Fires when any player data changes
- `OnCountdownTick` - Fires each countdown second (3, 2, 1)

---

### **2. PlayerSetupUI.cs** (`Assets/_Project/Scripts/UI/Lobby/`)
**Purpose:** Individual player panel controller
**Key Features:**
- Static label: "TANGO 1" or "TANGO 2"
- Name input field (8 char limit)
- 6 color preset buttons
- Ready button with state toggle
- Disables controls when ready

**Inspector Fields:**
- `playerIndex` (0 = Player 1, 1 = Player 2)
- `tangoLabel` - TextMeshProUGUI
- `nameInputField` - TMP_InputField
- `colorButtons[6]` - Button array
- `selectedColorIndicator` - Image
- `readyButton` - Button
- `readyButtonText` - TextMeshProUGUI
- `readyIndicator` - GameObject

**Color Presets:**
```csharp
Magenta: RGB(170, 0, 200)
Cyan:    RGB(0, 255, 255)
Yellow:  RGB(255, 255, 0)
Green:   RGB(0, 255, 0)
Red:     RGB(255, 0, 0)
Blue:    RGB(0, 128, 255)
```

---

### **3. LobbyUI.cs** (`Assets/_Project/Scripts/UI/Lobby/`)
**Purpose:** Main lobby UI orchestrator
**Key Features:**
- Manages 2 PlayerSetupUI panels
- Displays status ("Waiting for players..." or "Waiting for ready up...")
- Shows countdown ("Starting in 3... 2... 1...")
- Rankings button placeholder

**Inspector Fields:**
- `player1SetupUI` - PlayerSetupUI component
- `player2SetupUI` - PlayerSetupUI component
- `statusPanel` - GameObject
- `statusText` - TextMeshProUGUI
- `countdownPanel` - GameObject (hidden initially)
- `countdownText` - TextMeshProUGUI (magenta color)
- `rankingsButton` - Button
- `rankingsPlaceholderPanel` - GameObject (future feature)

---

## 🔧 Unity Setup Instructions

### **Step 1: Lobby Scene Hierarchy**

Create this structure in `Lobby.unity`:

```
Lobby (Scene)
├─ NetworkManager (DontDestroyOnLoad)
│  └─ LobbyManager component
│
├─ Canvas (Screen Space - Overlay)
│  ├─ LobbyUI component
│  │
│  ├─ Player1Panel (Left side)
│  │  ├─ PlayerSetupUI component (playerIndex = 0)
│  │  ├─ TangoLabel (TextMeshProUGUI: "TANGO 1")
│  │  ├─ NameInputField (TMP_InputField, limit: 8)
│  │  ├─ ColorButtonsGrid (GridLayoutGroup)
│  │  │  ├─ ColorButton1 (Image: Magenta)
│  │  │  ├─ ColorButton2 (Image: Cyan)
│  │  │  ├─ ColorButton3 (Image: Yellow)
│  │  │  ├─ ColorButton4 (Image: Green)
│  │  │  ├─ ColorButton5 (Image: Red)
│  │  │  └─ ColorButton6 (Image: Blue)
│  │  ├─ SelectedColorIndicator (Image)
│  │  ├─ ReadyButton (Button)
│  │  │  └─ ReadyButtonText (TextMeshProUGUI: "READY")
│  │  └─ ReadyIndicator (GameObject, hidden)
│  │
│  ├─ Player2Panel (Right side) - Same structure, playerIndex = 1
│  │
│  ├─ StatusPanel (Center)
│  │  └─ StatusText (TextMeshProUGUI)
│  │
│  ├─ CountdownPanel (Center, inactive)
│  │  └─ CountdownText (TextMeshProUGUI, color: #AA00C8)
│  │
│  └─ RankingsButton (Bottom)
│
└─ EventSystem
```

---

### **Step 2: Component Configuration**

#### **LobbyManager (NetworkManager GameObject)**
```
Max Players: 2
Countdown Duration: 3
Game Scene Name: "Game"
```

#### **Player1SetupUI (Player1Panel)**
```
Player Index: 0
Tango Label: → TangoLabel (TextMeshProUGUI)
Name Input Field: → NameInputField (TMP_InputField)
Color Buttons[6]: → Drag all 6 ColorButton GameObjects
Selected Color Indicator: → SelectedColorIndicator (Image)
Ready Button: → ReadyButton
Ready Button Text: → ReadyButtonText
Ready Indicator: → ReadyIndicator (GameObject)
Max Name Length: 8
Ready Color: Green (0, 255, 0)
Not Ready Color: Gray (128, 128, 128)
```

#### **Player2SetupUI (Player2Panel)**
- Same as Player1, but set `Player Index: 1`

#### **LobbyUI (Canvas)**
```
Player 1 Setup UI: → Player1Panel (PlayerSetupUI component)
Player 2 Setup UI: → Player2Panel (PlayerSetupUI component)
Status Panel: → StatusPanel
Status Text: → StatusText
Countdown Panel: → CountdownPanel
Countdown Text: → CountdownText
Rankings Button: → RankingsButton
Rankings Placeholder Panel: → (Optional, for future)
Countdown Color: #AA00C8 (Magenta)
```

---

### **Step 3: Button Configuration**

#### **Color Buttons (Each of 6)**
```
Button → onClick:
  - Add Listener (auto-assigned by PlayerSetupUI.SetupColorButtons())

Image Component:
  - ColorButton1: Color = Magenta (#AA00C8)
  - ColorButton2: Color = Cyan (#00FFFF)
  - ColorButton3: Color = Yellow (#FFFF00)
  - ColorButton4: Color = Green (#00FF00)
  - ColorButton5: Color = Red (#FF0000)
  - ColorButton6: Color = Blue (#0080FF)
```

#### **Ready Button**
```
Button → onClick:
  - (auto-assigned by PlayerSetupUI.Start())
```

#### **Rankings Button**
```
Button → onClick:
  - Target: LobbyUI
  - Function: LobbyUI.OnRankingsButtonClicked()
```

---

### **Step 4: Build Settings**

Add scenes in this order:
```
1. Menue
2. Lobby  ← ADD THIS
3. Game
```

**Path:** File → Build Settings → Add Open Scenes

---

## 🔄 Scene Transition Flow

### **Menu → Lobby**
**Trigger:** "Start Game" button in Menue scene
**Script:** `MenuManager.StartGame()` → `SceneManager.LoadScene("Lobby")`

### **Lobby → Game**
**Trigger:** Both players ready + 3s countdown
**Script:** `LobbyManager.StartGameCountdown()` → `SceneManager.LoadScene("Game")`

### **Game → Lobby (Restart)**
**Trigger:** Restart button in Victory/GameOver screen
**Script:** `HUDManager.OnRestartButtonPressed()` → `GameStateManager.RequestRestartServerRpc()` → `SceneManager.LoadScene("Lobby")`

---

## 🎮 Player Data Persistence

### **Lobby → Game Transfer**

When Game scene loads, player data (name, color) needs to be applied to spawned Player prefabs.

**Option A: PlayerController Integration (Recommended)**

PlayerController already has SyncVars:
```csharp
private readonly SyncVar<string> playerName;
private readonly SyncVar<Color> playerColor;
```

Add to `PlayerController.OnStartClient()`:
```csharp
// Get data from LobbyManager
if (LobbyManager.Instance != null && IsOwner)
{
    var lobbyData = LobbyManager.Instance.GetPlayerData();
    if (lobbyData.TryGetValue(LocalConnection.ClientId, out PlayerLobbyData data))
    {
        SetPlayerNameServerRpc(data.playerName);
        SetPlayerColorServerRpc(data.playerColor);
    }
}
```

**Option B: Separate PlayerLobbyDataTransfer Script**

Create script to transfer data on Game scene load (if needed).

---

## 🐛 Testing Checklist

### **Lobby Scene**
- [ ] Both player panels visible
- [ ] TANGO 1 / TANGO 2 labels correct
- [ ] Name input works (max 8 chars)
- [ ] Color buttons change selection
- [ ] Ready button toggles state
- [ ] Ready indicator shows/hides
- [ ] Status text updates ("Waiting...")
- [ ] Countdown appears when both ready
- [ ] Countdown shows "3... 2... 1... GO!"
- [ ] Game scene loads after countdown

### **Network Sync**
- [ ] Player 1 name changes sync to server
- [ ] Player 2 name changes sync to server
- [ ] Color changes sync
- [ ] Ready states sync
- [ ] Countdown visible on both clients

### **Restart Flow**
- [ ] Victory screen shows restart button
- [ ] Restart button loads Lobby scene
- [ ] Players can re-customize
- [ ] Second game starts correctly

---

## 📊 Network Events Flow

```
Player joins Lobby scene
  ↓
LobbyManager.RegisterPlayerServerRpc()
  ↓
playerDataDict updated (server)
  ↓
SyncDictionary.OnChange → all clients notified
  ↓
LobbyUI.UpdateLobbyDisplay() (client)
  ↓
PlayerSetupUI.UpdateFromServerData() (client)
  ↓
UI updates with server data
```

---

## 🚀 Future Enhancements

### **Rankings Button Integration**
**Current:** Placeholder panel
**Future:** PHP database integration

**Implementation:**
1. Create web server endpoint: `GET /api/rankings`
2. Add `UnityWebRequest` call in `LobbyUI.OnRankingsButtonClicked()`
3. Parse JSON response
4. Display in scrollable leaderboard UI

**Example API Response:**
```json
{
  "rankings": [
    {"rank": 1, "name": "Player1", "score": 5000},
    {"rank": 2, "name": "Player2", "score": 4500}
  ]
}
```

---

## ⚠️ Known Limitations

1. **Player Limit:** Hardcoded to 2 players
2. **No Reconnect:** If player disconnects in lobby, restart required
3. **No Kick Function:** Cannot remove AFK players
4. **Color Conflicts:** Two players can select same color (no validation)

---

## 📝 Script Dependencies

```
LobbyManager.cs
  ├─ Requires: FishNet.Object.NetworkBehaviour
  ├─ Requires: FishNet.Object.Synchronizing.SyncDictionary
  └─ Requires: UnityEngine.SceneManagement

PlayerSetupUI.cs
  ├─ Requires: TMPro (TextMeshPro)
  ├─ Requires: UnityEngine.UI
  └─ Requires: LobbyManager (runtime)

LobbyUI.cs
  ├─ Requires: TMPro
  ├─ Requires: PlayerSetupUI (x2)
  ├─ Requires: LobbyManager (runtime)
  └─ Requires: FishNet.Managing.NetworkManager
```

---

## ✅ Implementation Checklist

- [x] LobbyManager.cs created
- [x] PlayerSetupUI.cs created
- [x] LobbyUI.cs created
- [x] MenuManager updated (Menu → Lobby)
- [x] GameStateManager updated (Restart → Lobby)
- [ ] Lobby.unity scene built in Unity
- [ ] NetworkManager in Lobby scene
- [ ] PlayerSetupUI panels configured
- [ ] LobbyUI references assigned
- [ ] Color buttons set up
- [ ] Build settings updated
- [ ] PlayerController data transfer (optional)
- [ ] Network testing (2 clients)
- [ ] Restart flow testing

---

**End of Setup Guide**
**Version:** 1.0
**Last Updated:** 2025-01-23
