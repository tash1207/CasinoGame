using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    Vector2 defaultCursorHotspot = new Vector2(14, 11);

    public void StartGame()
    {
        SceneManager.LoadScene(2); // HotelRoom
        ResetCursor();
    }

    public void CasinoMode()
    {
        SceneManager.LoadScene(1); // Casino
        ResetCursor();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void ResetCursor()
    {
        Cursor.SetCursor(null, defaultCursorHotspot, CursorMode.Auto);
    }
}
