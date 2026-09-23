using System.Collections.Generic;

public class Hand
{
    public string handText;
    public List<Card> handCards;
    public int score;
    public List<Card> kickers;
    public int numInARow;
    public int numSameSuit;

    public Hand()
    {
        handCards = new List<Card>();
        kickers = new List<Card>();
    }
}
