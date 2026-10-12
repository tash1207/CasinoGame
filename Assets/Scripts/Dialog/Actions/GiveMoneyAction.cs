using UnityEngine;

[CreateAssetMenu(fileName = "GiveMoneyAction", menuName = "Scriptable Objects/GiveMoneyAction")]
public class GiveMoneyAction : DialogAction
{
    public int amount = 0;
    public override void Execute()
    {
        if (amount == 0)
        {
            // Go from negative money to $30.
            MoneyManager.Instance.AddMoney(30 - MoneyManager.Instance.currentMoney);
        }
        else
        {
            MoneyManager.Instance.AddMoney(amount);
        }
    }
}
