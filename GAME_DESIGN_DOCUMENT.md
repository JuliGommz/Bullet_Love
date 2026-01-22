# Game Design Document: Showroom Tango
## 2-Player Shared-Screen Co-op Bullet-Hell

**Project**: Showroom Tango  
**Developer**: Julian (Juli)  
**Course**: Game & Multimedia Design - PRG Module  
**Institution**: SRH Hochschule  
**Due Date**: 23.01.2026  
**Technology**: Unity 6000.0.62f1 + FishNet 4.x  
**Repository**: https://github.com/JuliGommz/Showroom Tango.git

**Version**: 1.1  
**Last Updated**: 26.12.2025  
**Status**: Phase 2 → Phase 3 Transition

---

## Document Attribution

**Source**: Academic project requirements + collaborative design process

**Content Origins**:
- **Academic Requirements**: University course assignment (Arbeitsauftrag SRH)
- **Technical Architecture**: Claude AI assistance (architecture decisions, risk analysis, technical specifications)
- **Game Concept Refinement**: Collaborative (original split-screen idea adapted to shared-screen coop based on technical analysis)
- **Implementation Roadmap**: Claude AI (based on project requirements and best practices)
- **Visual Design**: Julian's handwritten notes (Tron aesthetic concept)
- **Progression System**: Hybrid approach (Julian's time-based concept + AI's wave-clear system)

**AI Contributions (Claude)**:
- Technical feasibility analysis
- Architecture decision records (ADRs)
- Risk matrix and mitigation strategies
- Implementation timeline and phase breakdown
- Networking architecture specifications
- Quality standards and verification protocols
- Color palette optimization for clarity
- Rendering method recommendation (two-sprite outline)
- Upgrade system balancing

**Human Decisions (Julian)**:
- Final concept approval (Shared-Screen Co-op)
- Technology stack selection (Unity 6 + FishNet)
- Hybrid progression system (wave-clear + speed bonus)
- Simplified upgrade system (not garage)
- No boss fight (waves only)
- Tron visual aesthetic
- Player customization (name + color)
- Input system implementation choices

---

## CHANGELOG v1.1 (26.12.2025)

**Added**:
- Section 2.4: Hybrid Progression System (wave-clear + speed bonus)
- Section 2.5: Simplified Upgrade System (between-wave upgrades)
- Section 4.2: Player Customization (name + color selection)
- Section 8.1: Complete color palette specification
- Section 8.2: Rendering implementation method (two-sprite outline)
- Appendix D: Visual specification quick reference

**Modified**:
- Section 4.1: Session flow (added Player Setup + Upgrade Selection)
- Section 8.1: Art style finalized (Tron aesthetic)
- Section 15.2: Updated time estimates for new features

**Decisions Finalized**:
- Progression: Hybrid (wave-clear + optional speed timer)
- Upgrades: Simplified 3-option system (no garage)
- Boss: Removed (waves only, per ADR-001)
- Visual: Tron aesthetic confirmed
- Player bullets: White (clarity optimization)
- Enemy bullets: Red (danger signaling)

---

## 1. GAME CONCEPT

### 1.1 High-Level Overview

**Genre**: Top-Down Co-op Bullet-Hell Shooter  
**Player Count**: 2 (Online Multiplayer)  
**Core Loop**: Survive waves of enemies → Shoot bullet patterns → Earn score → Upgrade between waves → Progress through waves

**Unique Selling Point**:
```
Cooperative bullet-hell on a SHARED screen via network
→ Both players visible on same screen (one camera)
→ Connected via online multiplayer (Host + Client)
→ Teamwork required to survive intense bullet patterns
→ Strategic upgrade choices between waves
→ Tron-inspired neon aesthetic
→ Player customization (name + color)
```

### 1.2 Design Pillars

**1. Cooperation**
- Shared victory/defeat
- Independent upgrade choices
- Complementary playstyles encouraged
- Combo system rewards teamwork

**2. Intensity**
- Dense bullet patterns
- Wave-based escalation
- Speed-based scoring
- High skill ceiling

**3. Clarity**
- Player-chosen color distinction
- White player bullets vs red enemy bullets
- Readable patterns despite density
- Clear visual hierarchy

**4. Customization** *(New)*
- Player name selection (max 9 chars)
- 7 neon color options
- Visual identity persistence

**5. Strategic Depth**
- Upgrade decision points
- Risk/reward balancing
- Speed vs safety tradeoffs

---

## 2. GAMEPLAY MECHANICS

### 2.1 Core Mechanics

**Movement**
```
Type: Top-Down 2D
Speed: 5-7 units/second (adjustable, upgradeable)
Input: WASD (Player 1) / Arrow Keys (Player 2)
Constraints: Bounded to playfield area
Rotation: Towards mouse cursor (visual only)
```

**Shooting**
```
Fire Rate: 5 shots/second base (0.2s cooldown, upgradeable)
Input: Space (P1) / Right Ctrl (P2)
Direction: Towards mouse position
Auto-Fire: Optional (bonus feature)
Bullet Color: White core + player color trail
```

**Bullet Patterns** *(Requirement: Minimum 2)*
```
Pattern 1: Straight Shot (Default)
- Single bullet in aimed direction
- Base damage: 10
- Speed: 15 units/second
- Color: White core

Pattern 2: Spread Shot (Default)
- 3 bullets in fan formation
- Angle spread: 30 degrees
- Damage: 7 per bullet
- Speed: 12 units/second

Pattern 3: Triple Shot (Upgrade unlock)
- 3 directions simultaneously (front + 30° left/right)
- Damage: 8 per bullet
- Consumes same fire rate cooldown
```

### 2.2 Health System

**Player Health**
```
Max HP: 100 (base, upgradeable to 150)
Starting HP: 100
Damage Sources:
- Enemy bullet hit: -10 HP
- Enemy collision: -20 HP

Healing:
- Max HP upgrade: +25 HP (refills to new max)
- No regeneration during waves
```

**Death & Respawn**
```
Individual Death:
→ Player becomes spectator
→ Can still see gameplay
→ No respawn (single life per wave)

Game Over Condition:
→ BOTH players dead = Game Over
→ Score submission
→ Restart option
```

### 2.3 Scoring System

**Base Scoring**
```
Enemy Kill: +10 points (to shooter)
Wave Clear: +50 points (both players)
Victory Bonus: +200 points (both players)
```

**Speed Bonus System** *(NEW - Hybrid Progression)*
```
Target Clear Times:
- Wave 1-2: 30 seconds
- Wave 3-4: 45 seconds
- Wave 5: 60 seconds

Speed Bonus Tiers:
- Ultra-fast (< 50% target): +100 points
- Fast (< 75% target): +50 points
- Normal (< 100% target): +25 points
- Slow (> target): +0 points

Application: Both players receive same speed bonus
Display: "Wave Clear! +50 (Speed Bonus: +25)"
```

**Combo System** *(Bonus Feature)*
```
Trigger: Kills within 2 seconds
Combo Multiplier:
- 3-5 kills: 1.25x
- 6-10 kills: 1.5x
- 11+ kills: 2.0x

Both players contribute to shared combo
Combo resets after 2s without kill
```

### 2.4 Hybrid Progression System *(NEW)*

**Wave-Clear Primary Mechanic**
```
Wave ends when: All enemies defeated
No time limit: Players can take as long as needed
Safety-focused: Encourages careful play

Trigger: Last enemy killed
Pause: 3 seconds before next wave
Action: Upgrade selection window opens
```

