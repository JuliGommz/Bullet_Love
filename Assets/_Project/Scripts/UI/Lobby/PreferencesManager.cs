using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PreferencesManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider fxSlider;

    [Header("Difficulty")]
    public Button beginnerButton;
    public Button advancedButton;

    void Start()
    {
        LoadPreferences();
    }

    // Music Volume Control
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    // FX Volume Control
    public void SetFXVolume(float volume)
    {
        audioMixer.SetFloat("FXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("FXVolume", volume);
    }

    // Difficulty: Tango Beginner
    public void SetDifficultyBeginner()
    {
        PlayerPrefs.SetInt("Difficulty", 0);
        Debug.Log("Difficulty set to: Tango Beginner");
    }

    // Difficulty: Tango Advanced
    public void SetDifficultyAdvanced()
    {
        PlayerPrefs.SetInt("Difficulty", 1);
        Debug.Log("Difficulty set to: Tango Advanced");
    }

    void LoadPreferences()
    {
        // Load Music Volume
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVol = PlayerPrefs.GetFloat("MusicVolume");
            musicSlider.value = musicVol;
            SetMusicVolume(musicVol);
        }

        // Load FX Volume
        if (PlayerPrefs.HasKey("FXVolume"))
        {
            float fxVol = PlayerPrefs.GetFloat("FXVolume");
            fxSlider.value = fxVol;
            SetFXVolume(fxVol);
        }

        // Load Difficulty
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            int diff = PlayerPrefs.GetInt("Difficulty");
            // Apply difficulty to game logic
        }
    }
}
