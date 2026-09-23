using System;
using System.Collections.Generic;
using UnityEngine;

public static class HandManager
{
    public static Hand GetHand(List<Card> handCards, List<Card> tableCards)
    {
        if (handCards.Count == 0)
        {
            Debug.Log("Checking table hand");
        }
        List<Card> allCards = new List<Card>();
        allCards.AddRange(handCards);
        allCards.AddRange(tableCards);
        allCards.Sort((p1, p2) => p2.value.CompareTo(p1.value));

        Hand hand = new Hand();
        HandCheckStraightFlush(hand, allCards);

        Debug.Log("Hand: " + hand.handText);
        Debug.Log("Score: " + hand.score);

        return hand;
    }

    static void HandCheckStraightFlush(Hand hand, List<Card> handAndTableCards)
    {
        List<Card> spades = new List<Card>();
        List<Card> hearts = new List<Card>();
        List<Card> clubs = new List<Card>();
        List<Card> diamonds = new List<Card>();

        foreach (var card in handAndTableCards)
        {
            if (card.suit == Card.Suit.SPADE)
                spades.Add(card);
            if (card.suit == Card.Suit.HEART)
                hearts.Add(card);
            if (card.suit == Card.Suit.CLUB)
                clubs.Add(card);
            if (card.suit == Card.Suit.DIAMOND)
                diamonds.Add(card);
        }

        if (spades.Count >= 5 && CheckStraight(spades))
        {
            GetStraightFlushHand(hand, spades);
            return;
        }
        else if (hearts.Count >= 5 && CheckStraight(hearts))
        {
            GetStraightFlushHand(hand, hearts);
            return;
        }
        if (clubs.Count >= 5 && CheckStraight(clubs))
        {
            GetStraightFlushHand(hand, clubs);
            return;
        }
        if (diamonds.Count >= 5 && CheckStraight(diamonds))
        {
            GetStraightFlushHand(hand, diamonds);
            return;
        }

        HandCheckFoursAndFullHouse(hand, handAndTableCards);
    }

    static void GetStraightFlushHand(Hand hand, List<Card> cards)
    {
        if (CheckStraight(cards))
        {
            hand.handCards = cards.GetRange(0, 5);
            if (HasRoyalCards(cards))
            {
                hand.handText = "Royal Flush of " + cards[0].GetSuitName();
                hand.score = 10000;
            }
            else
            {
                hand.handText = "Straight Flush of " + cards[0].GetSuitName();
                int topCardValue = cards[0].value;
                if (cards[0].value == 14 && cards[1].value == 5) // A2345 straight
                {
                    topCardValue = 5;
                }
                hand.score = 9000 + topCardValue;
            }
        }
    }

    static void HandCheckFoursAndFullHouse(Hand hand, List<Card> handAndTableCards)
    {
        int numThrees = 0;
        int numPairs = 0;
        Card threeVal = null;
        Card pairVal = null;

        List<Card> winningCards = new List<Card>();
        for (int i = 0; i < handAndTableCards.Count - 1; i++)
        {
            Card currentCard = handAndTableCards[i];
            if (currentCard.value == handAndTableCards[i + 1].value)
            {
                if (i + 2 < handAndTableCards.Count && currentCard.value == handAndTableCards[i + 2].value)
                {
                    winningCards.Add(handAndTableCards[i]);
                    winningCards.Add(handAndTableCards[i + 1]);
                    winningCards.Add(handAndTableCards[i + 2]);
                    if (i + 3 < handAndTableCards.Count && currentCard.value == handAndTableCards[i + 3].value)
                    {
                        winningCards.Add(handAndTableCards[i + 3]);
                        hand.handText = "4 of a kind: " + handAndTableCards[i].valueName + "s";
                        hand.handCards = winningCards;
                        hand.score = 8000 + (handAndTableCards[i].value * 10);

                        foreach (var card in winningCards)
                        {
                            handAndTableCards.Remove(card);
                        }
                        hand.handCards.Add(handAndTableCards[0]);
                        hand.kickers.Add(handAndTableCards[0]);
                        return;
                    }
                    numThrees++;
                    threeVal = handAndTableCards[i];
                    i++;
                }
                else
                {
                    if (numPairs == 0)
                    {
                        winningCards.Add(handAndTableCards[i]);
                        winningCards.Add(handAndTableCards[i + 1]);
                        numPairs++;
                        pairVal = handAndTableCards[i];
                    }
                }
            }
        }
        if (numThrees == 1 && numPairs > 0 && threeVal != null && pairVal != null) {
            hand.handText = "Full House:\n" + threeVal.valueName + "s full of " + pairVal.valueName + "s";
            hand.handCards = winningCards;
            hand.score = 7000 + (threeVal.value * 10) + pairVal.value;
            return;
        }
        
        HandCheckFlush(hand, handAndTableCards);
    }

