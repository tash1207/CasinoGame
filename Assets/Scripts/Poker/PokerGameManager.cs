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
    [SerializeField] GameObject raiseButton;
    [SerializeField] GameObject playAgainButton;
    [SerializeField] GameObject anteButton;

    [Header("Chips")]
    [SerializeField] GameObject chipHolder;
    [SerializeField] GameObject chip1Prefab;
    [SerializeField] GameObject chip5Prefab;
    [SerializeField] TMP_Text currentPotText;

    public Round currentRound;
    public int antePrice = 1;
    public int currentPot = 0;

    private CardManager cardManager;
    private int currentBetValue = 0;
    private int totalBetValue = 0;

    void Awake()
    {
        cardManager = GetComponent<CardManager>();
        Initialize();
    }

    void SetCurrentPot(int value)
    {
        currentPot = value;
        currentPotText.text = "Current Pot: $" + currentPot;
    }

    public void Initialize()
    {
        currentRound = Round.PreDeal;
        currentBetValue = 0;
        totalBetValue = 0;
        SetCurrentPot(0);
        ResetChips();
        moveHistory.text = "New game";
        anteButton.SetActive(true);
    }
    void AddChipToPot(bool isPlayer, int chipAmount)
    {
        Vector3 chipPosition = Vector3.zero;
        chipPosition.x += UnityEngine.Random.Range(0, 200);
        chipPosition.y += UnityEngine.Random.Range(0, 100);
        if (chipAmount > 1 && UnityEngine.Random.Range(0f, 1f) > 0.4) chipPosition.z = -1;
        if (isPlayer) chipPosition.y *= -1;
        GameObject chip =
            Instantiate(chipAmount == 1 ? chip1Prefab : chip5Prefab, Vector3.zero, chip1Prefab.transform.rotation, chipHolder.transform);
        chip.transform.localPosition = chipPosition;
    }

    public void Ante()
    {
        SetCurrentPot(currentPot + antePrice);
        for (int i = 0; i < antePrice; i++)
        {
            AddChipToPot(true, 1);
        }
        moveHistory.text += "\nYou put $" + antePrice + " into the pot";
        cardManager.DealCards();
        moveHistory.text += "\nThe cards are dealt";
        currentRound = Round.PreFlop;
        anteButton.SetActive(false);
        ToggleAvailableActions(true);
    }

    public void Check(bool isPlayer)
    {
        if (isPlayer)
        {
            moveHistory.text += "\nYou check";
            if (DealerHasGreatHand())
            {
                Debug.Log("Dealer bets high for great hand");
                Bet(false, currentPot > 25 ? 10 : (currentPot > 10 ? 6 : 3));
            }
            else if (DealerHasGoodHand() || DealerShouldBet())
            {
                Debug.Log("Dealer should bet");
                if (UnityEngine.Random.Range(0f, 1f) > 0.2)
                {
                    Bet(false, currentPot > 20 ? 5 : (currentPot > 9 ? 4 : 2));
                }
                else
                {
                    Bet(false, currentPot > 10 ? 5 : 2);
                }
            }
            else if (UnityEngine.Random.Range(0f, 1f) > 0.85)
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
        SetCurrentPot(currentPot + currentBetValue);
        for (int i = 0; i < currentBetValue; i++)
        {
            AddChipToPot(isPlayer, 1);
        }
        if (isPlayer)
        {
            moveHistory.text += "\nYou call $" + currentBetValue;
        }
        else
        {
            moveHistory.text += "\nDealer calls $" + currentBetValue;
        }
        currentBetValue = 0;
        totalBetValue = 0;
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
        totalBetValue = betValue;
        currentBetValue = betValue;
        SetCurrentPot(currentPot + betValue);
        for (int i = 0; i < betValue; i++)
        {
            if (betValue - i >= 5)
            {
                AddChipToPot(isPlayer, 5);
                i += 5;
            }
            else
            {
                AddChipToPot(isPlayer, 1);
            }
        }
        if (isPlayer)
        {
            moveHistory.text += "\nYou bet $" + betValue;
            if (DealerHasGreatHand() && UnityEngine.Random.Range(0f, 1f) > 0.1)
            {
                Debug.Log("Dealer raises");
                // Dealer raise
                Raise(false, currentBetValue + 1);
                CanAdvance(false);
            }
            // Dealer can call or fold
            else if (DealerHasGoodHand() || DealerHasChance())
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

    public void Raise()
    {
        Raise(true, currentBetValue);
    }

    // betValue is amount over the current bet
    void Raise(bool isPlayer, int betValue)
    {
        SetCurrentPot(currentPot + currentBetValue + betValue);
        for (int i = 0; i < currentBetValue + betValue; i++)
        {
            if (currentBetValue + betValue - i >= 5)
            {
                AddChipToPot(isPlayer, 5);
                i += 4;
            }
            else
            {
                AddChipToPot(isPlayer, 1);
            }
        }
        if (isPlayer)
        {
            moveHistory.text += "\nYou raised to $" + (totalBetValue + betValue);
            currentBetValue = betValue;
            totalBetValue += currentBetValue;
            // Dealer move
            if (DealerHasGreatHand() || DealerHasGoodHand())
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
                if (currentBetValue <= 4 && DealerHasChance())
                {
                    Debug.Log("Dealer has chance and decided to call low raise");
                    // Dealer call
                    Call(false);
                    CanAdvance(true);
                }
                else if (currentBetValue > 4 && DealerHasChance() && UnityEngine.Random.Range(0f, 1f) > 0.4)
                {
                    Debug.Log("Dealer has chance and decided to call high raise");
                    // Dealer call
                    Call(false);
                    CanAdvance(true);
                }
                else
                {
                    Debug.Log("Dealer randomly decided to fold");
                    Fold(false);
                }
            }
        }
        else
        {
            moveHistory.text += "\nDealer raised to $" + (totalBetValue + betValue);
            currentBetValue = betValue;
            totalBetValue += currentBetValue;
        }
    }

    bool DealerHasGreatHand()
    {
        bool hasGreatHand = false;
        Hand dealersHand = HandManager.GetHand(cardManager.dealerCards, cardManager.tableCards);
        if (currentRound == Round.PreFlop)
        {
            if (dealersHand.score >= 2010)
            {
                Debug.Log("Has pair of 10s or better");
                hasGreatHand = true;
            }
            if (cardManager.dealerCards[0].suit == cardManager.dealerCards[1].suit &&
                cardManager.dealerCards[0].value > 10 && cardManager.dealerCards[1].value >= 10)
            {
                Debug.Log("Has suited face cards");
                hasGreatHand = true;
            }
            if (cardManager.dealerCards[0].value >= 13 && cardManager.dealerCards[1].value >= 13)
            {
                Debug.Log("Has A, K");
                hasGreatHand = true;
            }
        }
        else if (currentRound == Round.Flop)
        {
            List<Card> emptyList = new List<Card>();
            Hand tableHand = HandManager.GetHand(emptyList, cardManager.tableCards);
            if (dealersHand.score >= 4004 && dealersHand.score > tableHand.score)
            {
                Debug.Log("Hit 3 of a kind, 4s or higher");
                hasGreatHand = true;
            }
            if (dealersHand.score >= 3120 && tableHand.score < 2013)
            {
                Debug.Log("Has 2 pair with highest at least Qs");
                hasGreatHand = true;
            }
        }
        else if (currentRound == Round.Turn || currentRound == Round.River)
        {
            List<Card> emptyList = new List<Card>();
            Hand tableHand = HandManager.GetHand(emptyList, cardManager.tableCards);
            if (dealersHand.score >= 5000 && dealersHand.score > tableHand.score)
            {
                Debug.Log("Has straight or better");
                hasGreatHand = true;
            }
        }
        return hasGreatHand;
    }

    bool DealerHasGoodHand()
    {        
        Hand dealersHand = HandManager.GetHand(cardManager.dealerCards, cardManager.tableCards);
        if (currentRound == Round.PreFlop)
        {
            bool hasGoodHand = false;
            if (dealersHand.score >= 2005)
            {
                Debug.Log("Has pair of 5s or better");
                hasGoodHand = true;
            }
            if ((cardManager.dealerCards[0].value >= 12 && cardManager.dealerCards[1].value >= 9) || 
                (cardManager.dealerCards[1].value >= 12 && cardManager.dealerCards[0].value >= 9))
            {
                Debug.Log("Has A, K or Q in hand with other card at least 9");
                hasGoodHand = true;
            }
            if (cardManager.dealerCards[0].suit == cardManager.dealerCards[1].suit &&
                Math.Abs(cardManager.dealerCards[0].value - cardManager.dealerCards[1].value) == 1)
            {
                Debug.Log("Has suited connectors");
                hasGoodHand = true;
            }
            if (cardManager.dealerCards[0].suit == cardManager.dealerCards[1].suit &&
                cardManager.dealerCards[0].value >= 7 && cardManager.dealerCards[1].value >= 7)
            {
                Debug.Log("Has suited cards, at least 7");
                hasGoodHand = true;
            }
            if (cardManager.dealerCards[0].suit == cardManager.dealerCards[1].suit &&
                (cardManager.dealerCards[0].value == 14 || cardManager.dealerCards[1].value == 14))
            {
                Debug.Log("Has suited cards, with an Ace");
                hasGoodHand = true;
            }
            if (Math.Abs(cardManager.dealerCards[0].value - cardManager.dealerCards[1].value) == 1 &&
                (cardManager.dealerCards[0].value >= 8 || cardManager.dealerCards[1].value >= 8))
            {
                Debug.Log("Has 2 in a row");
                hasGoodHand = true;
            }
            return hasGoodHand;
        }
        else // Cards on the table
        {
            List<Card> emptyList = new List<Card>();
            Hand tableHand = HandManager.GetHand(emptyList, cardManager.tableCards);
            if (currentRound == Round.Flop)
            {
                return dealersHand.score > 2006 && dealersHand.score > tableHand.score; // Has pair of 7s or better
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
            if (dealersHand.score > 2005)
            {
                Debug.Log("Has better than pair of 5s");
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
            return dealersHand.score == tableHand.score && tableHand.score < 9000;
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
        raiseButton.SetActive(false);
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
                raiseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Raise to $" + (totalBetValue + currentBetValue);
                raiseButton.SetActive(true);
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
