using UnityEngine;

[System.Serializable]
public class DialogResponse
{
    [TextArea(2, 5)]
    public string responseText;
    public DialogEntry nextDialog;

    public DialogCondition[] conditions;

    public bool endsConversation = false;
    public string eventToTrigger;

    public bool MeetsConditions()
    {
        if (conditions == null || conditions.Length == 0)
            return true;

        foreach (var condition in conditions)
        {
            if (!condition.Evaluate())
                return false;
        }

        return true;
    }

}