**Speed Bonus Secondary Mechanic**
```
Wave Timer: Starts on wave begin
Display: Top center HUD, color-coded
- Green: Excellent pace (< 15s on Wave 1)
- Yellow: Good pace (15-30s)
- Red: Slow pace (> 30s)

Purpose: Rewards skilled play without punishing cautious approach
Optional: Players can ignore timer and focus on survival
```

**Design Rationale** *(AI contribution)*:
```
Primary: Wave-clear ensures completion is always possible
Secondary: Speed bonus rewards mastery without creating stress
Balance: Accessibility (everyone can finish) + Skill ceiling (speedrunners)
```

### 2.5 Simplified Upgrade System *(NEW)*

**Trigger & Timing**
```
When: Between waves during 3-second pause
Duration: 5-second decision window
Display: Full-screen upgrade selection UI
Countdown: Visible timer (5...4...3...)

Timeout Behavior: Random upgrade from 3 shown options
→ Automatic application
→ Notification: "Random: Fire Rate +20%"
→ Wave starts immediately
```

**Upgrade Pool** *(5 total options)*
```
1. Fire Rate +20%
   - Reduces cooldown by 0.2s → 0.16s → 0.128s
   - Max Stacks: 3
   - Effect: 5/s → 6.25/s → 7.8/s shots

2. Bullet Damage +50%
   - Base 10 → 15 → 22.5 damage
   - Max Stacks: 2
   - Effect: Faster enemy kills

3. Movement Speed +15%
   - Base 5 → 5.75 → 6.6 units/s
   - Max Stacks: 2
   - Effect: Better dodging

4. Max HP +25
   - 100 → 125 → 150 HP
   - Max Stacks: 2
   - Effect: HP refilled to new max

5. Triple Shot Unlock
   - Replaces Spread Shot pattern
   - One-time unlock (not stackable)
   - Effect: 3-direction simultaneous fire
```

**Selection Mechanics**
```
Input: Press 1, 2, or 3 keys
Display: 3 random options from pool
Constraints:
- Already-maxed upgrades excluded
- Triple Shot only appears if not unlocked
- At least 3 valid options guaranteed

Multiplayer Handling:
→ Each player chooses independently
→ Upgrades apply to individual player only
→ No shared upgrade pool
→ Different players can have different builds
```

**UI Design** *(Tron Style)*
```
┌─────────────────────────────────────────────────┐
│      Wave 2 Complete! Choose Upgrade (5s)        │
├─────────────────────────────────────────────────┤
│                                                  │
│   [1]              [2]              [3]          │
│ FIRE RATE        DAMAGE          SPEED           │
│   +20%            +50%            +15%           │
│  (1/3)           (0/2)           (0/2)           │
│                                                  │
│  Press 1, 2, or 3                                │
└─────────────────────────────────────────────────┘

Visual Style:
- Neon outline boxes (white glow)
- Black background
- Stack indicator below each option
- Selected option: Pulse animation
- Countdown: Color shift (green → red)
```

---

## 3. ENEMY DESIGN

### 3.1 Enemy Types *(Requirement: Minimum 2)*

**Type 1: Chaser**
```
Visual: Neon red triangle outline
Behavior: Pursues nearest player
Movement: Vector2.MoveTowards (simple pathfinding)
Speed: 3 units/second (base)
HP: 30
Damage: 20 (on collision, no bullets)
Score Value: 10 points

AI Logic:
- Select nearest player as target
- Move directly towards target
- No obstacle avoidance
- Collision = damage + bounce back
```

**Type 2: Shooter**
```
Visual: Neon pink square outline
Behavior: Maintains distance, fires at player
Movement: Keeps 5-8 units away from nearest player
Speed: 2 units/second
HP: 20
Damage: 10 (bullet) + 15 (collision)
Score Value: 15 points

AI Logic:
- Select nearest player
- If distance < 5 units: Move away
- If distance > 8 units: Move closer
- Fire rate: 1 shot per 2 seconds
- Bullet: Straight shot towards player position
```

**Enemy Bullets**
```
Visual: Small red circle (neon red #FF0033)
Speed: 8 units/second
Damage: 10 HP
Lifetime: 5 seconds (auto-destroy)
Collision: Player layer only (ignore enemies)
```

### 3.2 Wave Structure *(Based on ADR-001)*

**Wave Progression**
```
Wave 1: Tutorial
- Enemies: 5x Chaser only
- Goal: Learn controls, basic shooting
- Target Time: 30 seconds
- Difficulty: Very Easy

Wave 2: Introduction
- Enemies: 8x Chaser + 2x Shooter
- Goal: Dodge first enemy bullets
- Target Time: 30 seconds
- Difficulty: Easy

Wave 3: Escalation
- Enemies: 10x Chaser + 5x Shooter
- Goal: Multi-threat management
- Target Time: 45 seconds
- Difficulty: Medium

Wave 4: Challenge
- Enemies: 15x Mixed (10 Chaser + 5 Shooter)
- Spawn rate: +25% faster
- Enemy speed: +10%
- Target Time: 45 seconds
- Difficulty: Hard

Wave 5: Final Wave
- Enemies: 20x Mixed (12 Chaser + 8 Shooter)
- Spawn rate: +50% faster
- Enemy speed: +15%
- Enemy fire rate: +25%
- Target Time: 60 seconds
- Difficulty: Very Hard
- Reward: Victory!
```

**Wave Timing & Flow**
```
Wave Start:
- 3-second countdown overlay
- "Wave X - Ready!" display
- Enemies begin spawning

Wave Active:
- Timer running (top center)
- Enemies spawn according to wave definition
- Players shoot/dodge
- Score accumulating

Wave Clear:
- Last enemy killed
- "Wave Clear!" overlay
- Score summary (kills + speed bonus)
- 3-second pause
- Upgrade selection (5 seconds)
- Next wave auto-starts
```

**Spawn Patterns** *(AI contribution)*
```
Spawn Locations: Edges of playfield (random)
Spawn Interval: 
- Wave 1-2: 1 enemy per 2 seconds
- Wave 3-4: 1 enemy per 1.5 seconds
- Wave 5: 1 enemy per 1 second

Spawn Distribution:
- Random edge position
- Avoid spawning on top of players (min 3 units away)
- Stagger Chaser vs Shooter spawns (alternate when possible)
```

---

## 4. GAME FLOW

### 4.1 Session Structure *(UPDATED)*

```
Main Menu
    ↓
**Player Setup** *(NEW)*
│ ┌─────────────────────────────────┐
│ │ Enter Your Name (max 9 chars)   │
│ │ [Text Input Field]              │
│ │                                  │
│ │ Choose Your Color:               │
│ │ [Blue][Magenta][Green][Yellow]  │
│ │ [Orange][Cyan][Purple]           │
│ │                                  │
│ │ Preview: ●← Your Player          │
│ │                                  │
│ │ [Confirm]                        │
│ └─────────────────────────────────┘
    ↓
Host/Client Selection
    ↓
Connection Established
│ - Network status: "Connected"
│ - Players ready display
    ↓
Countdown (3-2-1-GO)
    ↓
Wave 1 Starts
    ↓
Gameplay Loop:
│   Wave Active:
│   ├─ Shoot enemies
│   ├─ Dodge bullets  
│   ├─ Wave timer running (optional pressure)
│   └─ Kill all enemies
│       ↓
│   Wave Clear:
│   ├─ Score summary (+kills +speed bonus)
│   ├─ 3-second pause
│   └─ **Upgrade Selection** *(NEW)*
│       ┌────────────────────────────┐
│       │ Choose Upgrade (5s)         │
│       │ [1] [2] [3]                │
│       │ Press 1, 2, or 3           │
│       └────────────────────────────┘
│       ↓
│   Next Wave (auto-start)
│   └─→ Repeat until Wave 5 complete
    ↓
Victory (Wave 5 complete)
OR
Game Over (Both players dead)
    ↓
End Screen:
│ - Final Score Display
│ - Speed Bonuses Summary
│ - Highscore Submission (PHP/SQL)
│ - Options: [Restart] [Main Menu]
    ↓
Return to Main Menu
```

