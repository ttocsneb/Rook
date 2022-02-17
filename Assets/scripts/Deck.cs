using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Deck : MonoBehaviour
{

    public GameObject[] cards;
    public GameObject hand1;
    public GameObject hand2;
    public GameObject hand3;
    public GameObject hand4;

    private List<GameObject> deck = new List<GameObject>();

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
        GameObject toInit = deck[deck.Count - 1];
        deck.RemoveAt(deck.Count - 1);
        return Instantiate(toInit, new Vector2(0, 0), rotation);
    }

    // Deal 1 card to each player
    public void Deal()
    {
        GameObject card1 = Draw(Quaternion.identity);
        card1.transform.SetParent(hand1.transform, false);
        GameObject card2 = Draw(Quaternion.Euler(x:0, y:0, z:90));
        card2.transform.SetParent(hand2.transform, false);
        GameObject card3 = Draw(Quaternion.Euler(x:0, y:0, z:180));
        card3.transform.SetParent(hand3.transform, false);
        GameObject card4 = Draw(Quaternion.Euler(x:0, y:0, z:270));
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
            GameObject value = deck[k];
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
        foreach (GameObject card in cards)
        {
            deck.Add(card);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
