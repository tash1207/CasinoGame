using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [SerializeField] TMP_Text currentMoneyText;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject volumeSettingsCanvas;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        Actions.CurrentMoney += SetCurrentMoney;
    }

    void OnDisable()
    {
        Actions.CurrentMoney -= SetCurrentMoney;
    }

    void Start()
    {
        currentMoneyText.text = "$" + MoneyManager.Instance.currentMoney;
    }

    void SetCurrentMoney(int currentMoney)
    {
        currentMoneyText.text = "$" + currentMoney;
    }

    public void ShowPauseMenu()
    {
        PauseManager.Instance.SetPaused(true);
        pauseMenu.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        PauseManager.Instance.SetPaused(false);
        pauseMenu.SetActive(false);
    }

    public void ShowVolumeSettings()
    {
        volumeSettingsCanvas.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0); // Main Menu
    }
}