### 4.2 Player Customization System *(NEW)*

**Player Setup Screen**

**UI Layout**:
```
┌─────────────────────────────────────────┐
│           Showroom Tango                    │
│                                          │
│  Enter Your Name:                        │
│  [_________] (max 9 characters)         │
│                                          │
│  Choose Your Color:                      │
│  ┌───┐┌───┐┌───┐┌───┐                  │
│  │ ● ││ ● ││ ● ││ ● │                  │
│  └───┘└───┘└───┘└───┘                  │
│  Blue  Mag  Grn  Yel                    │
│                                          │
│  ┌───┐┌───┐┌───┐                        │
│  │ ● ││ ● ││ ● │                        │
│  └───┘└───┘└───┘                        │
│  Org  Cya  Pur                          │
│                                          │
│  Preview:      ●                         │
│            Your Player                   │
│                                          │
│         [Continue]                       │
└─────────────────────────────────────────┘
```

**Data Storage**:
```
Local Persistence (PlayerPrefs):
→ Key: "PlayerName" | Value: string (1-9 chars)
→ Key: "PlayerColor" | Value: SerializedColor

Pre-fill on subsequent launches:
→ Name input shows last used name
→ Color button pre-selected

Network Synchronization:
→ [SyncVar] public string playerName
→ [SyncVar] public Color playerColor
→ Set via [ServerRpc] during OnStartClient()
```

**Validation**:
```
Name Field:
- Min length: 1 character
- Max length: 9 characters
- Allowed chars: A-Z, a-z, 0-9, underscore, hyphen
- Empty = default "Player"
- Profanity filter: Optional (out of scope for v1.0)

Color Selection:
- Exactly 1 color must be selected
- Default: Neon Blue (#00D9FF) if none chosen
- Visual feedback: Selected button highlighted
```

**Implementation Notes** *(AI contribution)*:
```csharp
// PlayerSetupUI.cs (pseudo-code)
public class PlayerSetupUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button[] colorButtons; // 7 buttons
    [SerializeField] private SpriteRenderer previewSprite;
    
    private Color selectedColor = ColorPalette.NeonBlue;
    
    void Start()
    {
        // Load saved data
        nameInput.text = PlayerPrefs.GetString("PlayerName", "");
        nameInput.characterLimit = 9;
        
        // Pre-select last color
        Color savedColor = LoadColor("PlayerColor", ColorPalette.NeonBlue);
        SelectColor(savedColor);
    }
    
    public void OnColorButtonClicked(int index)
    {
        Color[] colors = {
            ColorPalette.NeonBlue,
            ColorPalette.NeonMagenta,
            ColorPalette.NeonGreen,
            ColorPalette.NeonYellow,
            ColorPalette.NeonOrange,
            ColorPalette.NeonCyan,
            ColorPalette.NeonPurple
        };
        
        selectedColor = colors[index];
        UpdatePreview();
        HighlightButton(index);
    }
    
    public void OnContinue()
    {
        string finalName = string.IsNullOrEmpty(nameInput.text) 
            ? "Player" 
            : nameInput.text;
            
        PlayerPrefs.SetString("PlayerName", finalName);
        SaveColor("PlayerColor", selectedColor);
        
        // Proceed to Host/Client selection
        SceneManager.LoadScene("HostClientSelection");
    }
    
    void UpdatePreview()
    {
        previewSprite.color = selectedColor;
    }
}
```

### 4.3 Game States

**State Machine** *(Same as v1.0)*
```
LOBBY → PLAYER_SETUP → HOST_CLIENT_SELECTION → CONNECTING 
→ COUNTDOWN → PLAYING → WAVE_CLEAR → UPGRADE_SELECTION 
→ PLAYING (loop) → VICTORY/GAME_OVER
```

---

## 5. TECHNICAL ARCHITECTURE

### 5.1 Networking *(Same as v1.0, FishNet 4.x)*

**Server Authority**:
- Enemy spawning
- Damage calculation
- HP updates
- Score updates  
- Wave progression
- Game state transitions
- Upgrade application

**Client Authority**:
- Input sampling (movement, fire)
- Visual-only effects (particles)
- UI updates (reading SyncVars)

### 5.2 Camera System *(Same as v1.0)*

**Static Orthographic Camera** (chosen approach):
```
Projection: Orthographic
Size: 12 (adjustable to playfield)
Position: (0, 10, 0)
Rotation: (90, 0, 0) - looking down
Clear Flags: Solid Color (black #000000)

Rationale:
✓ Simple implementation
✓ Full playfield overview
✓ No issues when players separate
✓ Bullet patterns fully visible
```

### 5.3 Input System *(Based on ADR-007)*

**Unity New Input System**:
```
Input Actions Asset: PlayerInputActions.inputactions

Actions Defined:
1. Movement (Vector2)
   - P1: WASD
   - P2: Arrow Keys

2. Fire (Button)
   - P1: Space
   - P2: Right Ctrl

3. Upgrade1/2/3 (Buttons) *(NEW)*
   - Both: 1, 2, 3 keys
   - Triggered during upgrade screen only
```

### 5.4 Object Pooling *(ADR-002, critical)*

**Pool Sizes**:
```
Player Bullets: 100 (50 per player buffer)
Enemy Bullets: 100
Enemies: 30
VFX Particles: 20 (if implemented)
```

**Implementation**: Queue-based recycling (same as v1.0)

### 5.5 Project Structure *(Same as v1.0)*

```
Assets/_Project/
├── Scenes/
│   ├── Bootstrap/Bootstrap.unity
│   └── Game/Game.unity
├── Scripts/
│   ├── Network/
│   │   ├── NetworkUIManager.cs
│   │   └── BootstrapManager.cs
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerHealth.cs
│   │   ├── PlayerShooting.cs
│   │   └── PlayerSetupUI.cs *(NEW)*
│   ├── Enemies/
│   │   ├── EnemyBase.cs
│   │   ├── EnemyChaser.cs
│   │   └── EnemyShooter.cs
│   ├── Projectiles/
│   │   ├── Bullet.cs
│   │   └── BulletPool.cs
│   ├── UI/
│   │   ├── HUDManager.cs
│   │   ├── UpgradeSelectionUI.cs *(NEW)*
│   │   └── GameStateUI.cs
│   ├── Systems/
│   │   ├── WaveManager.cs *(NEW)*
│   │   ├── UpgradeManager.cs *(NEW)*
│   │   └── ScoreManager.cs
│   ├── Utility/
│   │   └── ColorPalette.cs *(NEW)*
│   └── Persistence/
│       └── HighscoreManager.cs
├── Prefabs/
│   ├── Player/PlayerPrefab.prefab
│   ├── Enemies/
│   ├── Projectiles/
│   └── UI/
├── Art/
│   ├── Sprites/
│   │   ├── CircleOutline.png *(NEW - for players/bullets)*
│   │   ├── TriangleOutline.png *(NEW - Chaser)*
│   │   └── SquareOutline.png *(NEW - Shooter)*
│   └── Materials/
└── Data/
    └── ScriptableObjects/
        └── WaveData/ *(NEW)*
```

