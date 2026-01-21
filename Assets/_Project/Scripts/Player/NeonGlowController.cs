/*
====================================================================
* NeonGlowController.cs - Dual-Sprite Neon-Glow System
====================================================================
* Project: Showroom_Tango
* Course: PRG - Game & Multimedia Design SRH Hochschule
* Developer: Julian Gomez
* Date: 2025-01-08
* Version: 1.1 - Sprite-swapping system for prototype
* 
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* 
* [HUMAN-AUTHORED]
* - Design decision: 3 colors for prototype (Blue/Green/Yellow)
* - Sprite-swapping approach instead of tinting
* 
* [AI-ASSISTED]
* - Implementation of sprite reference system
* - Color-to-sprite mapping logic
====================================================================
*/

using UnityEngine;

public class NeonGlowController : MonoBehaviour
{
    [Header("Sprite References")]
    [SerializeField] private SpriteRenderer fillRenderer;
    [SerializeField] private SpriteRenderer glowRenderer;

    [Header("UFO Sprite Variants (Prototype: 3 colors)")]
    [SerializeField] private Sprite ufoBlue;
    [SerializeField] private Sprite ufoGreen;
    [SerializeField] private Sprite ufoYellow;

    [Header("Glow Colors (Adjustable)")]
    [Tooltip("Blue glow color - adjust here, not in GlowSprite component")]
    [SerializeField] private Color blueGlowColor = new Color(0f, 0.7f, 1f);
    [Tooltip("Green glow color - adjust here, not in GlowSprite component")]
    [SerializeField] private Color greenGlowColor = new Color(0.224f, 1f, 0.078f);
    [Tooltip("Yellow glow color - adjust here, not in GlowSprite component")]
    [SerializeField] private Color yellowGlowColor = new Color(1f, 1f, 0f);

    [Header("Glow Settings")]
    [SerializeField] private NeonColorType selectedColor = NeonColorType.Blue;
    [SerializeField][Range(1.0f, 1.5f)] private float glowScaleMultiplier = 1.15f;
    [SerializeField][Range(0f, 1f)] private float glowAlpha = 0.8f;

    [Header("Pulsing Animation")]
    [SerializeField] private bool enablePulsing = false;
    [SerializeField][Range(0.1f, 5f)] private float pulsingSpeed = 1f;

    public enum NeonColorType
    {
        Blue,
        Green,
        Yellow
    }

    private float pulseTimer = 0f;
    private Vector3 baseGlowScale;

    void Start()
    {
        if (fillRenderer == null || glowRenderer == null)
        {
            Debug.LogError("Missing SpriteRenderer references!");
            enabled = false;
            return;
        }

        ApplyColorChoice(selectedColor);
        glowRenderer.sortingOrder = fillRenderer.sortingOrder - 1;
        baseGlowScale = glowRenderer.transform.localScale * glowScaleMultiplier;
        glowRenderer.transform.localScale = baseGlowScale;
    }

    void Update()
    {
        if (enablePulsing)
        {
            UpdatePulsing();
        }
    }

    private void UpdatePulsing()
    {
        pulseTimer += Time.deltaTime * pulsingSpeed;
        float scale = 1f + Mathf.Sin(pulseTimer * Mathf.PI * 2f) * 0.1f;
        glowRenderer.transform.localScale = baseGlowScale * scale;

        Color glowColor = glowRenderer.color;
        glowColor.a = glowAlpha * scale;
        glowRenderer.color = glowColor;
    }

    /// <summary>
    /// Applies color choice by swapping sprite and setting glow color
    /// </summary>
    public void ApplyColorChoice(NeonColorType colorType)
    {
        selectedColor = colorType;

        // Swap sprite based on color
        switch (colorType)
        {
            case NeonColorType.Blue:
                if (ufoBlue != null)
                {
                    fillRenderer.sprite = ufoBlue;
                    glowRenderer.sprite = ufoBlue;
                }
                SetGlowColor(blueGlowColor); // Uses Inspector value
                break;

            case NeonColorType.Green:
                if (ufoGreen != null)
                {
                    fillRenderer.sprite = ufoGreen;
                    glowRenderer.sprite = ufoGreen;
                }
                SetGlowColor(greenGlowColor); // Uses Inspector value
                break;

            case NeonColorType.Yellow:
                if (ufoYellow != null)
                {
                    fillRenderer.sprite = ufoYellow;
                    glowRenderer.sprite = ufoYellow;
                }
                SetGlowColor(yellowGlowColor); // Uses Inspector value
                break;
        }

        // Fill sprite stays untinted
        fillRenderer.color = Color.white;
    }

    private void SetGlowColor(Color glowColor)
    {
        glowColor.a = glowAlpha;
        glowRenderer.color = glowColor;
    }

    private void OnValidate()
    {
        if (fillRenderer != null && glowRenderer != null)
        {
            ApplyColorChoice(selectedColor);
        }
    }
}