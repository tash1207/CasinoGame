using System.Collections.Generic;
using UnityEngine;

public static class HandManager
{

    public static void CheckHand(List<Card> handCards, List<Card> tableCards)
    {
        handCards.AddRange(tableCards);
        handCards.Sort((p1, p2) => p2.value.CompareTo(p1.value));

        // Debug.Log("Full hand of cards: ");
        // foreach (var card in handCards)
        // {
        //     Debug.Log(card.valueName + " of " + card.GetSuitName());
        // }

        CheckStraightFlush(handCards);
        CheckPair(handCards);
        CheckHighCard(handCards);
    }

    static void CheckStraightFlush(List<Card> handAndTableCards)
    {
        CheckFlush(handAndTableCards);
        CheckStraight(handAndTableCards);
    }

    static void CheckFlush(List<Card> handAndTableCards)
    {
        int numSpades = 0;
        int numHearts = 0;
        int numClubs = 0;
        int numDiamonds = 0;

        foreach (var card in handAndTableCards)
        {
            if (card.suit == Card.Suit.SPADE)
                numSpades++;
            if (card.suit == Card.Suit.HEART)
                numHearts++;
            if (card.suit == Card.Suit.CLUB)
                numClubs++;
            if (card.suit == Card.Suit.DIAMOND)
                numDiamonds++;
        }

        if (numSpades >= 5)
            Debug.Log("Flush of Spades");
        if (numHearts >= 5)
            Debug.Log("Flush of Hearts");
        if (numClubs >= 5)
            Debug.Log("Flush of Clubs");
        if (numDiamonds >= 5)
            Debug.Log("Flush of Diamonds");
    }

    static void CheckStraight(List<Card> handAndTableCards)
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

        Debug.Log("maxNumInARow = " + maxNumInARow);
        if (maxNumInARow >= 5)
        {
            Debug.Log("Straight!");
        }
    }

    static void CheckPair(List<Card> handAndTableCards)
    {
        for (int i = 0; i < handAndTableCards.Count - 1; i++)
        {
            Card currentCard = handAndTableCards[i];
            if (currentCard.value == handAndTableCards[i + 1].value)
            {
                if (i + 2 < handAndTableCards.Count - 1 && currentCard.value == handAndTableCards[i + 2].value)
                {
                    Debug.Log("3 of a kind of " + handAndTableCards[i].valueName + "s");
                }
                else
                {
                    Debug.Log("Pair of " + handAndTableCards[i].valueName + "s");
                }
            }
        }
    }

    static void CheckHighCard(List<Card> handAndTableCards)
    {
        Card highCard = handAndTableCards[0];
        Debug.Log("High Card: " + highCard.valueName + " of " + highCard.GetSuitName());
    }
}