---

## 6. USER INTERFACE

### 6.1 HUD Layout *(UPDATED)*

```
┌─────────────────────────────────────────────────┐
│ P1 ████████░░ 85HP   Score: 1250                │
│ [PlayerName1]                                    │
│                                                  │
│          Wave 3/5    Timer: 00:34                │
│          ────────    ▓▓▓▓▓▓▓░░                  │
│                                                  │
│                                                  │
│          [Gameplay Area - Shared Screen]         │
│                                                  │
│                                                  │
│                                                  │
│                 P2 ██████░░░░ 60HP              │
│                 [PlayerName2] Score: 980        │
│                                                  │
│         Team Score: 2230 (optional)              │
└─────────────────────────────────────────────────┘
```

**HUD Elements** *(Tron Style)*:
```
Top Left (Player 1):
- Name label (neon color, 9 chars max)
- HP Bar (gradient fill, player color → red)
- Score counter (white text)

Top Right (Player 2):
- Name label (neon color, 9 chars max)
- HP Bar (gradient fill, player color → red)
- Score counter (white text)

Top Center:
- Wave counter: "Wave 3/5" (white neon)
- Timer: "00:34" (color-coded: green/yellow/red)
- Speed bonus indicator bar (optional)

Bottom Center (Optional):
- Combined team score
- Combo multiplier display (if active)
```

**Tron UI Styling**:
```
All UI Elements:
- Background: Black or transparent
- Borders: Neon outlines (white default)
- Text: Orbitron font (or similar sci-fi)
- Glow: Subtle outer glow shader
- HP Bar: Outlined box + gradient fill
- Buttons: Outlined rectangles with hover pulse
```

### 6.2 Upgrade Selection Screen *(NEW)*

**Full-Screen Overlay**:
```
┌─────────────────────────────────────────────────┐
│                                                  │
│                                                  │
│      Wave 2 Complete! Choose Upgrade             │
│                                                  │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐│
│  │     [1]     │ │     [2]     │ │     [3]     ││
│  │             │ │             │ │             │││
│  │  FIRE RATE  │ │   DAMAGE    │ │   SPEED     │││
│  │    +20%     │ │    +50%     │ │    +15%     │││
│  │   (1/3)     │ │   (0/2)     │ │   (0/2)     │││
│  │             │ │             │ │             │││
│  └─────────────┘ └─────────────┘ └─────────────┘││
│                                                  │
│                                                  │
│            Press 1, 2, or 3                      │
│                 5 seconds                        │
│                                                  │
└─────────────────────────────────────────────────┘
```

**Visual Feedback**:
```
Selected Option:
- Border: Pulsing glow animation
- Background: Slight brightness increase

Countdown Timer:
- 5s: Green
- 3s: Yellow
- 1s: Red + pulsing

On Selection:
- Brief screen flash (player color)
- Selected box expands
- Text: "Fire Rate +20% Applied!"
- Fade out transition to next wave
```

### 6.3 Menu Screens

**Main Menu** *(Updated)*:
```
Title: "Showroom Tango" (large neon text)
Subtitle: "Co-op Bullet-Hell" (smaller)

Buttons (vertical layout):
→ [Play] → Player Setup Screen
→ [Highscores]
→ [Settings] (optional)
→ [Quit]

Visual: Black background + grid pattern
```

**Player Setup Screen**: See Section 4.2

**Host/Client Selection**: Same as v1.0

**End Screen** *(Updated)*:
```
Result: "VICTORY!" or "GAME OVER"

Score Breakdown:
┌──────────────────────────────┐
│ [PlayerName1]                │
│ Kills: 45      (+450 pts)   │
│ Waves: 5       (+250 pts)   │
│ Speed Bonus:   (+175 pts)   │
│ Total: 875                   │
│                              │
│ [PlayerName2]                │
│ Kills: 38      (+380 pts)   │
│ Waves: 5       (+250 pts)   │
│ Speed Bonus:   (+175 pts)   │
│ Total: 805                   │
│                              │
│ Team Score: 1680             │
│ Rank: #5 on Highscore        │
└──────────────────────────────┘

Buttons:
→ [Restart]
→ [Main Menu]
```

---

## 7. AUDIO DESIGN *(Bonus Feature - Unchanged)*

### 7.1 Sound Effects (SFX)

**Player Actions**:
- Shoot: Short "pew" (white bullets)
- Hit Confirm: Impact sound
- Upgrade Select: Positive chime
- Death: Explosion

**Enemy Actions**:
- Enemy Shoot: Lower-pitched (red bullets)
- Enemy Death: Explosion (pitch varies by type)
- Enemy Spawn: Whoosh sound

**UI Sounds**:
- Wave Clear: Triumphant jingle
- Speed Bonus: Ascending tone
- Game Over: Defeat stinger
- Victory: Victory fanfare

### 7.2 Music

**Gameplay**: High-tempo electronic/chiptune  
**Menu**: Ambient electronic  

---

## 8. VISUAL DESIGN

### 8.1 Art Style - Tron Aesthetic *(FINALIZED)*

**Visual Direction**
```
Style: Neon outlines on pure black background
Inspiration: TRON Legacy, Geometry Wars, Gridrunner
Theme: Futuristic arcade, cyber aesthetic
Emphasis: High contrast for bullet clarity
```

**Complete Color Specification** *(NEW)*

**Player Colors (Selectable - 7 Options)**:
```
1. Neon Blue      #00D9FF  ████ (Default recommendation)
2. Neon Magenta   #FF00FF  ████
3. Neon Green     #00FF00  ████
4. Neon Yellow    #FFFF00  ████
5. Neon Orange    #FF6600  ████
6. Neon Cyan      #00FFFF  ████
7. Neon Purple    #CC00FF  ████
```

**Enemy Colors (Fixed)**:
```
Chaser Enemy:  Neon Red   #FF0033  ████ (Triangle)
Shooter Enemy: Neon Pink  #FF0099  ████ (Square)
```

**Bullet Colors** *(Optimized for Clarity)*:
```
Player Bullets:
- Core: White        #FFFFFF  ████
- Trail: Player's chosen color (fade out)
- Rationale: White = maximum visibility against black
- Distinction: Trail color identifies shooter

Enemy Bullets:
- Core: Neon Red     #FF0033  ████
- Trail: Red fade
- Rationale: Red = universal danger signal
- Clarity: Clearly distinct from white player bullets
```

**Background & Environment**:
```
Background: Pure Black   #000000  ████

Grid Pattern (Optional):
- Color: Dark Cyan       #003333  ████
- Opacity: 20%
- Style: Perspective grid lines
- Purpose: Depth cue, aesthetic
```

**Visual Hierarchy** *(AI contribution: 0.1% clarity optimization)*:
```
Priority 1: Enemy Bullets (RED) - MUST DODGE
Priority 2: Player Bullets (WHITE) - Your weapons
Priority 3: Player Sprites (COLOR) - Position awareness
Priority 4: Enemies (RED/PINK) - Targets
Priority 5: UI Elements (WHITE/COLOR) - Information
Priority 6: Background (BLACK) - Context

Design Principle:
→ Danger = Red
→ Offensive = White
→ Identity = Player color
→ Contrast = Maximum
```

