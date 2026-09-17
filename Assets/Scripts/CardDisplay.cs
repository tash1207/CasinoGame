using TMPro;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text cardValueText;
    [SerializeField] TMP_Text cardSuitText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCard(Card card)
    {
        Color suitColor;
        switch (card.suit)
        {
            case Card.Suit.SPADE:
                suitColor = Color.black;
                cardSuitText.text = "S";
                break;
            case Card.Suit.HEART:
                suitColor = Color.red;
                cardSuitText.text = "H";
                break;
            case Card.Suit.CLUB:
                suitColor = Color.black;
                cardSuitText.text = "C";
                break;
            case Card.Suit.DIAMOND:
                suitColor = Color.red;
                cardSuitText.text = "D";
                break;
            default:
                return;
        }

        cardValueText.text = card.valueName;

        cardSuitText.color = suitColor;
        cardValueText.color = suitColor;
    }
}
