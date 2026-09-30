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
        handCards = new List<Card>(10);
        kickers = new List<Card>(10);
    }

    public void Clear()
    {
        handText = "";
        handCards.Clear();
        score = 0;
        kickers.Clear();
        numInARow = 0;
        numSameSuit = 0;
    }
}
