using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] GameObject canvasToDisplay;
    [SerializeField] bool hasItem;
    [SerializeField] GameObject itemCanvas;

    [SerializeField] TMP_Text titleField;
    [SerializeField] TMP_Text textField;

    [SerializeField] string titleToDisplay;
    [TextArea(2, 5)]
    [SerializeField] string textToDisplay;
    [SerializeField] int distanceFromCameraToBeInteractable;

    Vector2 defaultCursorHotspot = new Vector2(14, 11);
    bool gotItem = false;

    public Texture2D hoverCursor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        Cursor.SetCursor(hoverCursor, defaultCursorHotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        // Reset to default system cursor when leaving the zone
        Cursor.SetCursor(null, defaultCursorHotspot, CursorMode.Auto);
    }

    // Detects clicks and touches
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        //Debug.Log(gameObject.name + " was clicked!");
        if (titleField != null)
        {
            titleField.text = titleToDisplay;
        }
        if (textField != null)
        {
            textField.text = textToDisplay;
        }
        if (hasItem && !gotItem) 
        {
            itemCanvas.SetActive(true);
            MoneyManager.Instance.AddMoney(2);
            gotItem = true;
        }
        canvasToDisplay.SetActive(true);
    }

    bool IsInteractable()
    {
        if (distanceFromCameraToBeInteractable == 0) return true;
        return Vector3.Distance(transform.position, Camera.main.transform.position) <= distanceFromCameraToBeInteractable;
    }
}
