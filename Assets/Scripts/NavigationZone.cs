using UnityEngine;
using UnityEngine.EventSystems;

public class NavigationZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Cursor Settings")]
    public Texture2D arrowCursor;

    [Header("Movement Settings")]
    public Transform targetWayPoint; // Where the camera should go
    public Transform cameraTransform; // Drag your Main Camera here

    [Header("Navigation Zones")]
    public GameObject[] disableZones;
    public GameObject[] enableZones;

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
        if (targetWayPoint != null && cameraTransform != null)
        {
            // Reset cursor before moving so it doesn't get stuck
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            
            // Move and rotate the camera to the new node
            cameraTransform.position = targetWayPoint.position;
            cameraTransform.rotation = targetWayPoint.rotation;

            UpdateNavigationNodes();
        }
    }

    private void UpdateNavigationNodes()
    {
        foreach (var zone in disableZones)
        {
            zone.SetActive(false);
        }
        foreach (var zone in enableZones)
        {
            zone.SetActive(true);
        }
    }
}
