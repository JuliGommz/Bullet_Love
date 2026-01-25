/*
====================================================================
* HighscoreManager.cs - PHP/SQL Highscore Integration
====================================================================
* Project: Showroom_Tango
* Course: PRG - Game & Multimedia Design SRH Hochschule
* Developer: Julian Gomez
* Date: 2025-01-20
* Version: 1.0
*
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
*
* [HUMAN-AUTHORED]
* - PHP/SQL backend requirement (mandatory)
* - Top 10 highscore list
*
* [AI-ASSISTED]
* - UnityWebRequest implementation
* - JSON serialization/deserialization
* - Async callback pattern
*
* [AI-GENERATED]
* - Complete backend integration logic
*
* BACKEND SETUP REQUIRED:
* 1. PHP scripts (submit_score.php, get_highscores.php)
* 2. MySQL database table
* 3. Web hosting or local XAMPP
====================================================================
*/

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class HighscoreEntry
{
    public string playerName;
    public int score;
    public string timestamp;
}

[System.Serializable]
public class HighscoreList
{
    public List<HighscoreEntry> entries;
}

public class HighscoreManager : MonoBehaviour
{
    public static HighscoreManager Instance { get; private set; }

    [Header("Backend Configuration")]
    [SerializeField] private string submitScoreURL = "http://localhost/bullethell/submit_score.php";
    [SerializeField] private string getHighscoresURL = "http://localhost/bullethell/get_highscores.php";

    [Header("Fallback: JSON Local Storage")]
    [SerializeField] private bool useJSONFallback = false;
    [SerializeField] private string jsonFilePath = "highscores.json";

    private List<HighscoreEntry> cachedHighscores = new List<HighscoreEntry>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Ensure root-level GameObject
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Submit score to PHP backend or JSON file
    /// </summary>
    public void SubmitScore(string playerName, int score)
    {
        if (useJSONFallback)
        {
            SubmitScoreJSON(playerName, score);
        }
        else
        {
            StartCoroutine(SubmitScorePHP(playerName, score));
        }
    }

    /// <summary>
    /// Get highscores from PHP backend or JSON file
    /// </summary>
    public void LoadHighscores(System.Action<List<HighscoreEntry>> callback)
    {
        if (useJSONFallback)
        {
            callback?.Invoke(LoadHighscoresJSON());
        }
        else
        {
            StartCoroutine(LoadHighscoresPHP(callback));
        }
    }

    // ======== PHP/SQL BACKEND ========

    private IEnumerator SubmitScorePHP(string playerName, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("player_name", playerName);
        form.AddField("score", score);

        using (UnityWebRequest www = UnityWebRequest.Post(submitScoreURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[HighscoreManager] Score submitted: {playerName} - {score}");
                Debug.Log($"[HighscoreManager] Response: {www.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"[HighscoreManager] Submit failed: {www.error}");
                Debug.LogWarning("[HighscoreManager] Consider enabling JSON fallback in Inspector");
            }
        }
    }

    private IEnumerator LoadHighscoresPHP(System.Action<List<HighscoreEntry>> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(getHighscoresURL))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log($"[HighscoreManager] Received: {json}");

                // Parse JSON (format: {"entries": [...]})
                HighscoreList list = JsonUtility.FromJson<HighscoreList>($"{{\"entries\":{json}}}");

                if (list != null && list.entries != null)
                {
                    cachedHighscores = list.entries;
                    callback?.Invoke(cachedHighscores);
                }
                else
                {
                    Debug.LogWarning("[HighscoreManager] Failed to parse highscores");
                    callback?.Invoke(new List<HighscoreEntry>());
                }
            }
            else
            {
                Debug.LogError($"[HighscoreManager] Load failed: {www.error}");
                callback?.Invoke(new List<HighscoreEntry>());
            }
        }
    }

    // ======== JSON FALLBACK ========

    private void SubmitScoreJSON(string playerName, int score)
    {
        List<HighscoreEntry> entries = LoadHighscoresJSON();

        HighscoreEntry newEntry = new HighscoreEntry
        {
            playerName = playerName,
            score = score,
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        entries.Add(newEntry);

        // Sort by score descending
        entries.Sort((a, b) => b.score.CompareTo(a.score));

        // Keep top 10
        if (entries.Count > 10)
        {
            entries.RemoveRange(10, entries.Count - 10);
        }

        // Save to file
        HighscoreList list = new HighscoreList { entries = entries };
        string json = JsonUtility.ToJson(list, true);

        string path = System.IO.Path.Combine(Application.persistentDataPath, jsonFilePath);
        System.IO.File.WriteAllText(path, json);

        Debug.Log($"[HighscoreManager] Score saved to JSON: {playerName} - {score}");
        cachedHighscores = entries;
    }

    private List<HighscoreEntry> LoadHighscoresJSON()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, jsonFilePath);

        if (!System.IO.File.Exists(path))
        {
            return new List<HighscoreEntry>();
        }

        string json = System.IO.File.ReadAllText(path);
        HighscoreList list = JsonUtility.FromJson<HighscoreList>(json);

        return list != null && list.entries != null ? list.entries : new List<HighscoreEntry>();
    }

    // ======== PUBLIC GETTERS ========

    public List<HighscoreEntry> GetCachedHighscores() => cachedHighscores;
}