    static void HandCheckFlush(Hand hand, List<Card> handAndTableCards)
    {
        List<Card> spades = new List<Card>();
        List<Card> hearts = new List<Card>();
        List<Card> clubs = new List<Card>();
        List<Card> diamonds = new List<Card>();

        foreach (var card in handAndTableCards)
        {
            if (card.suit == Card.Suit.SPADE)
                spades.Add(card);
            if (card.suit == Card.Suit.HEART)
                hearts.Add(card);
            if (card.suit == Card.Suit.CLUB)
                clubs.Add(card);
            if (card.suit == Card.Suit.DIAMOND)
                diamonds.Add(card);
        }

        if (spades.Count >= 5)
        {
            GetFlushHand(hand, spades);
            return;
        }
        if (hearts.Count >= 5)
        {
            GetFlushHand(hand, hearts);
            return;
        }
        if (clubs.Count >= 5)
        {
            GetFlushHand(hand, clubs);
            return;
        }
        if (diamonds.Count >= 5)
        {
            GetFlushHand(hand, diamonds);
            return;
        }

        HandCheckStraight(hand, handAndTableCards);
    }

    static void GetFlushHand(Hand hand, List<Card> cards)
    {
        hand.handCards = cards.GetRange(0, 5);
        hand.handText = "Flush of " + cards[0].GetSuitName();
        hand.score = 6000 + (cards[0].value * 10) + cards[1].value;
        hand.kickers = hand.handCards;
    }

    static void HandCheckStraight(Hand hand, List<Card> handAndTableCards)
    {
        if (CheckStraight(handAndTableCards))
        {
            int maxNumInARow = 1;
            int numInARow = 1;
            List<Card> winningCards = new List<Card>();
            for (int i = 0; i < handAndTableCards.Count; i++)
            {
                Card currentCard = handAndTableCards[i];
                if (!winningCards.Contains(currentCard))
                    winningCards.Add(currentCard);
                if (currentCard.value == 2 && handAndTableCards[0].value == 14)
                {
                    winningCards.Add(handAndTableCards[0]);
                    numInARow++;
                    if (numInARow > maxNumInARow)
                    {
                        maxNumInARow = numInARow;
                    }
                    break;
                }
                if (i + 1 >= handAndTableCards.Count)
                {
                    break;
                }
                if (currentCard.value == handAndTableCards[i + 1].value)
                {
                    winningCards.Remove(currentCard);
                    continue;
                }
                else if (currentCard.value == handAndTableCards[i + 1].value + 1)
                {
                    winningCards.Add(handAndTableCards[i + 1]);
                    numInARow++;
                    if (numInARow > maxNumInARow)
                    {
                        maxNumInARow = numInARow;
                    }
                }
                else
                {
                    if (numInARow > maxNumInARow)
                    {
                        maxNumInARow = numInARow;
                    }
                    if (maxNumInARow >= 5)
                    {
                        hand.handText = "Straight";
                        hand.handCards = winningCards.GetRange(0, 5);
                        int topCardValue = hand.handCards[0].value;
                        if (hand.handCards[0].value == 14 && hand.handCards[1].value == 5) // A2345 straight
                        {
                            topCardValue = 5;
                        }
                        hand.score = 5000 + topCardValue;
                        return;
                    }
                    numInARow = 1;
                    winningCards.Clear();
                }
            }

            hand.numInARow = maxNumInARow;

            if (maxNumInARow >= 5)
            {
                hand.handText = "Straight";
                hand.handCards = winningCards.GetRange(0, 5);
                int topCardValue = hand.handCards[0].value;
                if (hand.handCards[0].value == 14 && hand.handCards[1].value == 5) // A2345 straight
                {
                    topCardValue = 5;
                }
                hand.score = 5000 + topCardValue;
                return;
            }
        }

        HandCheckPairs(hand, handAndTableCards);
    }

