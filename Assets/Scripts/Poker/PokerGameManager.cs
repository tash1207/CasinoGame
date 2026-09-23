using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PokerGameManager : MonoBehaviour
{
    public enum Round
    {
        PreDeal, // No cards
        PreFlop, // 2 cards dealt
        Flop, // 3 cards on table
        Turn, // 4 cards on table
        River, // 5 cards on table
        Show, // Show cards
        End, // After show cards or after someone folds
    }

    [SerializeField] TMP_Text moveHistory;
    [SerializeField] GameObject advanceButton;
    [SerializeField] GameObject checkButton;
    [SerializeField] GameObject callButton;
    [SerializeField] GameObject betButton;
    [SerializeField] GameObject foldButton;
    [SerializeField] GameObject playAgainButton;
    [SerializeField] GameObject anteButton;

    public Round currentRound;
    public int antePrice = 1;
    public int currentPot = 0;

    private CardManager cardManager;
    private int currentBetValue = 0;

    void Awake()
    {
        cardManager = GetComponent<CardManager>();
        Initialize();
    }

    public void Initialize()
    {
        currentRound = Round.PreDeal;
        currentBetValue = 0;
        currentPot = 0;
        moveHistory.text = "New game";
        anteButton.SetActive(true);
    }

    public void Ante()
    {
        currentPot += antePrice;
        moveHistory.text += "\nYou put $" + antePrice + " into the pot";
        cardManager.DealCards();
        currentRound = Round.PreFlop;
        anteButton.SetActive(false);
        ToggleAvailableActions(true);
    }

    public void Check(bool isPlayer)
    {
        if (isPlayer)
        {
            moveHistory.text += "\nYou check";
            if ((currentRound == Round.PreFlop || currentRound == Round.River) && DealerHasGoodHand())
            {
                Debug.Log("Dealer has decent hand");
                Bet(false, 1);
            }
            else if (UnityEngine.Random.Range(0f, 1f) > 0.8)
            {
                Debug.Log("Dealer randomly decided to bet");
                Bet(false, 1);
            }
            else
            {
                // Dealer checks as well
                Check(false);
            }
        }
        else
        {
            moveHistory.text += "\nDealer checks";
            CanAdvance(true);
        }
    }

    public void Call(bool isPlayer)
    {
        currentPot += currentBetValue;
        if (isPlayer)
        {
            moveHistory.text += "\nYou call $" + currentBetValue;
        }
        else
        {
            moveHistory.text += "\nDealer calls $" + currentBetValue;
        }
        currentBetValue = 0;
        CanAdvance(true);
    }

    public void Fold(bool isPlayer)
    {
        if (isPlayer)
        {
            moveHistory.text += "\nYou fold";
            cardManager.PlayerFold(currentPot);
        }
        else
        {
            moveHistory.text += "\nDealer folds";
            cardManager.DealerFold(currentPot);
        }
        EndGame();
    }

    public void Bet1()
    {
        Bet(true, 1);
    }

    void Bet(bool isPlayer, int betValue)
    {
        currentBetValue = betValue;
        currentPot += betValue;
        if (isPlayer)
        {
            moveHistory.text += "\nYou bet $" + betValue;
            // Dealer can call or fold
            if (DealerHasGoodHand())
            {
                Debug.Log("Dealer should call");
                // Dealer call
                Call(false);
                CanAdvance(true);
            }
            else
            {
                if (UnityEngine.Random.Range(0f, 1f) > 0.25)
                {
                    Debug.Log("Dealer randomly decided to call");
                    // Dealer call
                    Call(false);
                    CanAdvance(true);
                }
                else
                {
                    Debug.Log("Dealer randomly decided to fold");
                    // Fold 25% of the time.
                    Fold(false);
                }
            }
        }
        else
        {
            moveHistory.text += "\nDealer bet $" + betValue;
            CanAdvance(false);
        }
    }

    bool DealerHasGoodHand()
    {        
        Hand dealersHand = HandManager.GetHand(cardManager.dealerCards, cardManager.tableCards);
        if (currentRound == Round.PreFlop)
        {
            if (dealersHand.score > 2000)
            {
                Debug.Log("Has better than a high card");
            }
            if (cardManager.dealerCards[0].value >= 12 || cardManager.dealerCards[1].value >= 12)
            {
                Debug.Log("Has A, K or Q in hand");
            }
            if (cardManager.dealerCards[0].suit == cardManager.dealerCards[1].suit)
            {
                Debug.Log("Has suited cards");
            }
            if (Math.Abs(cardManager.dealerCards[0].value - cardManager.dealerCards[1].value) == 1)
            {
                Debug.Log("Has 2 in a row");
            }
            return dealersHand.score > 2000 || // Has better than a high card
            cardManager.dealerCards[0].value >= 12 ||
            cardManager.dealerCards[1].value >= 12 || // Has A, K or Q in hand
            cardManager.dealerCards[0].suit == cardManager.dealerCards[1].suit || // Has suited cards
            Math.Abs(cardManager.dealerCards[0].value - cardManager.dealerCards[1].value) == 1; // Has 2 in a row
        }
        else
        {
            List<Card> emptyList = new List<Card>();
            Hand tableHand = HandManager.GetHand(emptyList, cardManager.tableCards);
            return dealersHand.score > 2011 && dealersHand.score > tableHand.score; // Has pair of queens or better
        }
    }

    void CanAdvance(bool value)
    {
        if (currentRound == Round.PreFlop)
        {
            advanceButton.GetComponentInChildren<TextMeshProUGUI>().text = "Flop";
        }
        else if (currentRound == Round.Flop)
        {
            advanceButton.GetComponentInChildren<TextMeshProUGUI>().text = "Turn";
        }
        else if (currentRound == Round.Turn)
        {
            advanceButton.GetComponentInChildren<TextMeshProUGUI>().text = "River";
        }
        else if (currentRound == Round.River)
        {
            advanceButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show Cards";
        }
        else
        {
            advanceButton.GetComponentInChildren<TextMeshProUGUI>().text = "Advance";
        }
        advanceButton.SetActive(value);
        ToggleAvailableActions(!value);
    }

    public void AdvanceRound()
    {
        currentBetValue = 0;
        moveHistory.text += "\nCurrent pot: $" + currentPot;
        switch (currentRound)
        {
            case Round.PreDeal:
                // TODO: Before the game begins, player must pay the ante and dealer must
                // call to move to preflop or
                // fold to move to end
            case Round.PreFlop:
                moveHistory.text += "\nThe flop is shown";
                cardManager.Flop();
                currentRound = Round.Flop;
                CanAdvance(false);
                break;
            case Round.Flop:
                moveHistory.text += "\nThe turn is shown";
                cardManager.Turn();
                currentRound = Round.Turn;
                CanAdvance(false);
                break;
            case Round.Turn:
                moveHistory.text += "\nThe river is shown";
                cardManager.River();
                currentRound = Round.River;
                CanAdvance(false);
                break;
            case Round.River:
                moveHistory.text += "\nShow cards";
                cardManager.ShowHand(currentPot);
                currentRound = Round.Show;
                CanAdvance(false);
                break;
            default:
                return;
        }
    }

    void ToggleAvailableActions(bool enabled)
    {
        Debug.Log("ToggleActions " + enabled);
        checkButton.SetActive(false);
        callButton.SetActive(false);
        betButton.SetActive(false);
        foldButton.SetActive(false);
        playAgainButton.SetActive(false);

        if (!enabled)
        {
            return;
        }

        if (currentRound == Round.Show)
        {
            playAgainButton.SetActive(true);
        }
        else
        {
            if (currentBetValue == 0)
            {
                betButton.SetActive(true);
                checkButton.SetActive(true);
            }
            else
            {
                callButton.GetComponentInChildren<TextMeshProUGUI>().text = "Call $" + currentBetValue;
                callButton.SetActive(true);
                foldButton.SetActive(true);
            }
        }
    }

    void EndGame()
    {
        currentRound = Round.End;
        ToggleAvailableActions(false);
        playAgainButton.SetActive(true);
    }
}
