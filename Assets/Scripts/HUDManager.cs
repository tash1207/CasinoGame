using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [SerializeField] TMP_Text currentMoneyText;

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

    public void ExitGame()
    {
        SceneManager.LoadScene(0); // Main Menu
    }
}
