using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Hand Cards")]
    [SerializeField] CardDisplay cardDisplay1;
    [SerializeField] CardDisplay cardDisplay2;

    [Header("Table Cards")]
    [SerializeField] GameObject cardHolder;
    [SerializeField] GameObject cardPrefab;

    [SerializeField] GameObject dealButton;

    private List<Card> allCards;
    private List<Card> handCards;
    private List<Card> tableCards;

    void OnEnable()
    {
        InitializeDeck();
    }

    void InitializeDeck()
    {
        allCards = new List<Card>();
        handCards = new List<Card>();
        tableCards = new List<Card>();

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

            allCards.Add(spade);
            allCards.Add(heart);
            allCards.Add(club);
            allCards.Add(diamond);
        }
    }

    public Card GetRandomCard()
    {
        //Debug.Log("num cards left: " + allCards.Count);
        int index = Random.Range(0, allCards.Count);
        Card card = allCards[index];
        allCards.Remove(card);
        return card;
    }

    public void DealCards()
    {
        Card randomCard1 = GetRandomCard();
        cardDisplay1.SetCard(randomCard1);
        Card randomCard2 = GetRandomCard();
        cardDisplay2.SetCard(randomCard2);

        handCards.Add(randomCard1);
        handCards.Add(randomCard2);

        cardDisplay1.gameObject.SetActive(true);
        cardDisplay2.gameObject.SetActive(true);
    }

    public void Flop()
    {
        for (int i = 0; i < 3; i++)
        {
            AddTableCard();
        }
    }

    void AddTableCard()
    {
        Card randomCard = GetRandomCard();
        GameObject flopCard = Instantiate(
            cardPrefab, Vector3.zero, Quaternion.identity, cardHolder.transform);
        flopCard.GetComponent<CardDisplay>().SetCard(randomCard);
        tableCards.Add(randomCard);
    }

    public void Turn()
    {
        AddTableCard();
    }

    public void River()
    {
        AddTableCard();
        PrintHands();
    }

    public void Reset()
    {
        cardDisplay1.gameObject.SetActive(false);
        cardDisplay2.gameObject.SetActive(false);

        List<GameObject> cardsToDestroy = new List<GameObject>();
        foreach (Transform child in cardHolder.transform)
        {
            cardsToDestroy.Add(child.gameObject);
        }

        foreach (GameObject card in cardsToDestroy)
        {
            Destroy(card);
        }

        dealButton.SetActive(true);
        gameObject.SetActive(false);
    }

    void PrintHands()
    {
        HandManager.CheckHand(handCards, tableCards);
    }

}
