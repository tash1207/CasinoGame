using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [SerializeField] TMP_Text currentMoneyText;
    public int currentMoney = 100;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