    static void HandCheckPairs(Hand hand, List<Card> handAndTableCards)
    {
        int numPairs = 0;
        string handString = "";
        List<Card> winningCards = new List<Card>();
        for (int i = 0; i < handAndTableCards.Count - 1; i++)
        {
            Card currentCard = handAndTableCards[i];
            if (currentCard.value == handAndTableCards[i + 1].value)
            {
                if (i + 2 < handAndTableCards.Count && currentCard.value == handAndTableCards[i + 2].value)
                {
                    winningCards.Add(handAndTableCards[i]);
                    winningCards.Add(handAndTableCards[i + 1]);
                    winningCards.Add(handAndTableCards[i + 2]);

                    hand.handText = "3 of a kind: " + handAndTableCards[i].valueName + "s";
                    hand.handCards = winningCards;
                    hand.score = 4000 + handAndTableCards[i].value;

                    foreach (var card in winningCards)
                    {
                        handAndTableCards.Remove(card);
                    }
                    hand.handCards.Add(handAndTableCards[0]);
                    hand.handCards.Add(handAndTableCards[1]);
                    hand.kickers.Add(handAndTableCards[0]);
                    hand.kickers.Add(handAndTableCards[1]);
                    return;
                }
                else
                {
                    numPairs++;
                    if (numPairs < 3)
                    {
                        winningCards.Add(handAndTableCards[i]);
                        winningCards.Add(handAndTableCards[i + 1]);
                        handString += "Pair of " + handAndTableCards[i].valueName + "s\n";
                    }
                }
            }
        }

        if (numPairs > 1) 
        {
            hand.handText = "Two pair:\n" + handString;
            hand.handCards = winningCards;
            hand.score = 3000 + (winningCards[0].value * 10) + winningCards[2].value;
            foreach (var card in winningCards)
            {
                handAndTableCards.Remove(card);
            }
            hand.handCards.Add(handAndTableCards[0]);
            hand.kickers.Add(handAndTableCards[0]);
            return;
        }
        else if (numPairs == 1)
        {
            hand.handText = handString;
            hand.handCards = winningCards;
            hand.score = 2000 + winningCards[0].value;
            foreach (var card in winningCards)
            {
                handAndTableCards.Remove(card);
            }
            for (int i = 0; i < Math.Min(handAndTableCards.Count, 3); i++)
            {
                hand.handCards.Add(handAndTableCards[i]);
                hand.kickers.Add(handAndTableCards[i]);
            }
            return;
        }
        
        HandCheckHighCard(hand, handAndTableCards);
    }

    static void HandCheckHighCard(Hand hand, List<Card> handAndTableCards)
    {
        Card highCard = handAndTableCards[0];
        hand.handText = "High Card: " + highCard.valueName;
        hand.handCards = handAndTableCards.GetRange(0, Math.Min(handAndTableCards.Count, 5));
        hand.score = 1000 + highCard.value;
        hand.kickers = hand.handCards;
    }

    static bool HasRoyalCards(List<Card> cards)
    {
        bool hasAce = false;
        bool hasKing = false;
        bool hasQueen = false;
        bool hasJack = false;
        bool hasTen = false;

        foreach (var card in cards)
        {
            if (card.value == 14)
            {
                hasAce = true;
            }
            else if (card.value == 13)
            {
                hasKing = true;
            }
            else if (card.value == 12)
            {
                hasQueen = true;
            }
            else if (card.value == 11)
            {
                hasJack = true;
            }
            else if (card.value == 10)
            {
                hasTen = true;
            }
        }
        return hasAce && hasKing && hasQueen && hasJack && hasTen;
    }

    static bool CheckStraight(List<Card> handAndTableCards)
    {
        int maxNumInARow = 1;
        int numInARow = 1;
        for (int i = 0; i < handAndTableCards.Count; i++)
        {
            Card currentCard = handAndTableCards[i];
            if (currentCard.value == 2 && handAndTableCards[0].value == 14)
            {
                numInARow++;
                if (numInARow > maxNumInARow)
                {
                    maxNumInARow = numInARow;
                }
                break;
            }
            if (i + 1 >= handAndTableCards.Count)
            {
                break;
            }
            if (currentCard.value == handAndTableCards[i + 1].value)
            {
                continue;
            }
            else if (currentCard.value == handAndTableCards[i + 1].value + 1)
            {
                numInARow++;
                if (numInARow > maxNumInARow)
                {
                    maxNumInARow = numInARow;
                }
            }
            else
            {
                if (numInARow > maxNumInARow)
                {
                    maxNumInARow = numInARow;
                }
                numInARow = 1;
            }
        }

        if (maxNumInARow >= 5)
        {
            return true;
        }

        return false;
    }
}
