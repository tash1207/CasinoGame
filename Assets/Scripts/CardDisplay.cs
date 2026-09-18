using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text cardValueText;
    [SerializeField] Image cardSuitImage;

    [SerializeField] Sprite spadeSprite;
    [SerializeField] Sprite heartSprite;
    [SerializeField] Sprite clubSprite;
    [SerializeField] Sprite diamondSprite;

    public void SetCard(Card card)
    {
        Color suitColor;
        switch (card.suit)
        {
            case Card.Suit.SPADE:
                suitColor = Color.black;
                cardSuitImage.sprite = spadeSprite;
                break;
            case Card.Suit.HEART:
                suitColor = Color.red;
                cardSuitImage.sprite = heartSprite;
                break;
            case Card.Suit.CLUB:
                suitColor = Color.black;
                cardSuitImage.sprite = clubSprite;
                break;
            case Card.Suit.DIAMOND:
                suitColor = Color.red;
                cardSuitImage.sprite = diamondSprite;
                break;
            default:
                return;
        }

        cardValueText.text = card.valueName;
        cardValueText.color = suitColor;
    }
}