**Sprite Design**

**Players**:
```
Shape: Circle outline (no fill)
Outline Width: 3-5 pixels
Core Sprite: White base (color applied via SpriteRenderer.color)
Glow: Outer shader glow effect
Trail: Line renderer with gradient (color → transparent)
Size: ~1 unit diameter
```

**Enemies**:
```
Chaser:
- Shape: Triangle outline (pointing towards target)
- Color: Neon Red #FF0033
- Outline Width: 2-3 pixels
- Size: ~1.2 units

Shooter:
- Shape: Square outline (weapon visible on one side)
- Color: Neon Pink #FF0099
- Outline Width: 2-3 pixels  
- Size: ~1.0 units
```

**Bullets**:
```
Player Bullets:
- Shape: Small circle outline
- Core Color: White #FFFFFF
- Trail: Player color particle fade
- Size: ~0.3 units

Enemy Bullets:
- Shape: Small circle outline
- Color: Neon Red #FF0033
- Trail: Red particle fade
- Size: ~0.25 units
```

### 8.2 Rendering Implementation *(NEW - AI contribution)*

**Chosen Method: Two-Sprite Outline**

**Rationale**:
```
✓ Simple to implement (no custom shaders)
✓ Fast iteration (Inspector tweaking)
✓ Guaranteed compatibility
✓ No shader debugging overhead
✓ Sufficient visual quality for project

Alternative (Shader-based) deferred to Phase 6 if time permits
```

**Implementation Pattern**:
```
GameObject Hierarchy:
ParentObject (NetworkObject)
├── OuterGlow (SpriteRenderer)
│   └── Sprite: CircleOutline.png
│   └── Color: Player/Enemy color
│   └── Scale: 1.1x - 1.15x
│   └── Sorting Order: 0 (back layer)
│   └── Material: Default (or glow material)
└── InnerCore (SpriteRenderer)
    └── Sprite: CircleOutline.png (same)
    └── Color: Black #000000 (or transparent)
    └── Scale: 1.0x
    └── Sorting Order: 1 (front layer)
    └── Material: Default
```

**Example: Player Prefab**:
```
Player (GameObject + NetworkObject)
├── PlayerGlow (SpriteRenderer)
│   └── Sprite: CircleOutline
│   └── Color: [SyncVar] playerColor
│   └── Scale: (1.1, 1.1, 1)
│   └── Sorting Layer: "Entities"
│   └── Order: 0
└── PlayerCore (SpriteRenderer)
    └── Sprite: CircleOutline
    └── Color: Black or (0,0,0,0) transparent
    └── Scale: (1, 1, 1)
    └── Sorting Layer: "Entities"
    └── Order: 1
```

**Example: Bullet Prefab**:
```
Bullet (GameObject + NetworkObject)
├── BulletGlow (SpriteRenderer)
│   └── Color: White (player) or Red (enemy)
│   └── Scale: 1.15x
└── BulletCore (SpriteRenderer)
    └── Color: White/Red (same as glow)
    └── Scale: 1.0x

Plus:
└── TrailRenderer (Component)
    └── Start Color: White/Red
    └── End Color: Transparent
    └── Lifetime: 0.3s
```

**Sprite Assets Required**:
```
CircleOutline.png:
- White circle with transparent center
- Outline width: ~10% of diameter
- Texture: 256x256 pixels (sufficient)
- Format: PNG with alpha channel

TriangleOutline.png:
- White triangle outline
- Same style as circle

SquareOutline.png:
- White square outline
- Same style as circle

Usage: Apply color via SpriteRenderer.color
```

**Glow Effect (Post-Processing)**:
```
Unity URP Volume Component:
→ Add "Bloom" override
→ Intensity: 4-6 (adjustable)
→ Threshold: 0.3 (only bright colors glow)
→ Scatter: 0.7 (soft glow spread)

Result: All neon-colored sprites automatically glow
No per-object configuration needed
```

**Color Application Code** *(Pseudo-code)*:
```csharp
// PlayerController.cs
public class PlayerController : NetworkBehaviour
{
    [SyncVar] public Color playerColor = ColorPalette.NeonBlue;
    
    private SpriteRenderer outerGlow;
    private TrailRenderer trail;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (IsOwner)
        {
            // Send chosen color to server
            Color savedColor = LoadPlayerColor();
            SetPlayerColorServerRpc(savedColor);
        }
        
        ApplyVisualStyle();
    }
    
    [ServerRpc]
    void SetPlayerColorServerRpc(Color color)
    {
        playerColor = color;
    }
    
    void ApplyVisualStyle()
    {
        // Outer glow sprite gets player color
        outerGlow.color = playerColor;
        
        // Trail renderer gradient
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(playerColor, 0.0f),
                new GradientColorKey(playerColor, 0.5f)
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        trail.colorGradient = gradient;
    }
}
```

### 8.3 VFX *(Bonus Feature)*

**Particle Effects**:
```
Muzzle Flash:
- Small radial burst at player position
- Color: White
- Lifetime: 0.1s
- Count: 5-10 particles

Explosion (Enemy Death):
- Radial particle burst
- Color: Enemy type color (red/pink)
- Lifetime: 0.5s
- Count: 20-30 particles

Hit Feedback:
- Sprite flash (white overlay, 1 frame)
- Small particle burst at hit point
- Optional: Subtle screen shake
```

---

## 9. PROGRESSION & DIFFICULTY

### 9.1 Difficulty Curve *(Updated for Hybrid System)*

**Wave Scaling**:
```
Wave 1: Tutorial (Target: 30s)
- Enemy Count: 5 Chasers
- Enemy Speed: 100% base
- Fire Rate: N/A
- Purpose: Learn controls

Wave 2: Introduction (Target: 30s)
- Enemy Count: 10 total (8 Chaser + 2 Shooter)
- Enemy Speed: 100%
- Fire Rate: 100% (2s cooldown)
- Purpose: Introduce dodging

Wave 3: Skill Check (Target: 45s)
- Enemy Count: 15 total (10 + 5)
- Enemy Speed: 105%
- Fire Rate: 110% (1.8s cooldown)
- Purpose: Multi-threat management

Wave 4: Challenge (Target: 45s)
- Enemy Count: 20 total (mixed)
- Enemy Speed: 110%
- Fire Rate: 120% (1.6s cooldown)
- Spawn Rate: +25%

Wave 5: Mastery (Target: 60s)
- Enemy Count: 25 total (mixed)
- Enemy Speed: 115%
- Fire Rate: 125% (1.6s cooldown)
- Spawn Rate: +50%
- Victory Condition
```

**Balancing Targets** *(Requires Playtesting)*:
```
Wave 1 Clear: 95%+ players (tutorial)
Wave 3 Clear: 60% players (skill gate)
Wave 5 Clear: 25-30% players (mastery)

Average Session: 6-10 minutes
Target Deaths: 2-4 per session (with upgrades)

Speed Bonus Achievement:
- Ultra-fast: Top 10% players
- Fast: Top 30% players
- Normal: Average 50% players
```

### 9.2 Player Progression *(Within Session)*

**No Persistent Progression**:
```
Reason: Arcade-style, pure skill-based
Each session: Fresh start
Progression: Upgrades earned within session only

Meta-Progression (Future):
→ Not planned for v1.0
→ Could add: Unlockable colors, patterns, skins
→ Risk: Scope creep
```

