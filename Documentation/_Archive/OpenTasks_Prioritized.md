# Showroom Tango - Open Tasks & Priority List
**Last Updated:** 2026-01-29
**Status:** Active Development
**Version:** v1.6.1
**Codebase Stats:** 36 scripts, 5,762 lines, 12 NetworkBehaviours

---

## TIER 1 - URGENT (Grade-critical)

| # | Task | Status | Notes |
|---|------|--------|-------|
| 1 | SQL/PHP Highscore Backend | DONE | Deployed XAMPP + PHP, DB created, URLs fixed, score submission integrated |
| 2 | Projectile spawn position bug | DONE | Fixed: TransformDirection for local offset rotation |
| 3 | Music Manager | DONE | GameAudioManager with DontDestroyOnLoad, 3 states (Menu/Lobby/Playing) |
| 4 | README.md update | DONE | Updated to v1.6: 3 waves, correct RPCs/SyncVars, audio system, lobby |
| 5 | Wave Transition UI not showing | DONE | Fixed: Static event pattern (EnemySpawner + WaveTransitionUI v1.2) |

## TIER 2 - HIGH (Secures bonus points / Academic Compliance)

| # | Task | Status | Notes |
|---|------|--------|-------|
| 6 | Build UI displacement bug | OPEN | Menu buttons displaced in Build version |
| 7 | Umlaute bug (ae, oe, ue) | OPEN | Text input encoding issue |
| 8 | Story Popup | DONE | StoryPopup.cs with CanvasGroup visibility |
| 9 | Preferences Menu | DONE | PreferencesMenu.cs with volume sliders |
| 10 | Player Colors | OPEN | Sprite-based visual distinction (manual Unity setup) |
| 11 | **Header Compliance** | OPEN | 10 scripts need WICHTIG warning added |
| 12 | **ButtonHover.cs Header** | OPEN | Missing header entirely (TIER 1 priority) |

### Header Compliance Details
Scripts missing full academic header (WICHTIG warning):
- `AutoStartNetwork.cs` - Partial header
- `WeaponConfig.cs` - Partial header
- `WeaponManager.cs` - Partial header
- `EnemyShooter.cs` - Partial header
- `GameAudioManager.cs` - Summary only
- `StoryPopup.cs` - Basic header
- `PreferencesMenu.cs` - Basic header
- `PlayerSetupUI.cs` - Partial header
- `AudioSettings.cs` - Summary only
- `ButtonHover.cs` - **NO HEADER AT ALL**

### LobbyManager.cs Note
Has duplicate header block (lines 1-43 + 44-96). Should consolidate.

## TIER 3 - MEDIUM (Improves grade)

| # | Task | Status | Notes |
|---|------|--------|-------|
| 13 | Difficulty System (3 levels) | OPEN | L1: baseline, L2: +10% spawn, L3: additional +10% |
| 14 | Score balancing per difficulty | OPEN | Reward scaling |
| 15 | Difficulty Choice UI | OPEN | Selection method TBD |

## TIER 4 - LOW (Polish)

| # | Task | Status | Notes |
|---|------|--------|-------|
| 16 | Graphics settings in Preferences | OPEN | If feasible |
| 17 | Clean up repo | OPEN | Remove Build_Testing/, untracked files |
| 18 | Delete Z-Parkplatz deprecated scripts | OPEN | 5 scripts, 350 lines - safe to remove |

---

## Completed Items (Session January 28, 2026)

| Task | Status | Notes |
|------|--------|-------|
| Audio System Consolidation | DONE | Merged to single GameAudioManager with DontDestroyOnLoad |
| Preferences Popup Fix | DONE | MenuManager now calls PreferencesMenu.ShowMenu() |
| Highscore Name Fix | DONE | PlayerSetupUI sends initial name on registration |
| Script Structure Cleanup | DONE | Moved deprecated scripts to Z-Parkplatz |
| GDD Update | DONE | v1.6.1 with script statistics and line counts |
| Full Codebase Audit | DONE | 36 scripts analyzed, action matrix created |
| README.md Rewrite | DONE | Updated to v1.6 with correct architecture |
| Bug Root Cause Analysis | DONE | Wave UI + Projectile spawn issues documented |
| Header Compliance Audit | DONE | 22 complete, 10 partial, 4 missing identified |

