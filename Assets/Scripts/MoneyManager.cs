using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [SerializeField] TMP_Text currentMoneyText;
    int currentMoney = 30;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentMoneyText.text = "$" + currentMoney;
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
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
