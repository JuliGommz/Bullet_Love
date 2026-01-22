# Video Background Setup Guide

## Overview
This guide shows how to add the looping video background (`Stars_loop_Background.mp4`) to the game scene using Unity's built-in VideoPlayer component - **no script needed**.

---

## Setup Steps

### 1. Create Background GameObject
1. In Hierarchy, right-click → **Create Empty**
2. Rename to `VideoBackground`
3. Reset Transform (Position: 0, 0, 0)

### 2. Add Video Player Component
1. Select `VideoBackground` GameObject
2. **Add Component** → search for **Video Player**
3. Configure Video Player settings:
   - **Source**: Video Clip
   - **Video Clip**: Drag `Assets/_Project/Media/Video/Stars_loop_Background.mp4`
   - **Render Mode**: Render Texture
   - **Target Texture**: Drag `Assets/_Project/Media/Video/GameBackground_RT` (RenderTexture)
   - **Play On Awake**: ✓ (checked)
   - **Loop**: ✓ (checked)
   - **Playback Speed**: 1
   - **Audio Output Mode**: None (or Direct with Volume = 0)

### 3. Display the Video
You need a visual component to display the RenderTexture.

**Option A: Using a Sprite Renderer (2D Game)**
1. Select `VideoBackground` GameObject
2. **Add Component** → **Sprite Renderer**
3. Configure Sprite Renderer:
   - **Sprite**: Use any white square sprite (or create one)
   - **Material**: Create new Material using `Sprites/Default` shader
   - **Material Texture**: Drag `GameBackground_RT` RenderTexture to material
   - **Sorting Layer**: Create/use "Background" layer
   - **Order in Layer**: -100 (behind everything)
4. Scale GameObject to cover camera view (e.g., Scale: 20, 15, 1)

**Option B: Using UI Raw Image (Canvas-based)**
1. Create Canvas (if not exists): Hierarchy → UI → Canvas
2. Set Canvas **Render Mode**: Screen Space - Overlay
3. Right-click Canvas → UI → **Raw Image**
4. Rename to `VideoBackground`
5. Configure RawImage:
   - **Texture**: Drag `GameBackground_RT` RenderTexture
   - **Rect Transform**: Stretch to full screen (Anchor Presets → bottom-right "stretch" icon)
6. Move Raw Image to **bottom of Canvas** hierarchy (renders first/behind)

---

## Testing
1. Press **Play** in Unity Editor
2. Video should start playing automatically
3. Video should loop seamlessly
4. Video should be behind all gameplay elements

---

## Troubleshooting

**Video not playing:**
- Check VideoPlayer "Play On Awake" is enabled
- Check video file is assigned to Video Clip field
- Check RenderTexture is assigned to Target Texture

**Video not visible:**
- Check Sprite Renderer/RawImage has RenderTexture assigned
- Check Sorting Layer is behind other objects
- Check GameObject scale is large enough to cover camera

**Video not looping:**
- Check VideoPlayer "Loop" checkbox is enabled

**Performance issues:**
- Video resolution should be appropriate (1080p max for backgrounds)
- Consider lower resolution if targeting mobile/low-end hardware

---

## Assets Used
- **Video File**: `Assets/_Project/Media/Video/Stars_loop_Background.mp4`
- **Render Texture**: `Assets/_Project/Media/Video/GameBackground_RT.renderTexture`

---

## Notes
- No custom scripts required - Unity VideoPlayer handles everything
- RenderTexture approach allows video to be used like any texture
- Can be used in 2D (SpriteRenderer) or UI (RawImage)
- Video audio is muted (backgrounds typically don't need sound)
