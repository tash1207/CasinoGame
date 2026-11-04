using UnityEngine;

public class OpeningLetter : MonoBehaviour
{
    [SerializeField] GameObject letterToDestroy;
    [SerializeField] GameObject envelopeToShow;
    [SerializeField] GameObject letterCanvas;
    [SerializeField] NavZoneManager.NavView targetView;

    Vector2 defaultCursorHotspot = new Vector2(14, 11);

    void Start()
    {
        // TODO: Add voiceover reading letter contents
    }

    public void ClickLetter()
    {
        Destroy(letterToDestroy);
        envelopeToShow.SetActive(true);
        letterCanvas.SetActive(false);

        // Reset cursor before moving so it doesn't get stuck
        Cursor.SetCursor(null, defaultCursorHotspot, CursorMode.Auto);
        NavZoneManager.Instance.ChangeView(targetView);

    }
}
