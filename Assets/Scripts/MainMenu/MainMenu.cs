using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(2); // HotelRoom
        CursorHelper.ResetCursor();
    }

    public void CasinoMode()
    {
        SceneManager.LoadScene(1); // Casino
        CursorHelper.ResetCursor();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
