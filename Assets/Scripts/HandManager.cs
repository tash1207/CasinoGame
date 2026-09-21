using System.Collections.Generic;
using UnityEngine;

public static class HandManager
{
    public static string GetHandText(List<Card> handCards, List<Card> tableCards)
    {
        handCards.AddRange(tableCards);
        handCards.Sort((p1, p2) => p2.value.CompareTo(p1.value));

        // Debug.Log("Full hand of cards: ");
        // foreach (var card in handCards)
        // {
        //     Debug.Log(card.valueName + " of " + card.GetSuitName());
        // }

        return CheckStraightFlush(handCards);
    }

    static string CheckStraightFlush(List<Card> handAndTableCards)
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
            Debug.Log("Flush of Spades");
            if (CheckStraight(spades))
                return HasRoyalCards(spades) ? "Royal Flush of Spades" : "Straight Flush of Spades";
        }
        if (hearts.Count >= 5)
        {
            Debug.Log("Flush of Hearts");
            if (CheckStraight(hearts))
                return HasRoyalCards(hearts) ? "Royal Flush of Hearts" :  "Straight Flush of Hearts";
        }
        if (clubs.Count >= 5)
        {
            Debug.Log("Flush of Clubs");
            if (CheckStraight(clubs))
                return HasRoyalCards(clubs) ? "Royal Flush of Clubs" :  "Straight Flush of Clubs";
        }
        if (diamonds.Count >= 5)
        {
            Debug.Log("Flush of Diamonds");
            if (CheckStraight(diamonds))
                return HasRoyalCards(diamonds) ? "Royal Flush of Diamonds" :  "Straight Flush of Diamonds";
        }
        return CheckFoursAndFullHouse(handAndTableCards);
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

    static string CheckFoursAndFullHouse(List<Card> handAndTableCards)
    {
        Debug.Log("Check Fours and Full House");
        int numPairs = 0;
        int numThrees = 0;
        List<Card> winningCards = new List<Card>();
        for (int i = 0; i < handAndTableCards.Count - 1; i++)
        {
            Card currentCard = handAndTableCards[i];
            if (currentCard.value == handAndTableCards[i + 1].value)
            {
                winningCards.Add(handAndTableCards[i]);
                winningCards.Add(handAndTableCards[i + 1]);
                if (i + 2 < handAndTableCards.Count && currentCard.value == handAndTableCards[i + 2].value)
                {
                    winningCards.Add(handAndTableCards[i + 2]);
                    if (i + 3 < handAndTableCards.Count && currentCard.value == handAndTableCards[i + 3].value)
                    {
                        winningCards.Add(handAndTableCards[i + 3]);
                        return "4 of a kind: " + handAndTableCards[i].valueName + "s";
                    }
                    Debug.Log("3 of a kind of " + handAndTableCards[i].valueName + "s");
                    numThrees++;
                    i++;
                }
                else
                {
                    Debug.Log("Pair of " + handAndTableCards[i].valueName + "s");
                    numPairs++;
                }
            }
        }
        if (numThrees == 1 && numPairs > 0) return "Full House!";
        return CheckFlush(handAndTableCards);
    }

    static string CheckFlush(List<Card> handAndTableCards)
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
        {
            Debug.Log("Flush of Spades");
            return "Flush of Spades";
        }
        if (numHearts >= 5)
        {
            Debug.Log("Flush of Hearts");
            return "Flush of Hearts";
        }
        if (numClubs >= 5)
        {
            Debug.Log("Flush of Clubs");
            return "Flush of Clubs";
        }
        if (numDiamonds >= 5)
        {
            Debug.Log("Flush of Diamonds");
            return "Flush of Diamonds";
        }
        return CheckStraightString(handAndTableCards);
    }

    static string CheckStraightString(List<Card> handAndTableCards)
    {
        if (CheckStraight(handAndTableCards)) return "Straight";
        else return CheckPairs(handAndTableCards);
    }

    static bool CheckStraight(List<Card> handAndTableCards)
    {
        int maxNumInARow = 1;
        int numInARow = 1;
        //List<Card> winningCards = new List<Card>();
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
            return true;
        }

        //return CheckPairs(handAndTableCards);
        return false;
    }

    static string CheckPairs(List<Card> handAndTableCards)
    {
        Debug.Log("Check Pairs");
        int numPairs = 0;
        List<Card> winningCards = new List<Card>();
        string returnString = "";
        for (int i = 0; i < handAndTableCards.Count - 1; i++)
        {
            Card currentCard = handAndTableCards[i];
            if (currentCard.value == handAndTableCards[i + 1].value)
            {
                winningCards.Add(handAndTableCards[i]);
                winningCards.Add(handAndTableCards[i + 1]);
                if (i + 2 < handAndTableCards.Count && currentCard.value == handAndTableCards[i + 2].value)
                {
                    winningCards.Add(handAndTableCards[i + 2]);
                    Debug.Log("3 of a kind of " + handAndTableCards[i].valueName + "s");
                    return "3 of a kind: " + handAndTableCards[i].valueName + "s";
                }
                else
                {
                    Debug.Log("Pair of " + handAndTableCards[i].valueName + "s");
                    numPairs++;
                    if (numPairs < 3)
                        returnString += "Pair of " + handAndTableCards[i].valueName + "s\n";
                }
            }
        }
        // TODO: Use winning cards to score hand, highlight cards, and add more info to string.
        if (numPairs > 1) return "Two pair!\n" + returnString;
        return returnString != "" ? returnString : CheckHighCard(handAndTableCards);
    }

    static string CheckHighCard(List<Card> handAndTableCards)
    {
        Card highCard = handAndTableCards[0];
        Debug.Log("High Card: " + highCard.valueName + " of " + highCard.GetSuitName());
        return "High Card: " + highCard.valueName;
    }
}
