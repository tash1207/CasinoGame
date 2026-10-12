using UnityEngine;
using UnityEngine.EventSystems;

public class DialogChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Texture2D hoverCursor;
    Vector2 defaultCursorHotspot = new Vector2(14, 11);

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(hoverCursor, defaultCursorHotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset to default system cursor when leaving the zone
        Cursor.SetCursor(null, defaultCursorHotspot, CursorMode.Auto);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // TODO: Add click sound
    }
}
