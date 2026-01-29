# SHOWROOM TANGO - Game Design Document v1.6

**Project:** Showroom Tango (Bullet_Love)
**Type:** 2-Player Cooperative Bullet-Hell
**Platform:** Unity 6000.0.62f1 + FishNet 4.x Networking
**Academic Context:** PRG Course, SRH Hochschule Heidelberg
**Developer:** Julian Gomez
**Last Updated:** January 28, 2026
**Version:** v1.6

---

## 1. EXECUTIVE SUMMARY

### Core Concept
Cooperative top-down bullet-hell shooter where 2 players fight 3 waves of enemies on a shared screen. Neon retrofuturistic aesthetic with auto-fire weapon system.

### Victory Condition
Survive all 3 waves with at least one player alive.

---

## 2. CURRENT IMPLEMENTATION STATUS

### Completed Features
- FishNet multiplayer (Host + Client)
- Single-scene architecture (Menu → Game with LobbyRoot/GameRoot)
- Player movement, rotation, auto-fire weapons
- 2 enemy types (Chaser, Shooter)
- 3-wave spawning system with ObserversRpc client notification
- HP system with damage/death
- Score system with SQL/PHP backend (XAMPP)
- Highscore persistence and display
- Lobby system with player customization (name, color, ready)
- Audio system with preferences
- Story popup

### In Progress
- Wave transition UI (client sync issue)
- Audio clip assignments in Inspector

---

## 3. SCRIPT ARCHITECTURE

### Folder Structure (Audited January 28, 2026)
```
Assets/_Project/Scripts/ (36 scripts, 5,762 lines total)
│
├── Enemies/ (4 scripts, 833 lines)
│   ├── EnemyChaser.cs         - NetworkBehaviour: Follow-player AI
│   ├── EnemyHealth.cs         - NetworkBehaviour: Server-authoritative HP + SyncVar
│   ├── EnemyShooter.cs        - NetworkBehaviour: Ranged AI with bullets
│   └── EnemySpawner.cs        - NetworkBehaviour: Wave system + SyncVar + ObserversRpc
│
├── Gameflow/ (4 scripts, 683 lines)
│   ├── GameStateManager.cs    - NetworkBehaviour: State machine (Lobby/Playing/GameOver/Victory)
│   ├── MenuManager.cs         - MonoBehaviour: Menu scene controller
│   ├── ScoreManager.cs        - NetworkBehaviour: SyncVar + SyncDictionary
│   └── GameAudioManager.cs    - MonoBehaviour: DontDestroyOnLoad singleton
│
├── Network/ (3 scripts, 424 lines)
│   ├── AutoStartNetwork.cs    - MonoBehaviour: FishNet auto-host (dev tool)
│   ├── NetworkUIManager.cs    - MonoBehaviour: Connection UI buttons
│   └── PlayerSpawner.cs       - MonoBehaviour: Player instantiation + lobby data
│
├── Persistence/ (2 scripts, 310 lines)
│   ├── HighscoreManager.cs    - MonoBehaviour: PHP/SQL + JSON fallback
│   └── AudioSettings.cs       - Static class: PlayerPrefs volume persistence
│
├── Player/ (6 scripts, 903 lines)
│   ├── CameraFollow.cs        - NetworkBehaviour: Cinemachine owner tracking
│   ├── NeonGlowController.cs  - MonoBehaviour: Dual-sprite glow visual
│   ├── PlayerController.cs    - NetworkBehaviour: Movement + rotation + SyncVars
│   ├── PlayerHealth.cs        - NetworkBehaviour: HP system + SyncVars
│   ├── WeaponConfig.cs        - ScriptableObject: Weapon data
│   └── WeaponManager.cs       - NetworkBehaviour: Auto-fire multi-weapon
│
├── Projectiles/ (2 scripts, 283 lines)
│   ├── Bullet.cs              - NetworkBehaviour: Projectile behavior
│   └── BulletPool.cs          - MonoBehaviour: FishNet object pooling
│
├── UI/ (10 scripts total)
│   ├── HighscoreUI.cs         - MonoBehaviour: Top 10 display (100 lines)
│   ├── HUDManager.cs          - MonoBehaviour: HP/Score/Wave/GameOver (314 lines)
│   ├── WaveTransitionUI.cs    - MonoBehaviour: Between-wave countdown (131 lines)
│   │
│   ├── Lobby/ (3 scripts, 1,109 lines)
│   │   ├── LobbyManager.cs    - NetworkBehaviour: SyncDict + countdown + ServerRpc
│   │   ├── LobbyUI.cs         - MonoBehaviour: Lobby canvas controller
│   │   └── PlayerSetupUI.cs   - MonoBehaviour: Name/color input panels
│   │
│   └── Menu/ (4 scripts, 650 lines)
│       ├── PreferencesMenu.cs     - MonoBehaviour: Volume sliders UI
│       ├── StoryPopup.cs          - MonoBehaviour: Lore display
│       └── Menu-Title/
│           ├── ButtonHover.cs     - MonoBehaviour: Hover effects (NO HEADER!)
│           └── SeamlessMenuVideo.cs - MonoBehaviour: Background video (407 lines)



### Audio System Architecture (v1.6)
```
GameAudioManager (DontDestroyOnLoad singleton)
├── Music Tracks: Menu, Gameplay
├── SFX: Button clicks
├── States: Menu → Lobby → Playing
└── Volume: Via AudioSettings.cs (PlayerPrefs)

