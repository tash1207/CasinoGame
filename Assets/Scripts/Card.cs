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

    public string GetSuitName()
    {
        switch (suit)
        {
            case Suit.SPADE:
                return "Spades";
            case Suit.HEART:
                return "Hearts";
            case Suit.CLUB:
                return "Clubs";
            case Suit.DIAMOND:
                return "Diamonds";
            default:
                return "";
        }
    }
}
