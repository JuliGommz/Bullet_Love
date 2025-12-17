using UnityEngine;

public class BootstrapPersist : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}