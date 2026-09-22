using System.Collections.Generic;
using UnityEngine;

public class NavZoneManager : MonoBehaviour
{
    public enum NavView
    {
        CasinoFacingPoker,
        PokerTable,
        CasinoFacingRight,
        GuitarDetail,
        PokerTableCanvas
    }

    [Header("Current State")]
    [SerializeField] NavView currentView = NavView.CasinoFacingPoker;

    public static NavZoneManager Instance { get; private set; }
    
    private List<NavigationZone> allNavZones = new List<NavigationZone>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        allNavZones.AddRange(FindObjectsByType<NavigationZone>(FindObjectsSortMode.None));
    }

    private void Start()
    {
        UpdateZones();
    }

    public void ChangeView(NavView newView)
    {
        if (currentView == newView) return;

        currentView = newView;
        UpdateZones();
    }

    private void UpdateZones()
    {
        foreach (var navZone in allNavZones)
        {
            bool isRelevant = navZone.belongsToView == currentView;
            navZone.Toggle(isRelevant);
        }
    }
}
