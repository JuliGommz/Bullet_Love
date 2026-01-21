using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAudioController : MonoBehaviour
{
    [SerializeField] AudioSource menuMusicSource;
    [SerializeField] AudioClip menuMusic;

    [Header("Timing Control")]
    [SerializeField] float audioStartOffset = 0f;
    [SerializeField] float audioStartDelay = 0.1f;

    [Header("Fade Settings")]
    [SerializeField] float fadeOutDuration = 1f;

    private bool isFading = false;

    void Awake()
    {
        // Preload Audio komplett in Memory
        menuMusic.LoadAudioData();

        menuMusicSource.clip = menuMusic;
        menuMusicSource.loop = true;
        menuMusicSource.playOnAwake = false;
    }

    public void StartMenuMusic()
    {
        StartCoroutine(DelayedStart());
    }

    System.Collections.IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(audioStartDelay);
        menuMusicSource.time = audioStartOffset;
        menuMusicSource.Play();
    }

    public void OnGameStart()
    {
        if (!isFading)
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    System.Collections.IEnumerator FadeOutAndLoadScene()
    {
        isFading = true;
        float startVolume = menuMusicSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            menuMusicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        menuMusicSource.Stop();
    }
}
