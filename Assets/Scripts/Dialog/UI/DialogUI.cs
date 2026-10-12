using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    public GameObject dialogPanel;

    [Header("Dialog Display")]
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogText;
    public float typingSpeed = 0.05f;
    private Coroutine typingCoroutine;

    [Header("Choices")]
    public GameObject dialogChoiceContainer;
    public GameObject dialogChoicePrefab;
    private List<GameObject> activeDialogChoices = new List<GameObject>();


    void Awake()
    {
        dialogPanel.SetActive(false);
    }

    public void Show()
    {
        dialogPanel.SetActive(true);
    }

    public void Hide()
    {
        dialogPanel.SetActive(false);
    }

    public void SetSpeakerName(string name)
    {
        speakerNameText.text = name;
    }

    public void SetDialogText(string text)
    {
        dialogText.text = text;
        // if (typingCoroutine != null)
        // {
        //     StopCoroutine(typingCoroutine);
        // }
        // typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        dialogText.text = "";

        foreach (char letter in text)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;
    }

    public void ShowChoices(List<DialogResponse> responses)
    {
        HideChoices();

        List<DialogResponse> availableResponses = new List<DialogResponse>();
        foreach (var response in responses)
        {
            if (response.MeetsConditions())
            {
                availableResponses.Add(response);
            }
        }

        for (int i = 0; i < availableResponses.Count; i++)
        {
            GameObject choiceObj = Instantiate(dialogChoicePrefab, dialogChoiceContainer.transform);
            TextMeshProUGUI choiceText = choiceObj.GetComponent<TextMeshProUGUI>();
            choiceText.text = availableResponses[i].responseText;

            int responseIndex = responses.IndexOf(availableResponses[i]);
            Button button = choiceObj.GetComponent<Button>();
            button.onClick.AddListener(() => OnChoiceSelected(responseIndex));

            activeDialogChoices.Add(choiceObj);
        }

        dialogChoiceContainer.SetActive(true);
    }

    private void HideChoices()
    {
        foreach (var dialogChoice in activeDialogChoices)
        {
            Destroy(dialogChoice);
        }
        activeDialogChoices.Clear();
        dialogChoiceContainer.SetActive(false);
    }

    private void OnChoiceSelected(int index)
    {
        DialogManager.Instance.SelectResponse(index);
    }
}
