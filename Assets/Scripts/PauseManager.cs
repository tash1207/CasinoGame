using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    bool isPaused = false;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void SetPaused(bool value)
    {
        isPaused = value;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
