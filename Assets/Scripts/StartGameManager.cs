using Unity.Cinemachine;
using UnityEngine;

public class StartGameManager : MonoBehaviour
{
    public enum Game
    {
        LOW_STAKES_POKER,
        HIGH_STAKES_POKER
    }

    public static StartGameManager Instance { get; private set; }

    [SerializeField] CinemachineCamera pokerCanvasVirtualCamera;
    [SerializeField] NavZoneManager.NavView targetView;

    [Header("Low Stakes Poker")]
    [SerializeField] GameObject cardTableCanvasLowStakes;

    [Header("High Stakes Poker")]
    [SerializeField] GameObject cardTableCanvasHighStakes;

    Vector2 defaultCursorHotspot = new Vector2(14, 11);

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void OnEnable()
    {
        Actions.OnStartGame += StartGame;
    }

    public void OnDisable()
    {
        Actions.OnStartGame -= StartGame;
    }

    void StartGame(Game game)
    {
        switch (game)
        {
            case Game.LOW_STAKES_POKER:
                StartLowStakesPoker();
                break;
            case Game.HIGH_STAKES_POKER:
                StartHighStakesPoker();
                break;
            default:
                break;
        }
    }

    void StartLowStakesPoker()
    {
        cardTableCanvasLowStakes.SetActive(true);
        StartPoker();
    }

    void StartHighStakesPoker()
    {
        cardTableCanvasHighStakes.SetActive(true);
        StartPoker();
    }

    void StartPoker()
    {
        // Reset cursor before moving so it doesn't get stuck
        Cursor.SetCursor(null, defaultCursorHotspot, CursorMode.Auto);
        pokerCanvasVirtualCamera.Prioritize();
        NavZoneManager.Instance.ChangeView(targetView);
    }
}
