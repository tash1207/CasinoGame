using UnityEngine;
using UnityEngine.EventSystems;

public class DialogTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] DialogConversation[] dialogs;
    [SerializeField] int distanceFromCameraToBeInteractable;

    Vector2 defaultCursorHotspot = new Vector2(14, 11);
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

        Cursor.SetCursor(null, defaultCursorHotspot, CursorMode.Auto);
        DialogManager.Instance.StartFirstValidConversation(dialogs);
    }

    bool IsInteractable()
    {
        if (!HasDialog()) return false;
        if (distanceFromCameraToBeInteractable == 0) return true;
        return Vector3.Distance(transform.position, Camera.main.transform.position) <= distanceFromCameraToBeInteractable;
    }

    bool HasDialog()
    {
        bool hasDialog = false;
        foreach (var dialog in dialogs)
        {
            if (dialog.MeetsConditions())
            {
                hasDialog = true;
                break;
            }
        }
        return hasDialog;
    }
}
