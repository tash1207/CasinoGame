using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private Dictionary<string, bool> gameFlags = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void SetFlag(string flagName, bool value)
    {
        gameFlags[flagName] = value;
    }

    public bool GetFlag(string flagName)
    {
        if (gameFlags.TryGetValue(flagName, out bool value))
        {
            return value;
        }
        return false;
    }

    public void GoToHotelRoom()
    {
        SceneManager.LoadScene(2); // HotelRoom
        CursorHelper.ResetCursor();
    }

    public void GoToCasino()
    {
        SceneManager.LoadScene(1); // Casino
        CursorHelper.ResetCursor();
    }
}
