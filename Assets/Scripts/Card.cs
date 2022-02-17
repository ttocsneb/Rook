using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public enum CardColor
{
    GREEN = 0,
    ORANGE = 14,
    RED = 28,
    BLACK = 42,
    ROOK = 56,
}

public class Card : MonoBehaviour
{

    public Texture2D texture;
    public int width;
    public int height;

    private CardColor color = CardColor.GREEN;
    private int number = 1;

    private int id = 0;

    public void SetCard(CardColor color, int number)
    {
        this.color = color;
        this.number = number;
        id = ((int)color) + number - 1;
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

        Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        Image i = gameObject.GetComponent<Image>();
        i.sprite = sprite;
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
