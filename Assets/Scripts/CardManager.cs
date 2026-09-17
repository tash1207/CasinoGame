using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] CardDisplay cardDisplay1;
    [SerializeField] CardDisplay cardDisplay2;
    private List<Card> cards;

    void OnEnable()
    {
        InitializeDeck();
        cardDisplay1.SetCard(GetRandomCard());
        cardDisplay2.SetCard(GetRandomCard());
    }

    void InitializeDeck()
    {
        cards = new List<Card>();
        for (int i = 2; i <= 14; i++)
        {
            string cardName = i.ToString();
            switch (i)
            {
                case 11:
                  cardName = "J";
                  break;
                case 12:
                  cardName = "Q";
                  break;
                case 13:
                  cardName = "K";
                  break;
                case 14:
                  cardName = "A";
                  break;
            }
            Card spade = new Card(Card.Suit.SPADE, i, cardName);
            Card heart = new Card(Card.Suit.HEART, i, cardName);
            Card club = new Card(Card.Suit.CLUB, i, cardName);
            Card diamond = new Card(Card.Suit.DIAMOND, i, cardName);

            cards.Add(spade);
            cards.Add(heart);
            cards.Add(club);
            cards.Add(diamond);
        }
    }

    public Card GetRandomCard()
    {
        Debug.Log("num cards left: " + cards.Count);
        int index = Random.Range(0, cards.Count);
        Card card = cards[index];
        cards.Remove(card);
        return card;
    }
}
