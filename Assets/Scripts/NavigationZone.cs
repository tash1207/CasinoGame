using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class NavigationZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Cursor Settings")]
    [SerializeField] Texture2D arrowCursor;

    [Header("Movement Settings")]
    [SerializeField] CinemachineCamera zoneVirtualCamera;

    [Header("Navigation Zones")]
    public NavZoneManager.NavView belongsToView;
    [SerializeField] NavZoneManager.NavView targetView;


    Vector2 cursorHotspot = new Vector2(16, 16); // Center of cursor image

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Change to the specific arrow cursor when hovering over this zone
        Cursor.SetCursor(arrowCursor, cursorHotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset to default system cursor when leaving the zone
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Reset cursor before moving so it doesn't get stuck
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        zoneVirtualCamera.Prioritize();
        NavZoneManager.Instance.ChangeView(targetView);
    }

    public void Toggle(bool active)
    {
        gameObject.SetActive(active); 
    }
}
