# Development Backlog - Showroom_Tango

## Priority Issues (Next Session)

### 1. Wave Transition Text Not Displaying
**Status:** UNRESOLVED
**Priority:** High

**Problem:**
- WaveTransitionUI.cs implemented with countdown logic
- Debug logs added to track wave state changes
- Text still not showing between waves during gameplay

**Investigation Required:**
- Verify UI GameObject hierarchy in Unity Inspector
- Check if WaveTransitionUI component is attached and enabled
- Confirm countdownPanel and countdownText are assigned in Inspector
- Review Canvas render mode and sorting order
- Test wave completion detection logic with console output
- Verify EnemySpawner.IsWaveActive() state transitions

**Files:**
- `Assets/_Project/Scripts/UI/WaveTransitionUI.cs`
- `Assets/_Project/Scripts/Enemies/EnemySpawner.cs`
- `Assets/_Project/Scenes/Game.unity` (UI setup)

**Documentation:**
- `Documentation/WAVE_TRANSITION_SETUP.md` (setup instructions)

---

### 2. Victory Screen - Restart Button Not Working
**Status:** UNRESOLVED
**Priority:** High

**Problem:**
- Victory screen displays after wave 3 completion
- Restart button does not function when clicked
- May be related to network synchronization issue

**Investigation Required:**
- Check button onClick event configuration in Inspector
- Verify ScoreManager.RestartGame() implementation
- Test if this is a host-only vs client issue (multiplayer)
- Review NetworkManager scene loading for Netcode
- Check for conflicting button interactions or UI blockers

**Files:**
- `Assets/_Project/Scripts/Gameflow/ScoreManager.cs`
- Victory screen UI prefab/scene objects
- Network synchronization for scene restart

---

## Completed Features (Current Version)

- ✅ Wave-based enemy spawning system (3 waves)
- ✅ 2-player local multiplayer support
- ✅ Bullet pooling system for performance
- ✅ Score tracking and HUD display
- ✅ Double-sized game area for 2-player gameplay
- ✅ Video background integration
- ✅ Enemy health and chaser AI

---

**Last Updated:** 2025-01-22
**Version:** 1.9