---

## BACKLOG - Detailed Bug Reports

### #5 - Wave Transition UI Not Showing Between Waves

**Status:** OPEN
**Priority:** TIER 1
**Last Attempted:** 2026-01-28
**Code Analysis Completed:** 2026-01-28

**Problem:**
`WaveTransitionUI.cs` should display countdown between waves but nothing appears.

**Root Cause Analysis (Completed):**

The C# event pattern IS correctly implemented. The ObserversRpc correctly fires `OnWaveCleared` on clients. Analysis of the code reveals the actual root cause:

**Architecture Flow:**
```
EnemySpawner (Server):
1. WaveSequence() completes wave
2. OnWaveCleared?.Invoke(wave)           → Server fires event
3. NotifyWaveClearedObserversRpc(wave)   → RPC sent to clients

EnemySpawner (Client RPC handler):
4. if (!IsServerStarted) OnWaveCleared?.Invoke(wave)  → Client fires event

WaveTransitionUI:
5. SubscribeWhenReady() → FindAnyObjectByType<EnemySpawner>()
6. enemySpawner.OnWaveCleared += HandleWaveCleared
7. HandleWaveCleared() → ShowCountdown()
```

**Likely Issues (Priority Order):**

1. **Inspector References Missing** (Most Likely)
   - `countdownPanel` and `countdownText` not assigned in Inspector
   - Script disables itself at line 64/72 if null
   - Check console for: "[WaveTransitionUI] countdownPanel/countdownText not assigned!"

2. **WaveTransitionUI on Wrong GameObject / GameRoot Timing**
   - `WaveTransitionUI` is on a child of GameRoot
   - GameRoot is `SetActive(false)` during Lobby (GameStateManager:211)
   - If `OnEnable()` runs before EnemySpawner exists → subscription fails silently
   - The coroutine SubscribeWhenReady() should handle this, but timing can race

3. **countdownSeconds Mismatch**
   - WaveTransitionUI has `countdownSeconds = 3` (line 53)
   - EnemySpawner has 3-second delay between waves (line 122)
   - Current setup: 3s countdown matches 3s delay ✓
   - Note: WAVE_TRANSITION_SETUP.md says 5 seconds - documentation outdated

4. **Event Already Unsubscribed**
   - OnDisable() at line 84 removes subscription
   - If GameRoot toggles visibility, subscription is lost
   - Re-enable requires new SubscribeWhenReady() call

**Debugging Steps:**
1. Add Debug.Log in `HandleWaveCleared()` to confirm event receipt
2. Check Unity Console for existing WaveTransitionUI error messages
3. Verify Inspector: WaveTransitionUI → countdownPanel, countdownText assigned
4. Test as Host (both server + local client = should show countdown)

**Current Implementation (v1.2):**
- `EnemySpawner.OnWaveCleared` event
- `NotifyWaveClearedObserversRpc()` sends to clients
- Client-side event subscription via FindAnyObjectByType

**Files:**
- `EnemySpawner.cs` (v1.2) - Lines 115-117 (server event + RPC)
- `WaveTransitionUI.cs` (v1.1) - Lines 91-105 (subscription), 107-116 (handler)
- `GameStateManager.cs` (v2.0) - Lines 207-219 (GameRoot visibility)

---

### #2 - Projectile Spawn Position Bug

**Status:** OPEN
**Priority:** TIER 1
**Last Attempted:** 2026-01-28
**Code Analysis Completed:** 2026-01-28

**Problem:**
Bullets spawn from behind the player instead of from the front (weapon muzzle).

**Root Cause Analysis:**

The issue is in `WeaponManager.cs` line 116:
```csharp
Vector3 firePosition = playerTransform.position + (Vector3)weapon.firePointOffset;
```

**Current Behavior:**
- `firePointOffset` is a `Vector2` in WeaponConfig (default: Vector2.zero)
- Offset is applied in WORLD SPACE, not local player space
- If player faces right (rotation 90°), offset (0, 0.5) still adds world-up, not player-forward
- Result: bullet spawns at player center, moves toward enemy from there

