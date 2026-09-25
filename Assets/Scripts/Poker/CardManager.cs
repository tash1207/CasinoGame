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

    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] TMP_Text showHandText;
    [SerializeField] TMP_Text showDealerHandText;
    [SerializeField] TMP_Text whoWonText;

    private List<Card> allCards;
    public List<Card> handCards { get; private set; }
    public List<Card> tableCards { get; private set; }
    public List<Card> dealerCards { get; private set; }

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
        int index = Random.Range(0, allCards.Count);
        Card card = allCards[index];
        Debug.Log("Dealing a " + card.valueName + " of " + card.GetSuitName());
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

    public void ShowHand(int currentPot)
    {
        yourHand = HandManager.GetHand(handCards, tableCards);
        dealersHand = HandManager.GetHand(dealerCards, tableCards);

        if (yourHand.score > dealersHand.score)
        {
            YouWin(currentPot);
        }
        else if (dealersHand.score > yourHand.score)
        {
            DealerWins(currentPot);
        }
        else // yourHand.score == dealersHand.score
        {
            if (yourHand.kickers.Count == 0)
            {
                Chop(currentPot);
            }
            for (int i = 0; i < yourHand.kickers.Count; i++)
            {
                if (yourHand.kickers[i].value > dealersHand.kickers[i].value)
                {
                    YouWin(currentPot);
                    break;
                }
                else if (dealersHand.kickers[i].value > yourHand.kickers[i].value)
                {
                    DealerWins(currentPot);
                    break;
                }
                else if (i == yourHand.kickers.Count - 1)
                {
                    Chop(currentPot);
                }
            }
        }

        showHandText.text = "Your Hand:\n" + yourHand.handText;
        showDealerHandText.text = "Dealer's Hand:\n" + dealersHand.handText;
        gameOverCanvas.SetActive(true);

        dealerCard1.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        dealerCard2.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    void YouWin(int currentPot)
    {
        HighlightYourCards();
        whoWonText.text = "YOU WIN $" + currentPot;
        MoneyManager.Instance.AddMoney(currentPot);
    }

    void DealerWins(int currentPot)
    {
        HighlightDealersCards();
        whoWonText.text = "DEALER WINS $" + currentPot;
    }

    void Chop(int currentPot)
    {
        HighlightYourCards();
        whoWonText.text = "CHOP $" + currentPot;
        whoWonText.text += "\nDealer gets $" + (currentPot + 1)/2;
        whoWonText.text += "\nYou get $" + (currentPot - 1)/2;
        MoneyManager.Instance.AddMoney((currentPot - 1)/2);
    }

    public void DealerFold(int currentPot)
    {
        whoWonText.text = "YOU WIN $" + currentPot;
        showHandText.text = "You win by default";
        showDealerHandText.text = "Dealer folded";
        MoneyManager.Instance.AddMoney(currentPot);
        gameOverCanvas.SetActive(true);

        dealerCard1.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        dealerCard2.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public void PlayerFold(int currentPot)
    {
        whoWonText.text = "DEALER WINS $" + currentPot;
        showHandText.text = "You folded";
        showDealerHandText.text = "Dealer wins by default";
        gameOverCanvas.SetActive(true);

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

        gameOverCanvas.SetActive(false);

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
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }

}