**Upgrade Build Diversity** *(NEW)*:
```
Tank Build:
- 2x Max HP upgrades
- 1x Movement Speed
- Focus: Survivability

DPS Build:
- 2x Bullet Damage
- 1x Triple Shot
- Focus: Kill speed

Speed Build:
- 3x Fire Rate
- 2x Movement Speed
- Focus: Evasion + output

Balanced Build:
- Mix of all upgrades
- Adaptable to situation
```

---

## 10. MANDATORY REQUIREMENTS MAPPING

### 10.1 Pflichtanforderungen (80 Points)

**1. Multiplayer-Basis (FishNet) - 10 Points**
```
[✓] FishNet correctly integrated
[✓] Host/Client connection functional
[✓] Two instances connect stably
[✓] NetworkManager properly configured

Status: COMPLETE (Phase 1)
Evidence:
- Bootstrap scene functional
- Tugboat transport configured
- Connection tested successfully
```

**2. Spielersteuerung & Synchronisation - 15 Points**
```
[✓] Top-Down movement (WASD)
[✓] Players as NetworkObject
[✓] Ownership correctly set
[~] Server-authoritative synchronization
[~] Minimum one SyncVar

Status: IN PROGRESS (Phase 2)
Implemented: Movement, NetworkObject, Ownership
TODO: Complete sync, add SyncVars (HP, Score, Color, Name)
```

**3. Schießen & Bullet-Hell-Mechaniken - 20 Points**
```
[ ] Network-capable projectile system
[ ] Minimum 2 bullet patterns (Straight + Spread)
[ ] Projectiles synchronized
[ ] Fire rate / cooldown system
[ ] Hit detection on both clients

Status: NOT STARTED (Phase 3 - NEXT)
```

**4. Gegner-, Wave-System - 15 Points**
```
[ ] Minimum 2 enemy types (Chaser + Shooter)
[ ] Enemies spawned server-side
[ ] Wave structure (5 waves planned)
[ ] Enemy bullets visible on both clients

Status: NOT STARTED (Phase 4)
```

**5. Leben, Treffer & Spielablauf - 10 Points**
```
[ ] HP system for players
[ ] Damage synchronized
[ ] Clear gameflow (Start → Play → End)
[ ] End screen / restart

Status: NOT STARTED (Phase 5)
```

**6. HUD & Punkte - 10 Points**
```
[ ] HUD with HP display
[ ] HUD with Score display
[ ] Score for kills / survival
[ ] Score synchronized
[ ] Highscore via PHP & SQL

Status: NOT STARTED (Phase 5)
Note: JSON fallback prepared (ADR-006)
```

**Current Total: 25/80 Points Secured**

### 10.2 Bonusfeatures (Max 20 Points)

**Implemented / Planned**:

**A) Upgrade System (NEW) - 5-10 Points**
```
Category: Power-Ups & Spielmechaniken
Feature: Between-wave upgrade selection
- 5 upgrade types
- 3-option random selection
- 5-second decision window
- Independent player choices

Implementation Complexity: Medium
Point Estimate: 7-10 points
Status: Specified, not implemented
```

**B) Player Customization (NEW) - 3-5 Points**
```
Category: Visuelle Verbesserungen
Feature: Name + Color selection
- 9-char name input
- 7 color options
- Visual preview
- Persistent storage

Implementation Complexity: Low
Point Estimate: 3-5 points
Status: Specified, not implemented
```

**C) Hybrid Progression (NEW) - 3-5 Points**
```
Category: Spielmechaniken
Feature: Wave-clear + Speed bonus
- Timer per wave
- Speed bonus tiers
- Score modifier

Implementation Complexity: Low
Point Estimate: 3-5 points
Status: Specified, not implemented
```

**D) Tron Visual Style - 5-8 Points**
```
Category: Visuelle Verbesserungen
Feature: Neon aesthetic + glow effects
- Two-sprite outline rendering
- Bloom post-processing
- Color palette implementation
- Grid background (optional)

Implementation Complexity: Medium
Point Estimate: 5-8 points
Status: Specified, not implemented
```

**E) VFX & SFX (Planned) - 5-10 Points**
```
Category: Visuelle & technische Verbesserungen
Feature: Sound effects + particle effects
- Shoot/Hit/Explosion sounds
- Particle systems (muzzle, death)
- Background music

Implementation Complexity: Medium
Point Estimate: 5-10 points
Status: Planned for Phase 6
```

**Bonus Estimate: 23-38 potential points**  
**Cap: 20 points maximum**  
**Target: Implement top 20-point features**

---

## 11. DEVELOPMENT ROADMAP

### 11.1 Phase Overview *(UPDATED for v1.1)*

```
Phase 1: Infrastructure (COMPLETE ✓)
Duration: 3-4 days (completed)
Deliverable: FishNet setup, Git, project structure
Status: Done

Phase 2: Player Foundation (IN PROGRESS ~)
Duration: 5-7 days
Deliverable: Movement, camera, basic sync
Points: 25/80
Status: 80% complete
TODO: Complete SyncVars, finalize sync

Phase 3: Shooting Core (NEXT ▶)
Duration: 5-7 days
Deliverable: Bullets, patterns, pooling, hit detection
Points Target: 45/80 (+20)
Key Tasks:
- Bullet prefab (two-sprite outline)
- Shooting mechanism + cooldown
- Object pooling (100 bullets)
- Two patterns (Straight + Spread)
- Hit detection [ServerRpc]

Phase 4: Enemy System
Duration: 5-7 days
Deliverable: 2 enemy types, wave spawning
Points Target: 60/80 (+15)
Key Tasks:
- Enemy prefabs (Triangle + Square outlines)
- Chaser AI (pursue nearest)
- Shooter AI (maintain distance + fire)
- Wave spawner (server-side)
- Enemy bullet system

Phase 5: Gameflow & HUD (+Upgrades) *(EXTENDED)*
Duration: 6-8 days (was 5-7)
Deliverable: HP, Score, UI, Upgrades, Highscore
Points Target: 80/80 (+20)
Key Tasks:
- Player Setup Screen (name + color)
- HP System (SyncVar + UI)
- Score System + Speed Bonus
- Upgrade Selection UI (5s window)
- UpgradeManager (apply upgrades)
- HUD implementation (Tron style)
- Game State Manager
- PHP/SQL Highscore (or JSON fallback)

Phase 6: Polish & Bonus
Duration: 5-7 days
Deliverable: Tron visuals, VFX, SFX, balance
Points Target: 95-100/100
Key Tasks:
- Bloom Post-Processing (glow effect)
- Grid background (optional)
- UI styling (Tron font + outlines)
- Particle effects (muzzle, explosions)
- Sound effects library
- Background music
- Gameplay balancing (playtesting)
- Bug fixes
- README.md completion
```

### 11.2 Critical Milestones *(UPDATED)*

