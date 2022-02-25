using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour
{
    public int maxCards = 4;
    public GameObject deck;

    private readonly List<GameObject> cards = new List<GameObject>();
    private CardColor trickColor = CardColor.NONE;
    private CardColor trumpColor = CardColor.NONE;

    // Start is called before the first frame update
    void Start()
    {
        SetRandomTrumpColor();
    }

    public void RemoveFromDropZone(GameObject card)
    {
        cards.Remove(card);
    }

    public void SetRandomTrumpColor()
    {
        // This is temporary, please remove
        switch (Random.Range(0, 4))
        {
            case 0:
                SetTrumpColor(CardColor.GREEN);
                break;
            case 1:
                SetTrumpColor(CardColor.YELLOW);
                break;
            case 2:
                SetTrumpColor(CardColor.RED);
                break;
            case 3:
                SetTrumpColor(CardColor.BLACK);
                break;
        }
    }

    public void NextTrick()
    {
        while (cards.Count > 0)
        {
            cards[0].GetComponent<Card>().Remove();
        }
        cards.Clear();
        SetTrickColor(CardColor.NONE);
        SetRandomTrumpColor();
    }

    public bool CanDrop(Card card)
    {
        if (cards.Count >= maxCards)
        {
            return false;
        }
        if (trickColor != CardColor.NONE && card.GetColor() != CardColor.ROOK && card.GetColor() != trumpColor)
        {
            if (card.GetColor() != trickColor)
            {
                return false;
            }
        }
        return true;
    }

    public void SetTrumpColor(CardColor color)
    {
        trumpColor = color;
        Deck d = deck.GetComponent<Deck>();
        d.SetTrumpColor(trumpColor);
    }

    private void SetTrickColor(CardColor color)
    {
        trickColor = color;
        Deck d = deck.GetComponent<Deck>();
        d.SetTrickColor(trickColor);
    }

    /// <summary>
    /// Try to drop the card into this dropzone
    /// </summary>
    /// <param name="card">A card that has the Card component</param>
    /// <returns>whether or not the card was successfully dropped</returns>
    public bool Drop(GameObject card)
    {
        Card c = card.GetComponent<Card>();
        if (!CanDrop(c))
        {
            return false;
        }
        card.transform.SetParent(transform, false);
        card.transform.rotation = Quaternion.Euler(x: 0, y: 0, z: 0);

        c.SetVisible(true);
        cards.Add(card);
        if (trickColor == CardColor.NONE && c.GetColor() != CardColor.ROOK && c.GetColor() != trumpColor)
        {
            SetTrickColor(c.GetColor());
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
