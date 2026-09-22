using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Header("Hand Cards")]
    [SerializeField] MeshCardDisplay handCard1;
    [SerializeField] MeshCardDisplay handCard2;

    [Header("Table Cards")]
    [SerializeField] GameObject cardHolder;
    [SerializeField] GameObject cardPrefab;

    [Header("Dealer Cards")]
    [SerializeField] MeshCardDisplay dealerCard1;
    [SerializeField] MeshCardDisplay dealerCard2;

    [SerializeField] GameObject dealButton;
    [SerializeField] GameObject showHandCanvas;
    [SerializeField] TMP_Text showHandText;
    [SerializeField] TMP_Text showDealerHandText;

    private List<Card> allCards;
    private List<Card> handCards;
    private List<Card> tableCards;
    private List<Card> dealerCards;

    private Hand yourHand;
    private Hand dealersHand;

    private List<GameObject> tableCardDisplays;

    void OnEnable()
    {
        InitializeDeck();
    }

    void InitializeDeck()
    {
        allCards = new List<Card>();
        handCards = new List<Card>();
        tableCards = new List<Card>();
        dealerCards = new List<Card>();

        tableCardDisplays = new List<GameObject>();

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
        //Debug.Log("Dealing a " + card.valueName + " of " + card.GetSuitName());
        allCards.Remove(card);
        return card;
    }

    public void DealCards()
    {
        Card randomCard1 = GetRandomCard();
        Card randomCard2 = GetRandomCard();
        Card randomCard3 = GetRandomCard();
        Card randomCard4 = GetRandomCard();

        handCard1.transform.parent.gameObject.SetActive(true);
        handCard2.transform.parent.gameObject.SetActive(true);
        dealerCard1.transform.parent.gameObject.SetActive(true);
        dealerCard2.transform.parent.gameObject.SetActive(true);

        handCards.Add(randomCard1);
        handCard1.SetCard(randomCard1);
        
        dealerCards.Add(randomCard2);
        dealerCard1.SetCard(randomCard2);

        handCards.Add(randomCard3);
        handCard2.SetCard(randomCard3);

        dealerCards.Add(randomCard4);
        dealerCard2.SetCard(randomCard4);
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
        GameObject tableCardDisplay = Instantiate(
            cardPrefab, cardHolder.transform.position, Quaternion.identity, cardHolder.transform);
        tableCardDisplay.GetComponentInChildren<MeshCardDisplay>().SetCard(randomCard);
        tableCards.Add(randomCard);
        tableCardDisplays.Add(tableCardDisplay);
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
        yourHand = HandManager.GetHand(handCards, tableCards);
        dealersHand = HandManager.GetHand(dealerCards, tableCards);

        if (yourHand.score > dealersHand.score)
        {
            HighlightYourCards();
            Debug.Log("You won!");
        }
        else if (yourHand.score == dealersHand.score)
        {
            HighlightYourCards();
            Debug.Log("TIE");
        } else
        {
            HighlightDealersCards();
            Debug.Log("You lose");
        }

        showHandText.text = "Your Hand:\n" + yourHand.handText;
        showDealerHandText.text = "Dealer's Hand:\n" + dealersHand.handText;
        showHandCanvas.SetActive(true);

        dealerCard1.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        dealerCard2.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public void HighlightYourCards()
    {
        ResetHighlightedCards();
        foreach (var tableCardDisplay in tableCardDisplays)
        {
            Card tableCard = tableCardDisplay.GetComponentInChildren<MeshCardDisplay>().GetCard();
            if (yourHand.handCards.Contains(tableCard))
            {
                Outline outline = tableCardDisplay.GetComponent<Outline>();
                outline.enabled = true;
                outline.effectColor = new Color(1f, 0f, 1f, 1f);
            }
        }

        if (yourHand.handCards.Contains(handCard1.GetCard()))
        {
            handCard1.gameObject.GetComponentInParent<Outline>().enabled = true;
        }
        if (yourHand.handCards.Contains(handCard2.GetCard()))
        {
            handCard2.gameObject.GetComponentInParent<Outline>().enabled = true;
        }
    }

    public void HighlightDealersCards()
    {
        ResetHighlightedCards();
        foreach (var tableCardDisplay in tableCardDisplays)
        {
            Card tableCard = tableCardDisplay.GetComponentInChildren<MeshCardDisplay>().GetCard();
            if (dealersHand.handCards.Contains(tableCard))
            {
                Outline outline = tableCardDisplay.GetComponent<Outline>();
                outline.enabled = true;
                outline.effectColor = new Color(1f, 1f, 0f, 1f);
            }
        }

        if (dealersHand.handCards.Contains(dealerCard1.GetCard()))
        {
            dealerCard1.gameObject.GetComponentInParent<Outline>().enabled = true;
        }
        if (dealersHand.handCards.Contains(dealerCard2.GetCard()))
        {
            dealerCard2.gameObject.GetComponentInParent<Outline>().enabled = true;
        }
    }

    void ResetHighlightedCards()
    {
        foreach (var tableCardDisplay in tableCardDisplays)
        {
            tableCardDisplay.GetComponent<Outline>().enabled = false;
        }
        handCard1.gameObject.GetComponentInParent<Outline>().enabled = false;
        handCard2.gameObject.GetComponentInParent<Outline>().enabled = false;
        dealerCard1.gameObject.GetComponentInParent<Outline>().enabled = false;
        dealerCard2.gameObject.GetComponentInParent<Outline>().enabled = false;
    }

    public void Reset()
    {
        handCards.Clear();
        tableCards.Clear();
        InitializeDeck();

        showHandCanvas.SetActive(false);

        handCard1.gameObject.GetComponentInParent<Outline>().enabled = false;
        handCard2.gameObject.GetComponentInParent<Outline>().enabled = false;

        handCard1.transform.parent.gameObject.SetActive(false);
        handCard2.transform.parent.gameObject.SetActive(false);

        dealerCard1.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        dealerCard2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        dealerCard1.gameObject.GetComponentInParent<Outline>().enabled = false;
        dealerCard2.gameObject.GetComponentInParent<Outline>().enabled = false;

        dealerCard1.transform.parent.gameObject.SetActive(false);
        dealerCard2.transform.parent.gameObject.SetActive(false);

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
