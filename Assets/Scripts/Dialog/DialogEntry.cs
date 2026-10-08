using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogEntry
{
    public string speakerName;

    [Header("Dialog Content")]
    [TextArea(3, 10)]
    public string dialogText;

    [Header("Conversation Flow")]
    public List<DialogResponse> responses = new List<DialogResponse>();
    public DialogEntry nextDialog;

    [Header("Events")]
    public bool endsConversation = false;
    public string eventToTrigger;

    public bool IsLinearDialog()
    {
        return responses.Count == 0 && nextDialog != null;
    }

    public bool HasChoices()
    {
        return responses.Count > 0;
    }
}