**Required Fix:**
Transform the offset by player rotation before applying:
```csharp
// Current (wrong):
Vector3 firePosition = playerTransform.position + (Vector3)weapon.firePointOffset;

// Correct:
Vector3 localOffset = playerTransform.TransformDirection(weapon.firePointOffset);
Vector3 firePosition = playerTransform.position + localOffset;
```

**WeaponConfig.firePointOffset Values:**
- Currently: likely (0, 0) or small values
- Should be: (0, 0.5) to spawn 0.5 units "in front" of player
- "In front" = local Y-axis (transform.up in 2D top-down)

**Additional Consideration:**
The rotation calculation at lines 119-122 computes direction to target, but this is for bullet HEADING, not spawn POSITION. The spawn position should be offset from player in the direction the player sprite faces.

**Files:**
- `WeaponManager.cs` - Line 116 (firePosition calculation)
- `WeaponConfig.cs` - Line 37 (firePointOffset field)

---

## Script Architecture (Audited January 28, 2026)

```
Assets/_Project/Scripts/ (36 scripts, 5,762 lines)
├── Enemies/           (4 scripts, 833 lines) - 4 NetworkBehaviour
├── Gameflow/          (4 scripts, 683 lines) - 2 NetworkBehaviour, 2 MonoBehaviour
├── Network/           (3 scripts, 424 lines) - 3 MonoBehaviour
├── Persistence/       (2 scripts, 310 lines) - 1 MonoBehaviour, 1 static class
├── Player/            (6 scripts, 903 lines) - 4 NetworkBehaviour, 1 MonoBehaviour, 1 SO
├── Projectiles/       (2 scripts, 283 lines) - 1 NetworkBehaviour, 1 MonoBehaviour
├── UI/
│   ├── Root/          (3 scripts, 545 lines)
│   ├── Lobby/         (3 scripts, 1,109 lines) - 1 NetworkBehaviour
│   └── Menu/          (4 scripts, 650 lines)
└── Z-Parkplatz/       (5 deprecated scripts, 350 lines) ⚠️ SAFE TO DELETE
```

### FishNet Usage Summary
- **NetworkBehaviour:** 12 scripts
- **SyncVar:** PlayerHealth, PlayerController, ScoreManager, EnemySpawner, GameStateManager, LobbyManager, EnemyHealth
- **SyncDictionary:** LobbyManager (playerDataDict), ScoreManager (playerScores)
- **ServerRpc:** 7 scripts
- **ObserversRpc:** EnemySpawner, LobbyManager

---

## Anforderungen Coverage Summary

| Pflicht Category (80 pts) | Points | Status |
|---|---|---|
| 1. Multiplayer-Basis (FishNet) | 10 | DONE |
| 2. Spielersteuerung & Sync | 15 | DONE |
| 3. Bullet-Hell Shooting | 20 | DONE |
| 4. Gegner/Wave System | 15 | DONE |
| 5. Leben & Gameflow | 10 | DONE |
| 6. HUD & Score | 10 | DONE |
| **Base Total** | **80/80** | **DONE** |

| Bonus Category (max 20 pts) | Status |
|---|---|
| A) Lobby & Matchmaking | DONE (5-10 pts) |
| B) Power-Ups | NOT IMPLEMENTING |
| C) Complex Patterns | NOT IMPLEMENTING |
| D) Extended Persistence | DONE (SQL/PHP) |
| E) Coop/Versus | NOT IMPLEMENTING |
| F) Visual/Technical | DONE (Audio system) |

**Estimated current score: ~90-92 / 100**

(Upgraded estimate based on comprehensive lobby system + audio system + SQL persistence)

---

## Unity Inspector Actions Required

After this session, manually verify in Unity:
1. **MenuManager** - Assign PreferencesMenu reference (changed from GameObject)
2. **GameAudioManager** - Assign Menu, Lobby, Gameplay AudioClips
3. **WaveTransitionUI** - Verify countdownPanel and countdownText assigned