```
✓ Milestone 1 (Week 2): Infrastructure Complete
→ FishNet working, Git setup
→ 0/80 points (foundation only)

~ Milestone 2 (Current): Player Movement
→ Both players move synchronously
→ Camera functional
→ 25/80 points secured

→ Milestone 3 (End Week 4): Shooting Functional
→ Bullets spawn and sync
→ Hit detection works
→ 45/80 points secured
→ CRITICAL: Phase 3 must complete on time

→ Milestone 4 (End Week 6): Enemies Spawn
→ Waves progress correctly
→ Enemy AI functional
→ 60/80 points secured

→ Milestone 5 (Week 8): MVP Complete
→ Full gameflow working
→ Upgrades functional
→ HUD displays all info
→ 80/80 points SECURED
→ CRITICAL: No bonus work until this checkpoint

→ Milestone 6 (Due Date): Polished Submission
→ Tron visuals applied
→ 1-2 bonus features polished
→ README complete
→ 95-100/100 target
```

### 11.3 Time Budget Check *(v1.1)*

```
Today: 26.12.2025
Deadline: 23.01.2026
Available: 28 days

Phase Estimates:
Phase 2 completion: 1-2 days remaining
Phase 3: 5-7 days
Phase 4: 5-7 days
Phase 5: 6-8 days (extended for upgrades)
Phase 6: 5-7 days
= 22-31 days estimated

Buffer: ~0-6 days (TIGHT)

Risk Assessment:
⚠ Schedule is tight
✓ Mandatory 80 points achievable
⚠ Bonus features require discipline (no scope creep)

Mitigation:
→ Strict phase deadlines
→ Daily progress tracking
→ Cut bonus features if behind schedule
→ JSON fallback ready if PHP/SQL delays
```

---

## 12. RISK ASSESSMENT *(From 03_RISK_MATRIX.md)*

### 12.1 Critical Risks *(Unchanged from v1.0)*

**Risk 1: Bullet Synchronization**  
Probability: Very High (75%)  
Impact: 30 points  
Mitigation: Object pooling, server authority, fire rate limit

**Risk 2: PHP/SQL Backend Failure**  
Probability: Medium (40%)  
Impact: 10 points  
Mitigation: JSON fallback (ADR-006)

**Risk 3: Scope Creep (Bonus Features)**  
Probability: Medium (50%)  
Impact: Indirect (time on mandatory)  
Mitigation: Feature freeze, strict checkpoints

### 12.2 New Risks (v1.1)

**Risk 4: Upgrade System Complexity**  
```
Probability: Low-Medium (30%)
Impact: 5-7 days delay

Symptoms:
- UI complexity underestimated
- Multiplayer sync issues (both players choosing)
- Balance problems (upgrades too weak/strong)

Mitigation:
→ Simple UI (3 buttons only)
→ Independent player choices (no sync needed)
→ Fixed percentage upgrades (easy balance)
→ If delayed: Cut upgrade system entirely (it's bonus)

Contingency:
→ Upgrade system is BONUS feature
→ Can be removed without affecting 80 points
→ Focus on mandatory if time runs short
```

**Risk 5: Tron Visual Implementation Time**  
```
Probability: Low (20%)
Impact: 3-5 days delay

Symptoms:
- Sprite creation takes longer than expected
- Glow effect tuning requires iteration
- Performance issues with bloom

Mitigation:
→ Use simple placeholder sprites initially
→ Bloom is single checkbox (URP Volume)
→ Two-sprite method is fast (no shader debugging)

Contingency:
→ Revert to simple colored sprites (solid fill)
→ Skip glow effect (still looks decent)
→ Visual polish is bonus, not mandatory
```

---

## 13. QUALITY STANDARDS

### 13.1 Code Quality *(Unchanged from v1.0)*

**Academic Attribution Headers**: Required on all new scripts  
**Component-First Methodology**: Setup components before scripts  
**Naming Conventions**: camelCase variables, PascalCase methods/classes

### 13.2 Testing Protocol

**Per-Phase Testing**:
```
[ ] Code compiles error-free
[ ] Both clients tested (Host + Client)
[ ] Console error-free
[ ] Functionality verified
[ ] No regressions
[ ] Performance acceptable (60 FPS)
```

**New Test Scenarios (v1.1)**:
```
Player Customization:
- Name saves/loads correctly
- Color selection applies to sprite
- Preview updates in real-time
- SyncVar updates on both clients

Upgrade System:
- All 5 upgrades apply correctly
- Stacking works (fire rate 3x, etc.)
- UI countdown accurate
- Timeout triggers random upgrade
- Both players can choose independently

Hybrid Progression:
- Wave timer starts/stops correctly
- Speed bonus calculates accurately
- Score updates reflect bonuses
- Timer color-codes properly
```

---

## 14. SUBMISSION REQUIREMENTS

### 14.1 GitHub Repository *(Unchanged from v1.0)*

Required contents:
- Unity project (no build artifacts)
- README.md (mandatory, see below)
- .gitignore (Unity template)
- Clean commit history

### 14.2 README.md Mandatory Sections

```markdown
# Showroom Tango - 2-Player Co-op Bullet-Hell

## Kurzbeschreibung
[2-3 sentences about the game]

## Anleitung zum Starten
**Player Setup:**
1. Enter name (max 9 characters)
2. Choose color (7 options)
3. Click Continue

**Host:**
1. Click "Start Host"
2. Wait for client

**Client:**
1. Click "Start Client"
2. Connection established

## Technischer Überblick

**Verwendete RPCs:**
- ShootServerRpc() - Player shooting
- TakeDamageServerRpc() - Damage application
- SelectUpgradeServerRpc() - Upgrade selection *(NEW)*
- [etc.]

**Verwendete SyncVars:**
- playerName (string) *(NEW)*
- playerColor (Color) *(NEW)*
- currentHP (int)
- playerScore (int)
- [etc.]

**Bullet-Logik:**
[Explanation of bullet spawning, pooling, sync]

**Gegner-Logik:**
[Chaser + Shooter AI explanation]

**Upgrade-System:** *(NEW)*
[Between-wave upgrade selection, 5 options, timeout behavior]

## Persistenz
**Highscore Backend:** PHP/SQL on [server URL]
OR
**Fallback:** JSON file storage (local)

## Umgesetzte Bonusfeatures
1. Upgrade System (7-10 Punkte)
2. Player Customization (3-5 Punkte)
3. Hybrid Progression (3-5 Punkte)
4. Tron Visual Style (5-8 Punkte)
5. [VFX/SFX if implemented]

**Total Bonus Estimate:** 18-28 Punkte (capped at 20)

## Bekannte Bugs
[List any known issues]

## Credits
Developer: Julian
Technical Advisor: Claude AI (architecture, risk analysis)
Visual Concept: Original (Tron-inspired)
```

---

## 15. SUCCESS METRICS

### 15.1 Mandatory Success (80 Points)

```
✓ FishNet multiplayer working
✓ Player movement synchronized
~ Shooting system functional (Phase 3)
~ Hit detection works
~ Enemies spawn in waves
~ HP system functions
~ Score tracking works
~ HUD displays all info
~ Gameflow complete
~ Highscore backend live
~ README.md complete
```

### 15.2 Target Success (90-100 Points)

```
✓ All mandatory criteria met (80 points)
~ Upgrade system polished (7-10 points)
~ Player customization (3-5 points)
~ Tron visuals applied (5-8 points)
~ VFX/SFX implemented (5-10 points)
= 100+ potential points (capped at 100)

Realistic Target: 95 points
- 80 mandatory
- 15 from top bonus features
```

### 15.3 Updated Time Estimates

**Phase 3 (Shooting) - 5-7 days**:
- Bullet prefab creation: 0.5 day
- Two-sprite setup: 0.5 day
- Shooting mechanism: 1 day
- Object pooling: 1 day
- Patterns (2): 1 day
- Hit detection: 1 day
- Testing/polish: 1 day

