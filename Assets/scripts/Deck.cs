using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{

    public GameObject card;
    public GameObject hand1;
    public GameObject hand2;
    public GameObject hand3;
    public GameObject hand4;

    private readonly List<CardData> deck = new List<CardData>();

    private class CardData
    {
        public CardColor color;
        public int number;

        public CardData(CardColor color, int number)
        {
            this.color = color;
            this.number = number;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Restock();
        Shuffle();
    }

    public bool CanDraw()
    {
        return deck.Count > 0;
    }

    // Draw a single card from the deck
    public GameObject Draw(Quaternion rotation)
    {
        if (deck.Count == 0)
        {
            Debug.Log("No more cards");
            return null;
        }
        CardData toInit = deck[deck.Count - 1];
        deck.RemoveAt(deck.Count - 1);
        GameObject obj = Instantiate(card, new Vector2(0, 0), rotation);
        Card c = obj.GetComponent<Card>();
        c.SetCard(toInit.color, toInit.number);
        return obj;
    }

    // Deal 1 card to each player
    public void Deal()
    {
        GameObject card1 = Draw(Quaternion.identity);
        if (card1 == null)
        {
            return;
        }
        card1.transform.SetParent(hand1.transform, false);
        GameObject card2 = Draw(Quaternion.Euler(x:0, y:0, z:90));
        if (card2 == null)
        {
            return;
        }
        card2.transform.SetParent(hand2.transform, false);
        GameObject card3 = Draw(Quaternion.Euler(x:0, y:0, z:180));
        if (card3 == null)
        {
            return;
        }
        card3.transform.SetParent(hand3.transform, false);
        GameObject card4 = Draw(Quaternion.Euler(x:0, y:0, z:270));
        if (card4 == null)
        {
            return;
        }
        card4.transform.SetParent(hand4.transform, false);
    }

    // Shuffle the deck in place
    public void Shuffle()
    {
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            CardData value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }

    // Restock the deck
    //
    // Note that all existing cards outside of the deck should be removed,
    // otherwise duplicates will be created
    public void Restock()
    {
        deck.Clear();
        CardColor color = CardColor.GREEN;
        for (int i = 1; i <= 14; ++i)
        {
            deck.Add(new CardData(color, i));
        }
        color = CardColor.ORANGE;
        for (int i = 1; i <= 14; ++i)
        {
            deck.Add(new CardData(color, i));
        }
        color = CardColor.RED;
        for (int i = 1; i <= 14; ++i)
        {
            deck.Add(new CardData(color, i));
        }
        color = CardColor.BLACK;
        for (int i = 1; i <= 14; ++i)
        {
            deck.Add(new CardData(color, i));
        }
        deck.Add(new CardData(CardColor.ROOK, 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
