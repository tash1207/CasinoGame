using UnityEngine;

public class MeshCardDisplay : MonoBehaviour
{
    [Header("Card Meshes")]
    [SerializeField] Mesh[] allSpades;
    [SerializeField] Mesh[] allHearts;
    [SerializeField] Mesh[] allClubs;
    [SerializeField] Mesh[] allDiamonds;

    private MeshFilter meshFilter;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void SetCard(Card card)
    {
        switch (card.suit)
        {
            case Card.Suit.SPADE:
                if (meshFilter)
                {
                    meshFilter.sharedMesh = allSpades[card.value - 2];
                    return;
                }
                break;
            case Card.Suit.HEART:
                if (meshFilter)
                {
                    meshFilter.sharedMesh = allHearts[card.value - 2];
                    return;
                }
                break;
            case Card.Suit.CLUB:
                if (meshFilter)
                {
                    meshFilter.sharedMesh = allClubs[card.value - 2];
                    return;
                }
                break;
            case Card.Suit.DIAMOND:
                if (meshFilter)
                {
                    meshFilter.sharedMesh = allDiamonds[card.value - 2];
                    return;
                }
                break;
            default:
                return;
        }
    }
}
