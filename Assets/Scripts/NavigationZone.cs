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


    Vector2 arrowCursorHotspot = new Vector2(16, 16); // Center of cursor image
    Vector2 defaultCursorHotspot = new Vector2(14, 11);

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Change to the specific arrow cursor when hovering over this zone
        Cursor.SetCursor(arrowCursor, arrowCursorHotspot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset to default system cursor when leaving the zone
        Cursor.SetCursor(null, defaultCursorHotspot, CursorMode.Auto);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Reset cursor before moving so it doesn't get stuck
        Cursor.SetCursor(null, defaultCursorHotspot, CursorMode.Auto);

        zoneVirtualCamera.Prioritize();
        NavZoneManager.Instance.ChangeView(targetView);
    }

    public void Toggle(bool active)
    {
        gameObject.SetActive(active); 
    }
}
