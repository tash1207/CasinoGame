using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Hand Cards")]
    [SerializeField] CardDisplay cardDisplay1;
    [SerializeField] CardDisplay cardDisplay2;

    [Header("Table Cards")]
    [SerializeField] GameObject cardHolder;
    [SerializeField] GameObject cardPrefab;

    [Header("Dealer Cards")]
    [SerializeField] GameObject dealerCard1;
    [SerializeField] GameObject dealerCard2;

    [SerializeField] GameObject dealButton;
    [SerializeField] GameObject showHandCanvas;
    [SerializeField] TMP_Text showHandText;

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

        dealerCard1.gameObject.SetActive(true);
        dealerCard2.gameObject.SetActive(true);
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
            cardPrefab, cardHolder.transform.position, Quaternion.identity, cardHolder.transform);
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
    }

    public void ShowHand()
    {
        showHandText.text = "Your Hand:\n" + HandManager.GetHandText(handCards, tableCards);
        showHandCanvas.SetActive(true);

        dealerCard1.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        dealerCard2.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public void Reset()
    {
        handCards.Clear();
        tableCards.Clear();
        InitializeDeck();

        cardDisplay1.gameObject.SetActive(false);
        cardDisplay2.gameObject.SetActive(false);

        dealerCard1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        dealerCard2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        dealerCard1.gameObject.SetActive(false);
        dealerCard2.gameObject.SetActive(false);

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
    }

    public void Exit()
    {
        Reset();
        gameObject.SetActive(false);
    }

}
