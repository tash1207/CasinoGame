using UnityEngine;

public enum ConditionType
{
    HasTask,
    HasItem,
    HasMoney,
    GameFlag
}

[System.Serializable]
public class DialogCondition
{
    public ConditionType conditionType;
    public string conditionID;
    public int requiredValue;
    public ItemManager.Item requiredItem;
    public bool invertCondition; // Check if condition is NOT met

    public bool Evaluate()
    {
        bool result = false;

        switch (conditionType)
        {
            case ConditionType.HasTask:
                break;
            case ConditionType.HasItem:
                result = ItemManager.Instance.HasItem(requiredItem);
                break;
            case ConditionType.HasMoney:
                if (requiredValue < 0)
                {
                    result = MoneyManager.Instance.currentMoney <= 0;
                }
                else
                {
                    result = MoneyManager.Instance.currentMoney >= requiredValue;
                }
                break;
            case ConditionType.GameFlag:
                result = GameStateManager.Instance.GetFlag(conditionID);
                break;
        }

        return invertCondition ? !result : result;
    }
}
