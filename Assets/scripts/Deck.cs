using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Deck : MonoBehaviour
{

    public GameObject[] cards; 

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
    public void Draw()
    {
        GameObject toInit = deck[deck.Count - 1];
        deck.RemoveAt(deck.Count - 1);
        GameObject playerCard = Instantiate(toInit, new Vector2(0, 0), Quaternion.identity);
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
