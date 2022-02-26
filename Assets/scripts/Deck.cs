using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{

    public GameObject card;
    public GameObject hand1;
    public GameObject hand2;
    public GameObject hand3;
    public GameObject hand4;
    public GameObject dropZone;
    public GameObject canvas;
    public GameObject kitty;
    public bool showAllCards = false;

    private readonly List<CardData> deck = new List<CardData>();
    private readonly List<GameObject> cards = new List<GameObject>();

    private CardColor trickColor = CardColor.NONE;
    private CardColor trumpColor = CardColor.NONE;

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
            Image i = gameObject.GetComponent<Image>();
            i.color = new Color32(255, 255, 255, 0);
            return null;
        }
        CardData toInit = deck[deck.Count - 1];
        deck.RemoveAt(deck.Count - 1);
        GameObject obj = Instantiate(card, new Vector2(0, 0), rotation);
        DragDrop dd = obj.GetComponent<DragDrop>();
        dd.Canvas = canvas;
        dd.DropZone = dropZone;
        Card c = obj.GetComponent<Card>();
        c.deck = this;
        c.SetCard(toInit.color, toInit.number);
        if (showAllCards)
        {
            c.SetVisible(true);
        }
        c.SetTrump(trumpColor);
        c.SetPlayable(trickColor);
        cards.Add(obj);
        return obj;
    }

    public void SetTrickColor(CardColor color)
    {
        trickColor = color;

        foreach (GameObject card in cards)
        {
            Card c = card.GetComponent<Card>();
            c.SetPlayable(trickColor);
        }
    }

    public void SetTrumpColor(CardColor color)
    {
        trumpColor = color;

        foreach (GameObject card in cards)
        {
            Card c = card.GetComponent<Card>();
            c.SetTrump(trumpColor);
        }
    }

    public void RemoveFromDeck(GameObject card)
    {
        cards.Remove(card);
    }

    public void DealAll() 
    {
        int cardsPerHand = 13;
        for (int i = 0; i < cardsPerHand + 1; i++)
        {
            Deal();
        }
    }

    // Deal 1 card to each player
    public void Deal()
    {
        GameObject card1 = Draw(Quaternion.identity);
        if (card1 == null)
        {
            return;
        }
        card1.GetComponent<Card>().SetVisible(true);
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
        if (kitty.transform.childCount < 5)
        {
            GameObject card5 = Draw(Quaternion.Euler(x:0, y:0, z:0));
            if (card5 == null)
            {
                return;
            }
            card5.transform.SetParent(kitty.transform, false);
        }
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
        while (cards.Count > 0)
        {
            cards[0].GetComponent<Card>().Remove();
        }

        deck.Clear();
        CardColor color = CardColor.GREEN;
        for (int i = 1; i <= 14; ++i)
        {
            deck.Add(new CardData(color, i));
        }
        color = CardColor.YELLOW;
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
        Image im = gameObject.GetComponent<Image>();
        im.color = new Color32(255, 255, 255, 255);
        Shuffle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
