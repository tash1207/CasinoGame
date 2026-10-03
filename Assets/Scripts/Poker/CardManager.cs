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

    [Header("Player Cards")]
    [SerializeField] MeshCardDisplay player1Card1;
    [SerializeField] MeshCardDisplay player1Card2;

    [Header("EndGame Panels")]
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] TMP_Text showHandText;
    [SerializeField] TMP_Text showDealerHandText;
    [SerializeField] TMP_Text showPlayer1HandText;
    [SerializeField] TMP_Text whoWonText;

    private List<Card> allCards;
    public List<Card> handCards { get; private set; }
    public List<Card> tableCards { get; private set; }
    public List<Card> dealerCards { get; private set; }
    public List<Card> player1Cards { get; private set; }

    private Hand yourHand;
    private Hand dealersHand;
    private Hand player1Hand;
    public int numPlayers { get; private set; }

    private List<GameObject> tableCardDisplays;

    void Awake()
    {
        numPlayers = player1Card1 == null ? 2 : 3;
    }

    void OnEnable()
    {
        InitializeDeck();
    }

    void InitializeDeck()
    {
        allCards = new List<Card>();
        handCards = new List<Card>(10);
        tableCards = new List<Card>(10);
        dealerCards = new List<Card>(10);
        player1Cards = new List<Card>(10);

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

        if (numPlayers == 3)
        {
            Card randomCard5 = GetRandomCard();
            Card randomCard6 = GetRandomCard();

            player1Card1.transform.parent.gameObject.SetActive(true);
            player1Card2.transform.parent.gameObject.SetActive(true);

            player1Cards.Add(randomCard5);
            player1Card1.SetCard(randomCard5);

            player1Cards.Add(randomCard6);
            player1Card2.SetCard(randomCard6);
        }
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

    public void ShowHand(int currentPot, bool dealerFolded, bool player1Folded)
    {
        yourHand = HandManager.GetHand(handCards, tableCards);
        dealersHand = HandManager.GetHand(dealerCards, tableCards);
        player1Hand = HandManager.GetHand(player1Cards, tableCards);

        if (numPlayers == 3)
        {
            player1Hand = HandManager.GetHand(player1Cards, tableCards);
            showPlayer1HandText.text = "Joe's Hand:\n" + player1Hand.handText;

            player1Card1.transform.rotation = Quaternion.Euler(180f, 0f, 90f);
            player1Card2.transform.rotation = Quaternion.Euler(180f, 0f, 90f);

            WinLogicWithPlayer1(currentPot, dealerFolded, player1Folded);
        }
        else
        {
            WinLogic(currentPot);
        }

        showHandText.text = "Your Hand:\n" + yourHand.handText;
        showDealerHandText.text = "Dealer's Hand:\n" + dealersHand.handText;
        gameOverCanvas.SetActive(true);

        dealerCard1.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        dealerCard2.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    void WinLogic(int currentPot)
    {
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
                Chop(currentPot, "Dealer");
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
                    Chop(currentPot, "Dealer");
                }
            }
        }
    }

    void WinLogicWithPlayer1(int currentPot, bool dealerFolded, bool player1Folded)
    {
        if (player1Folded)
        {
            WinLogic(currentPot);
            return;
        }
        else if (dealerFolded)
        {
            if (yourHand.score > player1Hand.score)
            {
                YouWin(currentPot);
            }
            else if (player1Hand.score > yourHand.score)
            {
                Player1Wins(currentPot);
            }
            else // yourHand.score == dealersHand.score
            {
                if (yourHand.kickers.Count == 0)
                {
                    Chop(currentPot, "Joe");
                }
                for (int i = 0; i < yourHand.kickers.Count; i++)
                {
                    if (yourHand.kickers[i].value > player1Hand.kickers[i].value)
                    {
                        YouWin(currentPot);
                        break;
                    }
                    else if (player1Hand.kickers[i].value > yourHand.kickers[i].value)
                    {
                        Player1Wins(currentPot);
                        break;
                    }
                    else if (i == yourHand.kickers.Count - 1)
                    {
                        Chop(currentPot, "Joe");
                    }
                }
            }
        }
        else
        {
            if (yourHand.score > dealersHand.score && yourHand.score > player1Hand.score)
            {
                YouWin(currentPot);
            }
            else if (player1Hand.score > yourHand.score && player1Hand.score > dealersHand.score)
            {
                Player1Wins(currentPot);
            }
            else if (dealersHand.score > yourHand.score && dealersHand.score > player1Hand.score)
            {
                DealerWins(currentPot);
            }
            else if (yourHand.score == dealersHand.score && yourHand.score == player1Hand.score)
            {
                // TODO: Fix 3 way chop logic
                if (yourHand.kickers.Count == 0)
                {
                    Chop3Ways(currentPot, "Dealer", "Joe");
                }
                for (int i = 0; i < yourHand.kickers.Count; i++)
                {
                    if (yourHand.kickers[i].value > dealersHand.kickers[i].value &&
                        yourHand.kickers[i].value > player1Hand.kickers[i].value)
                    {
                        YouWin(currentPot);
                        break;
                    }
                    else if (dealersHand.kickers[i].value > yourHand.kickers[i].value &&
                            dealersHand.kickers[i].value > player1Hand.kickers[i].value)
                    {
                        DealerWins(currentPot);
                        break;
                    }
                    else if (player1Hand.kickers[i].value > yourHand.kickers[i].value &&
                            player1Hand.kickers[i].value > dealersHand.kickers[i].value)
                    {
                        Player1Wins(currentPot);
                        break;
                    }
                    else if (i == yourHand.kickers.Count - 1)
                    {
                        Chop3Ways(currentPot, "Dealer", "Joe");
                    }
                }
            }
            else if (yourHand.score == dealersHand.score)
            {
                if (yourHand.kickers.Count == 0)
                {
                    Chop(currentPot, "Dealer");
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
                        Chop(currentPot, "Joe");
                    }
                }
            }
            else if (player1Hand.score == dealersHand.score)
            {
                if (player1Hand.kickers.Count == 0)
                {
                    Chop(currentPot, "Dealer");
                }
                for (int i = 0; i < player1Hand.kickers.Count; i++)
                {
                    if (player1Hand.kickers[i].value > dealersHand.kickers[i].value)
                    {
                        Player1Wins(currentPot);
                        break;
                    }
                    else if (dealersHand.kickers[i].value > player1Hand.kickers[i].value)
                    {
                        DealerWins(currentPot);
                        break;
                    }
                    else if (i == player1Hand.kickers.Count - 1)
                    {
                        // TODO: Fix Chop logic to add who is chopping pot.
                        Chop(currentPot, "Joe");
                    }
                }
            }
            else if (yourHand.score == player1Hand.score)
            {
                if (yourHand.kickers.Count == 0)
                {
                    Chop(currentPot, "Joe");
                }
                for (int i = 0; i < yourHand.kickers.Count; i++)
                {
                    if (yourHand.kickers[i].value > player1Hand.kickers[i].value)
                    {
                        YouWin(currentPot);
                        break;
                    }
                    else if (player1Hand.kickers[i].value > yourHand.kickers[i].value)
                    {
                        Player1Wins(currentPot);
                        break;
                    }
                    else if (i == yourHand.kickers.Count - 1)
                    {
                        Chop(currentPot, "Joe");
                    }
                }
            }
        }
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

    void Player1Wins(int currentPot)
    {
        HighlightPlayer1Cards();
        whoWonText.text = "JOE WINS $" + currentPot;
    }

    void Chop(int currentPot, string otherWinner)
    {
        HighlightYourCards();
        whoWonText.text = "CHOP $" + currentPot;
        whoWonText.text += "\n" + otherWinner + " gets $" + Mathf.CeilToInt(currentPot/2f);
        whoWonText.text += "\nYou get $" + Mathf.FloorToInt(currentPot/2f);
        MoneyManager.Instance.AddMoney(Mathf.FloorToInt(currentPot/2f));
    }

    void Chop3Ways(int currentPot, string otherWinner1, string otherWinner2)
    {
        HighlightYourCards();
        whoWonText.text = "CHOP $" + currentPot;
        whoWonText.text += "\n" + otherWinner1 + " gets $" + Mathf.CeilToInt(currentPot/3f);
        whoWonText.text += "\n" + otherWinner2 + " gets $" + Mathf.FloorToInt(currentPot/3f);
        whoWonText.text += "\nYou get $" + Mathf.FloorToInt(currentPot/3f);
        MoneyManager.Instance.AddMoney(Mathf.FloorToInt(currentPot/3f));
    }

    public void DealerFold(int currentPot)
    {
        whoWonText.text = "YOU WIN $" + currentPot;
        showHandText.text = "You win by default";
        showDealerHandText.text = "Dealer folded";
        MoneyManager.Instance.AddMoney(currentPot);
        gameOverCanvas.SetActive(true);

        if (numPlayers == 3)
        {
            showPlayer1HandText.text = "Joe folded";

            player1Card1.transform.rotation = Quaternion.Euler(180f, 0f, 90f);
            player1Card2.transform.rotation = Quaternion.Euler(180f, 0f, 90f);
        }

        dealerCard1.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        dealerCard2.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public void PlayerFold(int currentPot)
    {
        whoWonText.text = "DEALER WINS $" + currentPot;
        showHandText.text = "You folded";
        showDealerHandText.text = "Dealer wins by default";
        gameOverCanvas.SetActive(true);

        if (numPlayers == 3)
        {
            showPlayer1HandText.text = "Joe folded";

            player1Card1.transform.rotation = Quaternion.Euler(180f, 0f, 90f);
            player1Card2.transform.rotation = Quaternion.Euler(180f, 0f, 90f);
        }

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

    public void HighlightPlayer1Cards()
    {
        ResetHighlightedCards();
        foreach (var tableCardDisplay in tableCardDisplays)
        {
            Card tableCard = tableCardDisplay.GetComponentInChildren<MeshCardDisplay>().GetCard();
            if (player1Hand.handCards.Contains(tableCard))
            {
                Outline outline = tableCardDisplay.GetComponent<Outline>();
                outline.enabled = true;
                outline.effectColor = new Color(0f, 1f, 0f, 1f);
            }
        }

        if (player1Hand.handCards.Contains(player1Card1.GetCard()))
        {
            player1Card1.gameObject.GetComponentInParent<Outline>().enabled = true;
        }
        if (player1Hand.handCards.Contains(player1Card2.GetCard()))
        {
            player1Card2.gameObject.GetComponentInParent<Outline>().enabled = true;
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

        if (numPlayers == 3)
        {
            player1Card1.gameObject.GetComponentInParent<Outline>().enabled = false;
            player1Card2.gameObject.GetComponentInParent<Outline>().enabled = false;
        }
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

        if (numPlayers == 3)
        {
            player1Card1.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            player1Card2.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            player1Card1.transform.parent.gameObject.SetActive(false);
            player1Card2.transform.parent.gameObject.SetActive(false);
        }

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
