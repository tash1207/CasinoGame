using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    public DialogUI dialogUI;

    private DialogConversation currentConversation;
    private DialogEntry currentDialog;
    private bool isInDialog = false;

    private HashSet<string> completedConversations = new HashSet<string>();

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void StartConversation(DialogConversation conversation)
    {
        if (isInDialog) return;

        currentConversation = conversation;
        bool hasCompleted = completedConversations.Contains(conversation.conversationName);
        currentDialog = conversation.GetStartingDialog(hasCompleted);

        isInDialog = true;
        dialogUI.Show();
        DisplayCurrentDialog();
    }

    private void DisplayCurrentDialog()
    {
        dialogUI.SetSpeakerName(currentDialog.speakerName);
        dialogUI.SetDialogText(currentDialog.dialogText);

        if (currentDialog.HasChoices())
        {
            dialogUI.ShowChoices(currentDialog.responses);
        }

        // TODO: Make the event trigger system more robust and scalable.
        if (currentDialog.eventToTrigger == "GiveMoney")
        {
            // Go from negative money to $30.
            MoneyManager.Instance.AddMoney(30 - MoneyManager.Instance.currentMoney);
        }
    }

    public void SelectResponse(int responseIndex)
    {
        if (!isInDialog || !currentDialog.HasChoices()) return;

        if (responseIndex < 0 || responseIndex >= currentDialog.responses.Count) return;

        DialogResponse selectedResponse = currentDialog.responses[responseIndex];
        if (selectedResponse.eventToTrigger != null)
        {
            // Trigger event
        }
        if (selectedResponse.endsConversation)
        {
            EndConversation();
        }
        else
        {
            currentDialog = selectedResponse.nextDialog;
            DisplayCurrentDialog();
        }
    }

    private void EndConversation()
    {
        if (currentConversation != null)
        {
            completedConversations.Add(currentConversation.conversationName);
        }

        dialogUI.Hide();
        isInDialog = false;
        currentConversation = null;
        currentDialog = null;
    }

}
