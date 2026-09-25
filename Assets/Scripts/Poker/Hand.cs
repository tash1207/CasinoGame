using System.Collections.Generic;

public class Hand
{
    public string handText;
    public List<Card> handCards;
    public int score;
    public List<Card> kickers;
    public int numInARow = 0;
    public int numSameSuit = 0;
    public Card.Suit suit;

    public Hand()
    {
        handCards = new List<Card>();
        kickers = new List<Card>();
    }
}
