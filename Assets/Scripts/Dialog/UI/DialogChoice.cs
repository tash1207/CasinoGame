using UnityEngine;
using UnityEngine.EventSystems;

public class DialogChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public AudioClip clickSound;
    public Texture2D hoverCursor;
    Vector2 hoverCursorHotspot = new Vector2(14, 11);

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(hoverCursor, hoverCursorHotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset to default system cursor when leaving the zone
        CursorHelper.ResetCursor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
            AudioSource.PlayClipAtPoint(clickSound, Vector3.zero, 1.0f);
    }
}
