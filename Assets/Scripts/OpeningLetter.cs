using UnityEngine;

public class OpeningLetter : MonoBehaviour
{
    [SerializeField] GameObject letterToDestroy;
    [SerializeField] GameObject envelopeToShow;
    [SerializeField] GameObject letterCanvas;
    [SerializeField] NavZoneManager.NavView targetView;

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
        CursorHelper.ResetCursor();
        NavZoneManager.Instance.ChangeView(targetView);
    }
}
