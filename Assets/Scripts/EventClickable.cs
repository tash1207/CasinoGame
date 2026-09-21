using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] GameObject canvasToDisplay;

    [SerializeField] TMP_Text titleField;
    [SerializeField] TMP_Text textField;

    [SerializeField] string titleToDisplay;
    [SerializeField] string textToDisplay;


    Vector2 cursorHotspot = new Vector2(16, 16); // Center of cursor image

    public Texture2D hoverCursor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(hoverCursor, cursorHotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset to default system cursor when leaving the zone
        Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
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
