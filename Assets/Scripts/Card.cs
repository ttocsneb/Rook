using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public enum CardColor
{
    NONE,
    GREEN,
    YELLOW,
    RED,
    BLACK,
    ROOK,
}

public class Card : MonoBehaviour
{

    public Texture2D texture;
    public Sprite back;
    public int width;
    public int height;
    public Deck deck;

    public int greenPosition = 0;
    public int yellowPosition = 14;
    public int redPosition = 28;
    public int blackPosition = 42;
    public int rookPosition = 56;

    private Sprite front;

    private CardColor color = CardColor.NONE;
    private int number = 1;
    private bool isVisible = false;

    private bool isTrump = false;
    private bool isPlayable = false;

    private int id = 0;

    public void Remove()
    {
        DragDrop dd = GetComponent<DragDrop>();
        if (transform.parent == dd.DropZone.transform)
        {
            dd.DropZone.GetComponent<DropZone>().RemoveFromDropZone(gameObject);
        }
        deck.RemoveFromDeck(gameObject);
        Destroy(gameObject);
    }

    public string GetCardName()
    {
        string name = "Error ";
        switch (color)
        {
            case CardColor.NONE:
                name = "None ";
                break;
            case CardColor.GREEN:
                name = "Green ";
                break;
            case CardColor.YELLOW:
                name = "Yellow ";
                break;
            case CardColor.RED:
                name = "Red ";
                break;
            case CardColor.BLACK:
                name = "Black ";
                break;
            case CardColor.ROOK:
                name = "Rook";
                break;
        }
        return name + number;
    }

    private int GetPosition(CardColor color)
    {
        switch (color)
        {
            case CardColor.GREEN: return greenPosition;
            case CardColor.YELLOW: return yellowPosition;
            case CardColor.RED: return redPosition;
            case CardColor.BLACK: return blackPosition;
            case CardColor.ROOK: return rookPosition;
            default: return 0;
        }
    }

    public void SetCard(CardColor color, int number)
    {
        this.color = color;
        this.number = number;
        id = GetPosition(color) + number - 1;
        loadImage();
    }

    public CardColor GetColor()
    {
        return color;
    }

    public int GetNumber()
    {
        return number;
    }

    private void loadImage()
    {
        int x = id % width;
        int y = id / width;
        int w = texture.width / width;
        int h = texture.height / height;
        Rect rect = new Rect(x * w, texture.height - y * h - h, w, h);

        front = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        Image i = gameObject.GetComponent<Image>();
        if (visible)
        {
            if (front == null)
            {
                Debug.Log(GetCardName() + ": The card has not been loaded yet!");
            }
            i.sprite = front;
        }
        else
        {
            i.sprite = back;
        }
        SetIndicator();
    }

    public void SetTrump(CardColor trumpColor)
    {
        isTrump = color == trumpColor;
        SetIndicator();
    }

    public void SetPlayable(CardColor trickColor)
    {
        isPlayable = trickColor == CardColor.NONE || color == CardColor.ROOK || color == trickColor;
        SetIndicator();
    }

    private void SetIndicator()
    {
        Image i = gameObject.GetComponent<Image>();
        if (isVisible)
        {
            if (isTrump)
            {
                i.color = new Color32(118, 193, 244, 255);
                return;
            }
            if (!isPlayable)
            {
                i.color = new Color32(150, 150, 150, 255);
                return;
            }
        }
        i.color = new Color32(255, 255, 255, 255);
    }

    public bool IsVisible()
    {
        return isVisible;
    }


    // Start is called before the first frame update
    void Start()
    {
        Image i = gameObject.GetComponent<Image>();
        if (i.sprite == null)
        {
            SetVisible(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
