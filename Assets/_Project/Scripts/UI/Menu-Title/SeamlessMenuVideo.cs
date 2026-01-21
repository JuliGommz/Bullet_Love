using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Events;

public class SeamlessMenuVideo : MonoBehaviour
{
    [SerializeField] VideoPlayer playerA;
    [SerializeField] VideoPlayer playerB;
    [SerializeField] RawImage displayImage;

    [SerializeField] VideoClip mainClip;
    [SerializeField] VideoClip loopClip;

    [Header("Timing Control")]
    [SerializeField] float preloadOffset = 0.15f;
    [SerializeField] float clipStartOffset = 0f;

    public UnityEvent onVideoSystemReady;

    private RenderTexture rtA;
    private RenderTexture rtB;
    private bool hasPreloaded = false;
    private bool hasSwapped = false;

    void Start()
    {
        // RenderTextures erstellen (match video resolution)
        rtA = new RenderTexture(1920, 1080, 0);
        rtB = new RenderTexture(1920, 1080, 0);

        Debug.Log($"RenderTextures created: A={rtA.IsCreated()}, B={rtB.IsCreated()}");

        // Player A Setup
        playerA.clip = mainClip;
        playerA.isLooping = false;
        playerA.renderMode = VideoRenderMode.RenderTexture;
        playerA.targetTexture = rtA;
        playerA.time = clipStartOffset;

        Debug.Log($"PlayerA clip: {mainClip != null}, targetTexture assigned: {playerA.targetTexture != null}");

        // Player B Setup
        playerB.clip = loopClip;
        playerB.isLooping = true;
        playerB.renderMode = VideoRenderMode.RenderTexture;
        playerB.targetTexture = rtB;

        // Start mit Player A sichtbar
        displayImage.texture = rtA;
        Debug.Log($"RawImage texture assigned: {displayImage.texture != null}, RawImage active: {displayImage.gameObject.activeInHierarchy}");

        playerA.Play();
        playerB.Prepare();

        Debug.Log($"PlayerA playing: {playerA.isPlaying}, isPrepared: {playerA.isPrepared}");

        onVideoSystemReady?.Invoke();
    }


    void Update()
    {
        if (!hasPreloaded && playerA.isPlaying)
        {
            double timeRemaining = playerA.length - playerA.time;

            if (timeRemaining <= preloadOffset)
            {
                playerB.Play();
                hasPreloaded = true;
            }
        }

        // Frame-perfect swap GENAU wenn Player B ready ist
        if (hasPreloaded && !hasSwapped && playerB.isPlaying && playerB.frame > 2)
        {
            displayImage.texture = rtB;
            hasSwapped = true;
            playerA.Stop();
        }
    }

    void OnDestroy()
    {
        rtA?.Release();
        rtB?.Release();
    }
}
