using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(2); // HotelRoom
    }

    public void CasinoMode()
    {
        SceneManager.LoadScene(1); // Casino
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
