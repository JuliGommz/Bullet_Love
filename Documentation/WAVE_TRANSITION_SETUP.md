# Wave Transition Countdown - Setup Guide

## Overview
Adds visual countdown between waves: **"NEXT WAVE IN 5... 4... 3... 2... 1..."**

---

## Unity Editor Setup (5 minutes)

### 1. Create Countdown Panel

**In HUD_Canvas hierarchy:**

1. Right-click `HUD_Canvas` → UI → **Panel**
2. Rename to `WaveCountdownPanel`
3. **Rect Transform** (stretch to full screen):
   - **Anchors**: Stretch/Stretch (bottom-right preset + hold ALT)
   - **Left/Right/Top/Bottom**: All 0
   - **Width/Height**: Will auto-adjust
4. **Image Component**:
   - **Color**: Black with alpha 0.5 (R:0, G:0, B:0, A:128) - semi-transparent
   - **Raycast Target**: ✓ (blocks clicks during countdown)

---

### 2. Create Countdown Text

**As child of WaveCountdownPanel:**

1. Right-click `WaveCountdownPanel` → UI → **Text - TextMeshPro**
2. Rename to `CountdownText`
3. **Rect Transform** (centered):
   - **Anchors**: Center (middle preset)
   - **Position**: X:0, Y:0, Z:0
   - **Width**: 600
   - **Height**: 150
4. **TextMeshProUGUI Component**:
   - **Text**: "NEXT WAVE IN 5..." (placeholder)
   - **Font**: Roboto Bold SDF (same as other UI)
   - **Font Size**: 60
   - **Alignment**: Center + Middle
   - **Color**: Magenta (R:170, G:0, B:200, A:255) - #AA00C8
   - **Wrapping**: Disabled
   - **Overflow**: Overflow
   - **Auto Size**: Enabled (Min: 40, Max: 80)

---

### 3. Add WaveTransitionUI Script

**On HUD_Canvas GameObject (or create new GameObject):**

1. Select `HUD_Canvas` GameObject
2. **Add Component** → `WaveTransitionUI`
3. Configure script:
   - **Countdown Panel**: Drag `WaveCountdownPanel` GameObject
   - **Countdown Text**: Drag `CountdownText` TextMeshPro component
   - **Countdown Duration**: 5 (matches wave delay)
   - **Text Color**: Magenta RGB(170, 0, 200)

---

### 4. Set Display Order

**Ensure countdown appears on top:**

1. Select `WaveCountdownPanel`
2. In Hierarchy, drag it to **bottom of HUD_Canvas children** (renders last = on top)
3. Or use **Canvas component** on panel:
   - Add Canvas component
   - **Override Sorting**: ✓
   - **Sort Order**: 100

---

## Testing

1. **Play the game**
2. Complete Wave 1
3. **Should see**: "NEXT WAVE IN 5... 4... 3... 2... 1..."
4. Wave 2 starts automatically
5. Repeat for Wave 2 → Wave 3

**Expected Behavior**:
- Countdown appears after Wave 1 and Wave 2 complete
- Each number displays for 1 second (5 seconds total)
- Countdown does NOT appear before Wave 1 (game starts immediately)
- Countdown does NOT appear after Wave 3 (victory screen should show)

---

## Customization

### Change Colors
- **Cyan**: RGB(0, 179, 255) #00B3FF
- **Yellow**: RGB(255, 255, 0) #FFFF00
- **Green**: RGB(57, 255, 20) #39FF14

### Change Background Opacity
- **Panel Image → Color → A**: 0-255 (0=invisible, 255=solid)
- Recommended: 128 (50% transparent)

### Change Font Size
- **CountdownText → Font Size**: 40-100
- Or enable **Auto Size** (adjusts to fit)

### Change Countdown Duration
- **WaveTransitionUI → Countdown Duration**: Must match `EnemySpawner` wave delay
- Current: 5 seconds (don't change unless you also change EnemySpawner)

---

## Troubleshooting

**Countdown not appearing**:
- Check WaveTransitionUI script is added to scene
- Verify Countdown Panel and Text are assigned in Inspector
- Check panel is child of HUD_Canvas
- Ensure panel starts hidden (WaveTransitionUI sets this)

**Countdown timing wrong**:
- Verify Countdown Duration = 5 seconds
- Check EnemySpawner wave delay hasn't changed

**Text not visible**:
- Check text color alpha is 255 (fully opaque)
- Verify panel background isn't blocking text
- Check font size is large enough (60+)

**Countdown shows before Wave 1**:
- This is correct - countdown only shows between waves
- Wave 1 starts immediately after 2-second initial delay

---

## Technical Details

**How It Works**:
1. WaveTransitionUI monitors EnemySpawner wave number
2. When wave increments but isn't active = wave completed
3. Triggers coroutine to show countdown
4. Each second, updates text: "NEXT WAVE IN X..."
5. After 5 seconds, hides panel
6. Next wave starts (EnemySpawner handles timing)

**No changes to wave timing** - countdown purely visual feedback.

---

## Color Palette Reference

| Color   | RGB             | Hex     | Usage            |
|---------|-----------------|---------|------------------|
| Magenta | (170, 0, 200)   | #AA00C8 | Default text     |
| Cyan    | (0, 179, 255)   | #00B3FF | Alternative      |
| Yellow  | (255, 255, 0)   | #FFFF00 | Warning/emphasis |
| Green   | (57, 255, 20)   | #39FF14 | Success          |
