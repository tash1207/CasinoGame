using UnityEngine;

public class DoorClickManager : MonoBehaviour
{
    [SerializeField] AudioClip phoneRingSound;
    [SerializeField] GameObject mapCanvas;
    [SerializeField] DialogConversation[] dialogs;

    void OnEnable()
    {
        CursorHelper.ResetCursor();
        bool hasShownIntro = GameStateManager.Instance.GetFlag(Flags.INTRO_PHONE_CALL);
        if (!hasShownIntro)
        {
            RunIntroSequence();
            gameObject.SetActive(false);
        }
        else
        {
            ShowMap();
        }
    }

    void RunIntroSequence()
    {
        if (phoneRingSound != null && !GameStateManager.Instance.GetFlag(Flags.TRIGGERED_INTRO_PHONE_CALL))
            AudioSource.PlayClipAtPoint(phoneRingSound, Vector3.zero, 1.0f);
        DialogManager.Instance.StartFirstValidConversation(dialogs);
    }

    void ShowMap()
    {
        // Display map canvas
        mapCanvas.SetActive(true);
    }
}