**Phase 4 (Enemies) - 5-7 days**:
- Enemy prefabs: 1 day
- Chaser AI: 1 day
- Shooter AI: 1.5 days
- Wave spawner: 1 day
- Enemy bullets: 1 day
- Testing/balance: 1 day

**Phase 5 (Gameflow + Upgrades) - 6-8 days**:
- Player Setup UI: 1 day
- HP System: 1 day
- Score + Speed Bonus: 1 day
- Upgrade UI: 1.5 days
- UpgradeManager logic: 1 day
- HUD implementation: 1 day
- Game States: 0.5 day
- Highscore backend: 1-2 days
- Testing: 1 day

**Phase 6 (Polish) - 5-7 days**:
- Bloom setup: 0.5 day
- Sprite creation: 1 day
- UI Tron styling: 1 day
- Particles: 1-2 days
- Sound effects: 1-2 days
- Music: 1 day
- Balance/playtesting: 1 day
- Bug fixes: 1 day
- README: 0.5 day

**Total: 21-29 days**  
**Available: 28 days**  
**Status: TIGHT but ACHIEVABLE**

---

## 16. POST-LAUNCH CONSIDERATIONS *(Out of Scope)*

### 16.1 Potential Expansions

If continued beyond academic submission:
- More enemy types (3-5 total)
- Boss fights (end-of-wave)
- Additional waves (10+ mode)
- Player classes (Tank, DPS, Support)
- Persistent meta-progression
- 3-4 player support
- Dedicated server
- Steam integration

### 16.2 Lessons Learned

*To be filled post-implementation*

---

## 17. GLOSSARY *(Unchanged from v1.0)*

**Technical Terms**:
- NetworkObject, SyncVar, ServerRpc, ClientRpc
- Object Pooling, Server Authority

**Game Terms**:
- Bullet-Hell, Top-Down, Wave, Pattern, Coop, HUD

---

## 18. APPENDICES

### Appendix A: Reference Documents

**Internal**:
- 01_PROJECT_REQUIREMENTS.md
- 02_IMPLEMENTATION_ROADMAP.md
- 03_RISK_MATRIX.md
- 04_ARCHITECTURE_DECISIONS.md
- 06_SETUP_GUIDE.md

**External**:
- FishNet Docs: https://fish-networking.gitbook.io/docs/
- Unity Manual: https://docs.unity3d.com/
- New Input System: https://docs.unity3d.com/Packages/com.unity.inputsystem@latest

### Appendix B: Architecture Decisions (Summary)

```
ADR-001: Wave System ✓ (No boss)
ADR-002: Object Pooling ✓ (Day 1)
ADR-003: Fixed Timestep ✓ (0.02s)
ADR-004: No Client Prediction ✓ (v1.0)
ADR-005: Tugboat Transport ✓
ADR-006: JSON Fallback ✓
ADR-007: New Input System ✓
ADR-008: Prefab-Only NetworkObjects ✓
ADR-009: Server Authority ✓
ADR-010: Academic Code Style ✓
```

### Appendix C: Implementation Status

**Completed (Phase 1-2)**:
```
✓ Unity 6 project created
✓ FishNet imported + configured
✓ Bootstrap scene + NetworkManager
✓ Git repository initialized
✓ Folder structure established
✓ Host/Client connection working
✓ Player prefab (NetworkObject)
✓ Basic movement (WASD)
✓ Input Actions defined
✓ Camera system (Cinemachine)
```

**In Progress (Phase 2)**:
```
~ PlayerController shooting (started)
~ SyncVar integration (HP, Score, Color, Name)
~ Network sync testing
```

**Not Started**:
```
- Bullet system (Phase 3)
- Enemy system (Phase 4)
- Wave management (Phase 4)
- HP system (Phase 5)
- Score system (Phase 5)
- Upgrade system (Phase 5)
- Player Setup UI (Phase 5)
- HUD implementation (Phase 5)
- Gameflow states (Phase 5)
- Highscore backend (Phase 5)
- Tron visuals (Phase 6)
- VFX/SFX (Phase 6)
```

### Appendix D: Visual Specification Quick Reference *(NEW)*

**Color Palette Cheat Sheet**:
```
PLAYERS (Selectable):
Blue    #00D9FF  ████
Magenta #FF00FF  ████
Green   #00FF00  ████
Yellow  #FFFF00  ████
Orange  #FF6600  ████
Cyan    #00FFFF  ████
Purple  #CC00FF  ████

ENEMIES (Fixed):
Red     #FF0033  ████ (Chaser Triangle)
Pink    #FF0099  ████ (Shooter Square)

BULLETS:
White   #FFFFFF  ████ (Player bullets)
Red     #FF0033  ████ (Enemy bullets)

ENVIRONMENT:
Black   #000000  ████ (Background)
D.Cyan  #003333  ████ (Grid, 20% opacity)
```

**Sprite Requirements**:
```
CircleOutline.png    - Players, Bullets (256x256)
TriangleOutline.png  - Chaser Enemy (256x256)
SquareOutline.png    - Shooter Enemy (256x256)

All sprites:
- White base color
- Transparent center
- ~10% outline width
- PNG with alpha
```

**Unity Components Checklist**:
```
Sprite Setup:
[ ] 2x SpriteRenderer per object (glow + core)
[ ] Outer scale 1.1x, Inner scale 1.0x
[ ] Sorting layers configured
[ ] Color applied via SpriteRenderer.color

Post-Processing:
[ ] URP Volume in scene
[ ] Bloom component added
[ ] Intensity: 4-6
[ ] Threshold: 0.3

Trail Effects:
[ ] TrailRenderer on bullets/players
[ ] Gradient: Color → Transparent
[ ] Lifetime: 0.3s
```

---

## DOCUMENT VERSION HISTORY

```
Version 1.0 - 26.12.2025
- Initial GDD creation
- Consolidated project documentation
- Academic requirements integrated
- Technical specifications added

Version 1.1 - 26.12.2025
- Added Section 2.4: Hybrid Progression System
- Added Section 2.5: Simplified Upgrade System
- Added Section 4.2: Player Customization System
- Finalized Section 8.1: Complete color palette
- Added Section 8.2: Rendering implementation
- Updated Section 4.1: Session flow
- Updated Section 11: Extended Phase 5 timeline
- Added Appendix D: Visual specification quick reference
- Updated risk assessment (new risks 4-5)
- Updated test scenarios for new features
- Updated README.md template
- Clarified bonus features point estimates
```

---

## FINAL NOTES

**This GDD is a living document.**

**Next Review**: After Phase 3 completion (Shooting Core)

**Update Triggers**:
- Phase completion
- Design changes
- Risk realization
- New features added
- Performance issues discovered

**All decisions traceable to**:
- Academic requirements (Arbeitsauftrag SRH)
- Technical analysis (AI feasibility studies)
- Risk mitigation (AI risk matrix)
- Best practices (FishNet docs, Unity standards)
- User concept notes (handwritten Tron aesthetic)

**Approval Chain**:
- Concept: Julian ✓
- Technical Architecture: AI-assisted ✓
- Visual Design: Julian ✓
- Progression System: Collaborative (Hybrid) ✓
- Implementation: In Progress

**Document Owner**: Julian  
**Technical Advisor**: Claude AI  
**Next Milestone**: Phase 3 (Shooting Core) completion

---

**END OF GAME DESIGN DOCUMENT v1.1**
