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
    [SerializeField] GameObject betButton1;
    [SerializeField] GameObject betButton2;
    [SerializeField] GameObject foldButton;
    [SerializeField] GameObject playAgainButton;
    [SerializeField] GameObject anteButton;

    [Header("Chips")]
    [SerializeField] GameObject chipHolder;
    [SerializeField] GameObject chipPrefab;

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
    void AddChipToPot(bool isPlayer)
    {
        Vector3 chipPosition = Vector3.zero;
        chipPosition.x += UnityEngine.Random.Range(0, 200);
        chipPosition.y += UnityEngine.Random.Range(0, 100);
        if (isPlayer) chipPosition.y *= -1;
        GameObject chip =
            Instantiate(chipPrefab, Vector3.zero, chipPrefab.transform.rotation, chipHolder.transform);
        chip.transform.localPosition = chipPosition;
    }

    public void Ante()
    {
        currentPot += antePrice;
        AddChipToPot(true);
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
            if (DealerHasGoodHand() || DealerShouldBet())
            {
                Debug.Log("Dealer should bet");
                if (UnityEngine.Random.Range(0f, 1f) > 0.2)
                {
                    Bet(false, 1);
                }
                else
                {
                    Bet(false, 2);
                }
            }
            else if (UnityEngine.Random.Range(0f, 1f) > 0.8)
            {
                Debug.Log("Dealer randomly decided to bet");
                if (UnityEngine.Random.Range(0f, 1f) > 0.1)
                {
                    Bet(false, 1);
                }
                else
                {
                    Bet(false, 2);
                }
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
        AddChipToPot(isPlayer);
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

    public void Bet2()
    {
        Bet(true, 2);
    }

    void Bet(bool isPlayer, int betValue)
    {
        currentBetValue = betValue;
        currentPot += betValue;
        for (int i = 0; i < betValue; i++)
        {
            AddChipToPot(isPlayer);
        }
        if (isPlayer)
        {
            moveHistory.text += "\nYou bet $" + betValue;
            // Dealer can call or fold
            if (DealerHasGoodHand() || DealerHasChance())
            {
                Debug.Log("Dealer should call");
                // Dealer call
                Call(false);
                CanAdvance(true);
            }
            else if (DealerShouldFold())
            {
                Debug.Log("Dealer should fold");
                Fold(false);
            }
            else
            {
                if (currentBetValue == 1 && UnityEngine.Random.Range(0f, 1f) > 0.3)
                {
                    Debug.Log("Dealer randomly decided to call");
                    // Dealer call
                    Call(false);
                    CanAdvance(true);
                }
                else if (currentBetValue > 1 && UnityEngine.Random.Range(0f, 1f) > 0.85)
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
        else // Cards on the table
        {
            List<Card> emptyList = new List<Card>();
            Hand tableHand = HandManager.GetHand(emptyList, cardManager.tableCards);
            if (currentRound == Round.Flop)
            {
                return dealersHand.score > 2005 && dealersHand.score > tableHand.score; // Has pair of 6s or better
            }
            else
            {
                return dealersHand.score > 2010 && dealersHand.score > tableHand.score; // Has pair of jacks or better
            }
        }
    }

    bool DealerHasChance()
    {
        if (currentRound == Round.PreFlop)
        {
            return DealerHasGoodHand();
        }

        List<Card> emptyList = new List<Card>();
        Hand tableHand = HandManager.GetHand(emptyList, cardManager.tableCards);
        Hand dealersHand = HandManager.GetHand(cardManager.dealerCards, cardManager.tableCards);
        if (currentRound == Round.Flop)
        {
            if (dealersHand.score > 2003)
            {
                Debug.Log("Has better than pair of 3s");
                return true;
            }
            if (cardManager.dealerCards[0].value >= 12 || cardManager.dealerCards[1].value >= 12)
            {
                Debug.Log("Has A, K or Q in hand");
                return true;
            }
            if (dealersHand.numSameSuit >= 4 ||
                (dealersHand.numSameSuit >= 3 && tableHand.numSameSuit < 3))
            {
                Debug.Log("Has at least 3 suited cards not from table");
                return true;
            }
            if (dealersHand.numInARow >= 4 ||
                (dealersHand.numInARow >= 3 && tableHand.numInARow < 3))
            {
                // TODO: Figure out straight gap logic like AK J10 (just needs Q)
                Debug.Log("Has at least 3 in a row not from table");
                return true;
            }
        }
        else if (currentRound == Round.Turn)
        {
            if (dealersHand.score > tableHand.score)
            {
                Debug.Log("Dealer has better hand than table");
                return true;
            }
            if (tableHand.numSameSuit >= 3 && dealersHand.score > tableHand.score)
            {
                Debug.Log("Dealer can compete with flush draw");
                return true;
            }
        }
        return false;
    }

    bool DealerShouldBet()
    {
        if (currentRound == Round.PreFlop)
        {
            return false;
        }
        else
        {
            List<Card> emptyList = new List<Card>();
            Hand tableHand = HandManager.GetHand(emptyList, cardManager.tableCards);
            Hand dealersHand = HandManager.GetHand(cardManager.dealerCards, cardManager.tableCards);
            if (currentRound == Round.Flop)
            {
                if (dealersHand.numSameSuit >= 4 && tableHand.numSameSuit < 4)
                {
                    Debug.Log("Has 4 suited cards not from table");
                    return true;
                }
                if (dealersHand.numInARow >= 4)
                {
                    // TODO: Figure out straight gap logic like AK J10 (just needs Q)
                    Debug.Log("Has 4 in a row not from table");
                    return true;
                }
            }
        }
        return false;
    }

    bool DealerShouldFold()
    {
        if (currentRound == Round.River)
        {
            List<Card> emptyList = new List<Card>();
            Hand tableHand = HandManager.GetHand(emptyList, cardManager.tableCards);
            Hand dealersHand = HandManager.GetHand(cardManager.dealerCards, cardManager.tableCards);
            return dealersHand.score == tableHand.score;
        }
        return false;
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
        checkButton.SetActive(false);
        callButton.SetActive(false);
        betButton1.SetActive(false);
        betButton2.SetActive(false);
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
                betButton1.SetActive(true);
                betButton2.SetActive(true);
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

        ResetChips();
    }

    public void ResetChips()
    {
        List<GameObject> chipsToDestroy = new List<GameObject>();
        foreach (Transform child in chipHolder.transform)
        {
            chipsToDestroy.Add(child.gameObject);
        }

        foreach (GameObject chip in chipsToDestroy)
        {
            Destroy(chip);
        }
    }
}
