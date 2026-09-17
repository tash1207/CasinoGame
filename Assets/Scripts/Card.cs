using UnityEngine;

public class Card
{
    public enum Suit
    {
        SPADE,
        HEART,
        CLUB,
        DIAMOND
    }
    public int value; // J = 11, Q = 12, K = 13, A = 14
    public string valueName;
    public Suit suit;

    public bool dealt = false;

    public Card(Suit suit, int value, string valueName)
    {
        this.suit = suit;
        this.value = value;
        this.valueName = valueName;
    }
}
