using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [SerializeField] TMP_Text currentMoneyText;

    public int currentMoney { get; private set; }

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            currentMoney = 30;
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        currentMoneyText.text = "$" + currentMoney;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        currentMoneyText.text = "$" + currentMoney;
    }

    public void LoseMoney(int amount)
    {
        currentMoney -= amount;
        currentMoneyText.text = "$" + currentMoney;
    }
}