AudioSettings.cs (Static utility)
├── MasterVolume, MusicVolume, SFXVolume
├── FinalMusicVolume = Master × Music
├── PlayerPrefs persistence
└── ApplySettings() → GameAudioManager

PreferencesMenu.cs (UI Controller)
├── 3 sliders (Master, Music, SFX)
├── Real-time volume adjustment
└── CanvasGroup visibility pattern
```

---

## 4. NETWORK ARCHITECTURE

### FishNet Patterns Used
- **SyncVar:** playerName, currentHP, score, wave count
- **SyncDictionary:** Player lobby data
- **ServerRpc:** Player actions (fire, ready, name change)
- **ObserversRpc:** Wave cleared notification, game state changes

### Scene Flow
```
Menue.unity → LoadScene("Game") → Game.unity
                                   ├── LobbyRoot (active during Lobby)
                                   └── GameRoot (active during Playing)
```

---

## 5. SCENES

### Menue.unity
- Menu canvas with Start, Story, Preferences buttons
- GameAudioManager (first instance, persists)
- Video background

### Game.unity
- NetworkManager
- LobbyRoot (player setup panels)
- GameRoot (play area, enemies, HUD)
- GameStateManager controls visibility

---

## 6. KNOWN ISSUES

### Open - TIER 1 (Grade-Critical)
1. **

2. **Projectile spawn position** - Bullets spawn behind player
   - Root cause: firePointOffset applied in world space, not local
   - Fix location: WeaponManager.cs line 116
   - Required: `playerTransform.TransformDirection(offset)`

### Open - TIER 2
3. **
4. **Umlaute bug** - ae/oe/ue encoding issues
5. **


---

## 7. GRADING REQUIREMENTS MAPPING

| Category | Points | Status |
|----------|--------|--------|
| Multiplayer Basis | 10/10 | DONE |
| Player Control | 15/15 | DONE |
| Shooting System | 20/20 | DONE |
| Enemy System | 15/15 | DONE |
| Health & Gameflow | 10/10 | DONE |
| HUD & Score | 10/10 | DONE |
| **Base Total** | **80/80** | **DONE** |
| Lobby & Matchmaking | 5-10 | DONE |
| Extended Persistence | 5-10 | DONE (SQL/PHP) |
| Visual/Technical | 5-10 | PARTIAL |
| **Estimated Total** | **~90/100** | |

---

## 8. VERSION HISTORY

### v1.6.1 (January 28, 2026)
- Full codebase audit: 36 scripts, 5,762 lines analyzed
- Header compliance review: 22 complete, 10 partial, 4 missing
- Bug root cause analysis documented for Wave UI + Projectile spawn
- README.md completely rewritten to match v1.6 architecture
- GDD updated with script statistics and line counts
- OpenTasks updated with detailed debugging steps

### v1.6 (January 28, 2026)
- Audio system consolidated (GameAudioManager with DontDestroyOnLoad)
- Deprecated MenuAudioController, PreferencesManager → Z-Parkplatz
- Script folder structure reorganized
- Preferences popup CanvasGroup fix
- Highscore name submission fix
- Wave transition ObserversRpc implementation

### v1.5 (January 26, 2026)
- Single-scene architecture (Lobby + Game merged)
- Event-driven lobby initialization
- BulletPool refactored to MonoBehaviour

### v1.4 (January 21, 2026)
- Wave system reduced to 3 waves
- Restart button network fix
- Enemy targeting AI improvements

---

## 9. CREDITS

**Developer:** Julian Gomez
**Course:** PRG - Game & Multimedia Design
**Institution:** SRH Hochschule Heidelberg

**Technology:**
- Unity 6000.0.62f1
- FishNet 4.x
- XAMPP (PHP/MySQL)

**Assets:**
- Sprites: Kenney Space Shooter Redux (CC0)
- Font: Electronic Highway Sign

**AI Assistance:**
- Claude (Anthropic) - Technical consultation
- AI-assisted sections documented in code headers

---

*END OF DOCUMENT v1.6*
