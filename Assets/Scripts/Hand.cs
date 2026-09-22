using System.Collections.Generic;

public class Hand
{
    public string handText;
    public List<Card> handCards;
    public int score;

    public Hand()
    {
        handCards = new List<Card>();
    }
}
