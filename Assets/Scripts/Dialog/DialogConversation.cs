using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog System/Conversation")]
public class DialogConversation : ScriptableObject
{
    [Header("Conversation Info")]
    public string conversationName;

    [Header("Dialog Entries")]
    public DialogEntry startingDialog;

    [Header("Repeatable Dialog")]
    public bool canRepeat = true;
    public DialogEntry repeatDialog;

    public DialogCondition[] conditions;

    public DialogEntry GetStartingDialog(bool hasBeenCompleted)
    {
        if (hasBeenCompleted && !canRepeat && repeatDialog != null)
        {
            return repeatDialog;
        }
        return startingDialog;
    }

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