---

## Next Session Priority

1. **Fix Projectile Spawn Position Bug** - WeaponManager.cs line 116
   - Change: `playerTransform.TransformDirection(weapon.firePointOffset)`
   - Set WeaponConfig.firePointOffset to (0, 0.5) in Inspector

2. **Debug Wave Transition UI** - Check Inspector refs first
   - Verify countdownPanel and countdownText assigned
   - Add Debug.Log in HandleWaveCleared() if still not working

3. **Add Missing Headers** - Academic compliance
   - ButtonHover.cs needs full header (highest priority)
   - 9 other scripts need WICHTIG warning added

4. **Consolidate LobbyManager.cs Header** - Remove duplicate block

5. **Optional: Delete Z-Parkplatz** - 5 scripts, 350 lines

---

## SESSION HANDOFF - January 28, 2026 (End of Day)

### What Was Done This Session
1. **Full Codebase Audit** - 36 scripts, 5,762 lines analyzed
2. **README.md** - Complete rewrite to v1.6 architecture
3. **GDD** - Updated with script statistics and line counts
4. **OpenTasks** - Added header compliance tracking, FishNet summary
5. **Wave Transition UI Debugging** - Added debug logs to trace event flow

### Current State of Wave Transition UI Bug

**Debug logs added to:**
- `EnemySpawner.cs` lines 115-117: Shows subscriber count when firing event
- `WaveTransitionUI.cs` lines 109, 118, 123, 134, 140, 145: Full trace through handler

**What we know:**
- `[WaveTransitionUI] Subscribed to EnemySpawner.OnWaveCleared` appears ✅
- `HandleWaveCleared` is NOT being called ❌

**Test needed tomorrow:**
1. Play as Host
2. Kill all enemies in Wave 1
3. Check Console for:
   - `[EnemySpawner] Wave 1 cleared!`
   - `[EnemySpawner] Firing OnWaveCleared event for wave 1. Subscribers: X`
   - If Subscribers = 0: WaveTransitionUI subscribed to wrong instance
   - If Subscribers = 1+ but no HandleWaveCleared: Event invoke failing

**Likely root cause:**
WaveTransitionUI uses `FindAnyObjectByType<EnemySpawner>()` which may find a different instance than the one running WaveSequence. The networked EnemySpawner may have multiple instances or the subscription happens to a prefab.

### Projectile Spawn Bug - Ready to Fix

**File:** `WeaponManager.cs` line 116
**Current (broken):**
```csharp
Vector3 firePosition = playerTransform.position + (Vector3)weapon.firePointOffset;
```
**Fix:**
```csharp
Vector3 rotatedOffset = playerTransform.TransformDirection(weapon.firePointOffset);
Vector3 firePosition = playerTransform.position + rotatedOffset;
```
**Also:** Set `WeaponConfig.firePointOffset` to `(0, 0.5)` in Unity Inspector

### Header Compliance - 10 Scripts Need Updates

Priority order:
1. `ButtonHover.cs` - NO HEADER AT ALL
2. `AudioSettings.cs` - Summary only
3. `GameAudioManager.cs` - Summary only
4. `AutoStartNetwork.cs` - Partial
5. `WeaponConfig.cs` - Partial
6. `WeaponManager.cs` - Partial
7. `EnemyShooter.cs` - Partial
8. `StoryPopup.cs` - Basic
9. `PreferencesMenu.cs` - Basic
10. `PlayerSetupUI.cs` - Partial

### Files Modified This Session
- `Documentation/OpenTasks_Prioritized.md` - Major updates
- `Documentation/GameDesignDocument.md` - Script statistics added
- `Documentation/WAVE_TRANSITION_SETUP.md` - Corrected countdown seconds
- `README.md` - Complete rewrite
- `WaveTransitionUI.cs` - Debug logs added
- `EnemySpawner.cs` - Debug logs added

### Estimated Project Score: ~90-92 / 100
- Base requirements: 80/80 ✅
- Lobby system: +10 pts
- SQL/PHP persistence: +7 pts
- Audio system: +5 pts

---

*Document maintained for session continuity*
