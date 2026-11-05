using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public int currentMoney { get; private set; }

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            currentMoney = 30;
        }
        else Destroy(gameObject);
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Actions.CurrentMoney(currentMoney);
    }

    public void LoseMoney(int amount)
    {
        currentMoney -= amount;
        Actions.CurrentMoney(currentMoney);
    }
}
