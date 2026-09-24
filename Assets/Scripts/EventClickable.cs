using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] GameObject canvasToDisplay;

    [SerializeField] TMP_Text titleField;
    [SerializeField] TMP_Text textField;

    [SerializeField] string titleToDisplay;
    [TextArea(2, 5)]
    [SerializeField] string textToDisplay;


    Vector2 defaultCursorHotspot = new Vector2(14, 11);

    public Texture2D hoverCursor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(hoverCursor, defaultCursorHotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset to default system cursor when leaving the zone
        Cursor.SetCursor(null, defaultCursorHotspot, CursorMode.Auto);
    }

    // Detects clicks and touches
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log(gameObject.name + " was clicked!");
        if (titleField != null)
        {
            titleField.text = titleToDisplay;
        }
        if (textField != null)
        {
            textField.text = textToDisplay;
        }
        canvasToDisplay.SetActive(true);
    }
}
