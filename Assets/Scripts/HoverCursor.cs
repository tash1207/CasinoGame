using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public AudioClip clickSound;
    public Texture2D hoverCursor;
    public bool outlineText;

    Vector2 hoverCursorHotspot = new Vector2(14, 11);
    TMP_Text tmpText;
    float hoverOutlineThickness = 0.75f;
    float normalOutlineThickness = 0.25f;
    Color hoverOutlineColor = Color.red;
    Color normalOutlineColor = Color.black;

    void Awake()
    {
        if (outlineText)
        {
            tmpText = GetComponentInChildren<TMP_Text>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(hoverCursor, hoverCursorHotspot, CursorMode.Auto);
        if (outlineText) SetHoverOutline();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorHelper.ResetCursor();
        if (outlineText) SetNormalOutline();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
            AudioSource.PlayClipAtPoint(clickSound, Vector3.zero, 1.0f);
    }

    void SetHoverOutline()
    {
        if (tmpText != null)
        {
            tmpText.outlineWidth = hoverOutlineThickness;
            tmpText.outlineColor = hoverOutlineColor;
        }
    }

    void SetNormalOutline()
    {
        if (tmpText != null)
        {
            tmpText.outlineWidth = normalOutlineThickness;
            tmpText.outlineColor = normalOutlineColor;
        }
    }
}
