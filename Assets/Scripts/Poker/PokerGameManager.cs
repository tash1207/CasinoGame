using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] GameObject notEnoughMoneyText;

    [Header("Action Buttons")]
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
    [SerializeField] GameObject chip10Prefab;
    [SerializeField] TMP_Text currentPotText;

    [Header("Game Info")]
    [SerializeField] GameObject dealerStatusBanner;
    [SerializeField] GameObject player1StatusBanner;
    public Round currentRound;
    public int antePrice = 1;
    public int currentPot = 0;

    private Hand dealerHand;
    private Hand player1Hand;
    private Hand tableHand;

    private CardManager cardManager;
    private int currentBetValue = 0;
    private int totalBetValue = 0;
    private int playerSetBet = 0; // 1 for Player1, 2 for you, 3 for dealer

    private int numPlayers = 0;
    private bool dealerFolded;
    private bool player1Folded;
    private float statusTimeWait = 1.5f;

    void Awake()
    {
        cardManager = GetComponent<CardManager>();
    }

    void OnEnable()
    {
        dealerHand = new Hand();
        player1Hand = new Hand();
        tableHand = new Hand();
        numPlayers = cardManager.numPlayers;
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
        playerSetBet = 0;
        SetCurrentPot(0);
        ResetChips();

        if (dealerStatusBanner != null) dealerStatusBanner.SetActive(false);
        if (player1StatusBanner != null) player1StatusBanner?.SetActive(false);
        dealerFolded = false;
        player1Folded = numPlayers == 3 ? false : true;
        dealerHand.Clear();
        player1Hand.Clear();
        tableHand.Clear();
        moveHistory.text = "New game";
        if (MoneyManager.Instance.currentMoney < antePrice)
        {
            notEnoughMoneyText.SetActive(true);
            anteButton.SetActive(false);
        }
        else
        {
            notEnoughMoneyText.SetActive(false);
            anteButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ante $" + antePrice;
            anteButton.SetActive(true);
        }
    }

    void AddChipsToPot(bool isPlayer, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (amount - i >= 10)
            {
                AddChipToPot(isPlayer, 10);
                i += 9;
            }
            else if (amount - i >= 5)
            {
                AddChipToPot(isPlayer, 5);
                i += 4;
            }
            else
            {
                AddChipToPot(isPlayer, 1);
            }
        }
    }

    void AddChipToPot(bool isPlayer, int chipAmount)
    {
        Vector3 chipPosition = Vector3.zero;
        chipPosition.x += UnityEngine.Random.Range(0, 200);
        chipPosition.y += UnityEngine.Random.Range(0, 100);
        // Usually show the higher value chips on top of the stack
        if (chipAmount > 1 && UnityEngine.Random.Range(0f, 1f) > 0.3) chipPosition.z = -1;
        if (isPlayer) chipPosition.y *= -1;
        GameObject chip =
            Instantiate(chipAmount == 1 ? chip1Prefab : (chipAmount == 5 ? chip5Prefab : chip10Prefab), Vector3.zero, chip1Prefab.transform.rotation, chipHolder.transform);
        chip.transform.localPosition = chipPosition;
    }

    public void Ante()
    {
        // If there's another player have them ante first
        if (numPlayers == 3)
        {
            SetCurrentPot(currentPot + antePrice);
            AddChipsToPot(false, antePrice);
            moveHistory.text += "\nJoe put $" + antePrice + " into the pot";
        }
        SetCurrentPot(currentPot + antePrice);
        MoneyManager.Instance.LoseMoney(antePrice);
        AddChipsToPot(true, antePrice);
        moveHistory.text += "\nYou put $" + antePrice + " into the pot";
        cardManager.DealCards();
        moveHistory.text += "\nThe cards are dealt";
        dealerHand = HandManager.GetHand(cardManager.dealerCards, cardManager.tableCards);
        if (numPlayers == 3)
        {
            player1Hand = HandManager.GetHand(cardManager.player1Cards, cardManager.tableCards);
        }
        
        currentRound = Round.PreFlop;
        anteButton.SetActive(false);
        
        // Player 1 acts first
        StartCoroutine(Player1MoveFirst());
        ToggleAvailableActions(true);
    }

    IEnumerator WaitForDealerMove()
    {
        DisableAllActionButtons();
        dealerStatusBanner.GetComponentInChildren<TextMeshProUGUI>().text = "Dealer's Move...";
        dealerStatusBanner.SetActive(true);
        yield return new WaitForSeconds(statusTimeWait);

        EnableAllActionButtons();
        dealerStatusBanner.SetActive(false);
    }

    IEnumerator WaitForPlayer1Move()
    {
        DisableAllActionButtons();
        player1StatusBanner.GetComponentInChildren<TextMeshProUGUI>().text = "Joe's Move...";
        player1StatusBanner.SetActive(true);
        yield return new WaitForSeconds(statusTimeWait);

        EnableAllActionButtons();
        player1StatusBanner.SetActive(false);
    }

    void DisableAllActionButtons()
    {
        checkButton.GetComponent<Button>().interactable = false;
        callButton.GetComponent<Button>().interactable = false;
        betButton1.GetComponent<Button>().interactable = false;
        betButton2.GetComponent<Button>().interactable = false;
        foldButton.GetComponent<Button>().interactable = false;
        raiseButton.GetComponent<Button>().interactable = false;
    }

    void EnableAllActionButtons()
    {
        checkButton.GetComponent<Button>().interactable = true;
        callButton.GetComponent<Button>().interactable = true;
        betButton1.GetComponent<Button>().interactable = true;
        betButton2.GetComponent<Button>().interactable = true;
        foldButton.GetComponent<Button>().interactable = true;
        raiseButton.GetComponent<Button>().interactable = true;
    }

    IEnumerator Player1MoveFirst()
    {
        if (numPlayers != 3 || player1Folded)
        {
            yield break;
        }

        yield return StartCoroutine(WaitForPlayer1Move());

        if (Player1HasGreatHand())
        {
            Debug.Log("Player1 bets high for great hand");
            BetNPC(false, getBetButton2Amount());
        }
        else if (Player1HasGoodHand() || Player1ShouldBet())
        {
            Debug.Log("Player1 should bet");
            BetNPC(false, getBetButton1Amount());
        }
        else if (UnityEngine.Random.Range(0f, 1f) > 0.9)
        {
            Debug.Log("Player 1 randomly decided to bet");
            if (UnityEngine.Random.Range(0f, 1f) > 0.9)
            {
                BetNPC(false, getBetButton2Amount());
            }
            else
            {
                BetNPC(false, getBetButton1Amount());
            }
        }
        else
        {
            // Player 1 checks as well
            CheckNPC(false);
        }

        CanAdvance(false);
    }

    void CheckNPC(bool isDealer)
    {
        if (isDealer)
        {
            StartCoroutine(ShowDealerStatus("Check"));
            moveHistory.text += "\nDealer checks";
            CanAdvance(true);
        }
        else
        {
            StartCoroutine(ShowPlayer1Status("Check"));
            moveHistory.text += "\nJoe checks";
        }
    }

    public void Check()
    {
        moveHistory.text += "\nYou check";
        StartCoroutine(DealerMoveAfterCheck());
    }

    IEnumerator DealerMoveAfterCheck()
    {
        if (dealerFolded) {
            CanAdvance(true);
            yield break;
        }

        yield return StartCoroutine(WaitForDealerMove());

        if (DealerHasGreatHand())
        {
            Debug.Log("Dealer bets high for great hand");
            BetNPC(true, getBetButton2Amount());
        }
        else if (DealerHasGoodHand() || DealerShouldBet())
        {
            Debug.Log("Dealer should bet");
            if (UnityEngine.Random.Range(0f, 1f) > 0.2)
            {
                BetNPC(true, getBetButton1Amount());
            }
            else
            {
                BetNPC(true, currentPot > 10 ? 5 : 2);
            }
        }
        else if (UnityEngine.Random.Range(0f, 1f) > 0.85)
        {
            Debug.Log("Dealer randomly decided to bet");
            if (UnityEngine.Random.Range(0f, 1f) > 0.1)
            {
                BetNPC(true, getBetButton1Amount());
            }
            else
            {
                BetNPC(true, getBetButton2Amount());
            }
        }
        else
        {
            // Dealer checks as well
            CheckNPC(true);
        }
    }

    void CallNPC(bool isDealer)
    {
        SetCurrentPot(currentPot + currentBetValue);
        AddChipsToPot(false, currentBetValue);

        if (isDealer)
        {
            StartCoroutine(ShowDealerStatus("Call"));
            moveHistory.text += "\nDealer calls $" + currentBetValue;
        }
        else
        {
            StartCoroutine(ShowPlayer1Status("Call"));
            moveHistory.text += "\nJoe calls $" + currentBetValue;
        }
    }

    IEnumerator ShowDealerStatus(string status)
    {
        dealerStatusBanner.GetComponentInChildren<TextMeshProUGUI>().text = status;
        dealerStatusBanner.SetActive(true);
        yield return new WaitForSeconds(statusTimeWait);
        dealerStatusBanner.SetActive(false);
    }

    IEnumerator ShowPlayer1Status(string status)
    {
        player1StatusBanner.GetComponentInChildren<TextMeshProUGUI>().text = status;
        player1StatusBanner.SetActive(true);
        yield return new WaitForSeconds(statusTimeWait);
        player1StatusBanner.SetActive(false);
    }

    public void Call()
    {
        SetCurrentPot(currentPot + currentBetValue);
        AddChipsToPot(true, currentBetValue);

        moveHistory.text += "\nYou call $" + currentBetValue;
        MoneyManager.Instance.LoseMoney(currentBetValue);

        if (playerSetBet == 3 || // Dealer set bet so you are last to act
            playerSetBet == 1 && dealerFolded) // Player1 set bet and dealer folded, you are last to act
        {
            LastToActAfterBet();
        }
        else // Player 1 set bet so dealer must act
        {
            StartCoroutine(DealerMoveAfterBet());
        }
    }

    void LastToActAfterBet()
    {
        currentBetValue = 0;
        totalBetValue = 0;
        CanAdvance(true);
    }

    public void Fold()
    {
        moveHistory.text += "\nYou fold";
        if (dealerFolded) cardManager.PlayerFold(currentPot, "Joe");
        else cardManager.PlayerFold(currentPot);
        // TODO: Play out rest of game with Dealer and Player1
        EndGame();
    }

    void FoldNPC(bool isDealer)
    {
        if (isDealer)
        {
            moveHistory.text += "\nDealer folds";
            dealerFolded = true;
            dealerStatusBanner.GetComponentInChildren<TextMeshProUGUI>().text = "Folded";
            dealerStatusBanner.SetActive(true);
        }
        else
        {
            moveHistory.text += "\nJoe folds";
            player1Folded = true;
            player1StatusBanner.GetComponentInChildren<TextMeshProUGUI>().text = "Folded";
            player1StatusBanner.SetActive(true);
        }

        if (dealerFolded && player1Folded)
        {
            cardManager.DealerFold(currentPot);
            EndGame();
        }
    }

    public void Bet1()
    {
        Bet(getBetButton1Amount());
    }

    public void Bet2()
    {
        Bet(getBetButton2Amount());
    }

    void BetNPC(bool isDealer, int betValue)
    {
        totalBetValue = betValue;
        currentBetValue = betValue;
        SetCurrentPot(currentPot + betValue);
        AddChipsToPot(false, betValue);

        if (isDealer)
        {
            StartCoroutine(ShowDealerStatus("Bet $" + betValue));
            playerSetBet = 3;
            moveHistory.text += "\nDealer bet $" + betValue;
            StartCoroutine(Player1MoveAfterBet());
        }
        else
        {
            StartCoroutine(ShowPlayer1Status("Bet $" + betValue));
            playerSetBet = 1;
            moveHistory.text += "\nJoe bet $" + betValue;
        }
        CanAdvance(false);
    }

    void Bet(int betValue)
    {
        totalBetValue = betValue;
        currentBetValue = betValue;
        SetCurrentPot(currentPot + betValue);
        AddChipsToPot(true, betValue);

        playerSetBet = 2;
        moveHistory.text += "\nYou bet $" + betValue;
        MoneyManager.Instance.LoseMoney(currentBetValue);
        StartCoroutine(DealerMoveAfterBet());
    }

    IEnumerator DealerMoveAfterBet()
    {
        if (dealerFolded) {
            StartCoroutine(Player1MoveAfterBet());
            yield break;
        }

        yield return StartCoroutine(WaitForDealerMove());

        if (DealerHasGreatHand() && UnityEngine.Random.Range(0f, 1f) > 0.1)
        {
            Debug.Log("Dealer raises");
            // Dealer raise
            if (currentBetValue <= 2)
            {
                Raise(false, currentBetValue + 1);
            }
            else
            {
                Raise(false, currentBetValue);
            }
            if (!player1Folded) StartCoroutine(Player1MoveAfterBet());
            else CanAdvance(false);
            yield break;
            
        }
        else if (DealerHasGoodHand() || DealerHasChance())
        {
            Debug.Log("Dealer should call");
            // Dealer call
            CallNPC(true);
        }
        else if (DealerShouldFold())
        {
            Debug.Log("Dealer should fold");
            FoldNPC(true);
        }
        else
        {
            if (currentBetValue <= getBetButton1Amount(currentPot - currentBetValue) && UnityEngine.Random.Range(0f, 1f) > 0.5)
            {
                Debug.Log("Dealer randomly decided to call low bet");
                // Dealer call
                CallNPC(true);
            }
            else if (currentBetValue > getBetButton1Amount(currentPot - currentBetValue) && UnityEngine.Random.Range(0f, 1f) > 0.85)
            {
                Debug.Log("Dealer randomly decided to call high bet");
                // Dealer call
                CallNPC(true);
            }
            else
            {
                Debug.Log("Dealer randomly decided to fold");
                FoldNPC(true);
            }
        }

        if (dealerFolded && player1Folded) yield break;

        if (playerSetBet == 1 || // Player1 set bet so dealer is last to act
            playerSetBet == 2 && player1Folded) // You set bet and Player1 folded, dealer is last to act
        {
            LastToActAfterBet();
        }
        else
        {
            StartCoroutine(Player1MoveAfterBet());
        }
    }

    IEnumerator Player1MoveAfterBet()
    {
        if (numPlayers != 3 || player1Folded)
        {
            yield break;
        }

        yield return StartCoroutine(WaitForPlayer1Move());

        if (Player1HasGreatHand() || Player1HasGoodHand() || Player1HasChance())
        {
            Debug.Log("Player1 should call");
            // Player1 call
            CallNPC(false);
        }
        else if (Player1ShouldFold())
        {
            Debug.Log("Player1 should fold");
            FoldNPC(false);
        }
        else
        {
            if (currentBetValue <= getBetButton1Amount(currentPot - currentBetValue) && UnityEngine.Random.Range(0f, 1f) > 0.45)
            {
                Debug.Log("Player1 randomly decided to call low bet");
                // Player1 call
                CallNPC(false);
            }
            else if (currentBetValue > getBetButton1Amount(currentPot - currentBetValue) && UnityEngine.Random.Range(0f, 1f) > 0.75)
            {
                Debug.Log("Player1 randomly decided to call high bet");
                // Player1 call
                CallNPC(false);
            }
            else
            {
                Debug.Log("Player1 randomly decided to fold");
                FoldNPC(false);
            }
        }

        if (dealerFolded && player1Folded) yield break;

        if (playerSetBet == 2) // If you set bet, Player 1 is last to call.
        {
            LastToActAfterBet();
        }
        else
        {
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
        AddChipsToPot(isPlayer, currentBetValue + betValue);
        if (isPlayer)
        {
            playerSetBet = 2;
            moveHistory.text += "\nYou raised to $" + (totalBetValue + betValue);
            MoneyManager.Instance.LoseMoney(currentBetValue + betValue);
            currentBetValue = betValue;
            totalBetValue += currentBetValue;
            StartCoroutine(DealerMoveAfterRaise());
        }
        else
        {
            StartCoroutine(ShowDealerStatus("Raise to $" + (totalBetValue + betValue)));
            playerSetBet = 3;
            moveHistory.text += "\nDealer raised to $" + (totalBetValue + betValue);
            currentBetValue = betValue;
            totalBetValue += currentBetValue;
        }
    }

    IEnumerator DealerMoveAfterRaise()
    {
        if (dealerFolded) {
            StartCoroutine(Player1MoveAfterBet());
            yield break;
        }

        yield return StartCoroutine(WaitForDealerMove());

        if (DealerHasGreatHand() || DealerHasGoodHand())
        {
            Debug.Log("Dealer should call");
            // Dealer call
            CallNPC(true);
        }
        else if (DealerShouldFold())
        {
            Debug.Log("Dealer should fold");
            FoldNPC(true);
        }
        else
        {
            if (currentBetValue <= 4 && DealerHasChance())
            {
                Debug.Log("Dealer has chance and decided to call low raise");
                // Dealer call
                CallNPC(true);
            }
            else if (currentBetValue > 4 && DealerHasChance() && UnityEngine.Random.Range(0f, 1f) > 0.4)
            {
                Debug.Log("Dealer has chance and decided to call high raise");
                // Dealer call
                CallNPC(true);
            }
            else
            {
                Debug.Log("Dealer randomly decided to fold");
                FoldNPC(true);
            }
        }

        if (dealerFolded && player1Folded) yield break;

        if (!player1Folded) StartCoroutine(Player1MoveAfterBet());
        else CanAdvance(true);
    }

    bool DealerHasGreatHand()
    {
        return NPCHasGreatHand(dealerHand, cardManager.dealerCards[0], cardManager.dealerCards[1]);
    }

    bool Player1HasGreatHand()
    {
        return NPCHasGreatHand(player1Hand, cardManager.player1Cards[0], cardManager.player1Cards[1]);
    }

    bool NPCHasGreatHand(Hand npcHand, Card card0, Card card1)
    {
        bool hasGreatHand = false;
        if (currentRound == Round.PreFlop)
        {
            if (npcHand.score >= 2010)
            {
                Debug.Log("Has pair of 10s or better");
                hasGreatHand = true;
            }
            if (card0.suit == card1.suit &&
                card0.value > 10 && card1.value >= 10)
            {
                Debug.Log("Has suited face cards");
                hasGreatHand = true;
            }
            if (card0.value >= 13 && card1.value >= 13)
            {
                Debug.Log("Has A, K");
                hasGreatHand = true;
            }
        }
        else if (currentRound == Round.Flop)
        {
            if (npcHand.score >= 4004 && npcHand.score > tableHand.score)
            {
                Debug.Log("Hit 3 of a kind, 4s or higher");
                hasGreatHand = true;
            }
            if (npcHand.score >= 3120 && tableHand.score < 2013)
            {
                Debug.Log("Has 2 pair with highest at least Qs");
                hasGreatHand = true;
            }
        }
        else if (currentRound == Round.Turn || currentRound == Round.River)
        {
            if (npcHand.score >= 5000 && npcHand.score > tableHand.score)
            {
                Debug.Log("Has straight or better");
                hasGreatHand = true;
            }
        }
        return hasGreatHand;
    }

    bool DealerHasGoodHand()
    {
        return NPCHasGoodHand(dealerHand, cardManager.dealerCards[0], cardManager.dealerCards[1]);
    }

    bool Player1HasGoodHand()
    {
        return NPCHasGoodHand(player1Hand, cardManager.player1Cards[0], cardManager.player1Cards[1]);
    }

    bool NPCHasGoodHand(Hand npcHand, Card card0, Card card1)
    {        
        if (currentRound == Round.PreFlop)
        {
            bool hasGoodHand = false;
            if (npcHand.score >= 2005)
            {
                Debug.Log("Has pair of 5s or better");
                hasGoodHand = true;
            }
            if ((card0.value >= 12 && card1.value >= 9) || 
                (card1.value >= 12 && card0.value >= 9))
            {
                Debug.Log("Has A, K or Q in hand with other card at least 9");
                hasGoodHand = true;
            }
            if (card0.suit == card1.suit &&
                Math.Abs(card0.value - card1.value) == 1)
            {
                Debug.Log("Has suited connectors");
                hasGoodHand = true;
            }
            if (card0.suit == card1.suit &&
                card0.value >= 7 && card1.value >= 7)
            {
                Debug.Log("Has suited cards, at least 7");
                hasGoodHand = true;
            }
            if (card0.suit == card1.suit &&
                (card0.value == 14 || card1.value == 14))
            {
                Debug.Log("Has suited cards, with an Ace");
                hasGoodHand = true;
            }
            if (Math.Abs(card0.value - card1.value) == 1 &&
                (card0.value >= 8 || card1.value >= 8))
            {
                Debug.Log("Has 2 in a row");
                hasGoodHand = true;
            }
            return hasGoodHand;
        }
        else // Cards on the table
        {
            if (currentRound == Round.Flop)
            {
                return npcHand.score > 2006 && npcHand.score > tableHand.score; // Has pair of 7s or better
            }
            else
            {
                return npcHand.score > 2010 && npcHand.score > tableHand.score; // Has pair of jacks or better
            }
        }
    }

    bool DealerHasChance()
    {
        return NPCHasChance(dealerHand, cardManager.dealerCards[0], cardManager.dealerCards[1]);
    }

    bool Player1HasChance()
    {
        return NPCHasChance(player1Hand, cardManager.player1Cards[0], cardManager.player1Cards[1]);
    }

    bool NPCHasChance(Hand npcHand, Card card0, Card card1)
    {
        if (currentRound == Round.PreFlop)
        {
            return NPCHasGoodHand(npcHand, card0, card1);
        }
        if (currentRound == Round.Flop)
        {
            if (npcHand.score > 2005)
            {
                Debug.Log("Has better than pair of 5s");
                return true;
            }
            if (card0.value >= 12 || card1.value >= 12)
            {
                Debug.Log("Has A, K or Q in hand");
                return true;
            }
            if (npcHand.numSameSuit >= 4 ||
                (npcHand.numSameSuit >= 3 && tableHand.numSameSuit < 3))
            {
                Debug.Log("Has at least 3 suited cards not from table");
                return true;
            }
            if (npcHand.numInARow >= 4 ||
                (npcHand.numInARow >= 3 && tableHand.numInARow < 3))
            {
                // TODO: Figure out straight gap logic like AK J10 (just needs Q)
                Debug.Log("Has at least 3 in a row not from table");
                return true;
            }
        }
        else if (currentRound == Round.Turn)
        {
            if (tableHand.numSameSuit >= 3 &&
                (card0.suit == tableHand.suit || card1.suit == tableHand.suit) &&
                npcHand.score > 2000 &&
                npcHand.score > tableHand.score)
            {
                Debug.Log("NPC can compete with flush draw");
                return true;
            }
            if (tableHand.numSameSuit < 3 && npcHand.score > tableHand.score)
            {
                // TODO: Check for wet board and straight draws
                Debug.Log("NPC has better hand than table");
                return true;
            }
        }
        return false;
    }

    bool DealerShouldBet()
    {
        return NPCShouldBet(dealerHand, cardManager.dealerCards[0], cardManager.dealerCards[1]);
    }

    bool Player1ShouldBet()
    {
        return NPCShouldBet(player1Hand, cardManager.player1Cards[0], cardManager.player1Cards[1]);
    }

    bool NPCShouldBet(Hand npcHand, Card card0, Card card1)
    {
        if (currentRound == Round.PreFlop)
        {
            return false;
        }
        else
        {
            if (currentRound == Round.Flop)
            {
                if (npcHand.numSameSuit >= 4 && tableHand.numSameSuit < 4)
                {
                    Debug.Log("Has 4 suited cards not from table");
                    return true;
                }
                if (npcHand.numInARow >= 4)
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
        return NPCShouldFold(dealerHand, cardManager.dealerCards[0], cardManager.dealerCards[1]);
    }

    bool Player1ShouldFold()
    {
        return NPCShouldFold(player1Hand, cardManager.player1Cards[0], cardManager.player1Cards[1]);
    }

    bool NPCShouldFold(Hand npcHand, Card card0, Card card1)
    {
        if (currentRound == Round.PreFlop)
        {
            if (card0.suit != card1.suit &&
                card0.value < 10 && card1.value < 10 &&
                Math.Abs(card0.value - card1.value) > 1)
            {
                Debug.Log("Has unsuited unconnected low cards");
                return true;
            }
        }
        if (currentRound == Round.River)
        {
            return npcHand.score == tableHand.score && tableHand.score < 9000;
            // TODO: Update when pot is small, bet is low, and we have a high card on a dry board
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
        playerSetBet = 0;
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
                GetHands();
                StartCoroutine(Player1MoveFirst());
                CanAdvance(false);
                break;
            case Round.Flop:
                moveHistory.text += "\nThe turn is shown";
                cardManager.Turn();
                currentRound = Round.Turn;
                GetHands();
                StartCoroutine(Player1MoveFirst());
                CanAdvance(false);
                break;
            case Round.Turn:
                moveHistory.text += "\nThe river is shown";
                cardManager.River();
                currentRound = Round.River;
                GetHands();
                StartCoroutine(Player1MoveFirst());
                CanAdvance(false);
                break;
            case Round.River:
                moveHistory.text += "\nShow cards";
                cardManager.ShowHand(currentPot, dealerFolded, player1Folded);
                currentRound = Round.Show;
                CanAdvance(false);
                break;
            default:
                return;
        }
    }

    void GetHands()
    {
        dealerHand = HandManager.GetHand(cardManager.dealerCards, cardManager.tableCards);
        tableHand = HandManager.GetHand(cardManager.tableCards);
        if (numPlayers == 3)
        {
            player1Hand = HandManager.GetHand(cardManager.player1Cards, cardManager.tableCards);
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
                betButton1.GetComponentInChildren<TextMeshProUGUI>().text = "Bet $" + getBetButton1Amount();
                betButton2.GetComponentInChildren<TextMeshProUGUI>().text = "Bet $" + getBetButton2Amount();
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

    int getBetButton1Amount(int pot)
    {
        if (pot > 30)
        {
            return 10;
        }
        if (pot > 15)
        {
            return 5;
        }
        if (pot > 12)
        {
            return 4;
        }
        if (pot > 4)
        {
            return 2;
        }
        return 1;
    }

    int getBetButton1Amount()
    {
        return getBetButton1Amount(currentPot);
    }

    int getBetButton2Amount(int pot)
    {
        if (pot > 30)
        {
            return 20;
        }
        if (pot > 15)
        {
            return 10;
        }
        if (pot > 12)
        {
            return 8;
        }
        if (pot > 4)
        {
            return 5;
        }
        return 2;
    }

    int getBetButton2Amount()
    {
        return getBetButton2Amount(currentPot);
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
