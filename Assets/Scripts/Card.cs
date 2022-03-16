using Mirror;
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

public class Card : NetworkBehaviour
{

    public Texture2D texture;
    public Sprite back;
    public int width;
    public int height;

    public static int greenStartIndex = 0;
    public static int yellowStartIndex = 14;
    public static int redStartIndex = 28;
    public static int blackStartIndex = 42;
    public static int rookIndex = 56;

    private Sprite front;
    private CardColor color = CardColor.NONE;
    private int number = 1;
    private bool isVisible = false;

    private bool isTrump = false;
    private bool isPlayable = false;
    private bool shouldBeVisible = false;
    private int id = 0;

    public string GetCardName()
    {
        if (!isVisible) {
            return "Hidden";
        }
        string name;
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
                name = "Rook ";
                break;
            default:
                name = "Error ";
                break;
        }
        return name + number;
    }

    private static int getColorIndex(CardColor color)
    {
        switch (color)
        {
            case CardColor.GREEN: return greenStartIndex;
            case CardColor.YELLOW: return yellowStartIndex;
            case CardColor.RED: return redStartIndex;
            case CardColor.BLACK: return blackStartIndex;
            case CardColor.ROOK: return rookIndex;
            default: return 0;
        }
    }

    [Server]
    public void SetCard(CardColor color, int number)
    {
        this.color = color;
        this.number = number;
        id = getColorIndex(color) + number - 1;
        loadImage();
    }

    public CardColor GetColor()
    {

        if (isVisible || isServer) 
        {
            return color;
        }
        return CardColor.NONE;
    }

    public int GetNumber()
    {
        if (isVisible || isServer) 
        {
            return number;
        }
        return 0;
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

    private void setVisibility(bool visible)
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
        setIndicator();
    }

    [Command]
    public void Play()
    {
        Debug.Log("Play Card");
    }

    [ClientRpc]
    public void RpcSetVisible(bool visible)
    {
        shouldBeVisible = visible;
        if (hasAuthority) {
            setVisibility(true);
        } else {
            setVisibility(visible);
        }
    }

    [ClientRpc]
    public void RpcSetTrump(CardColor trumpColor)
    {
        isTrump = color == trumpColor;
        setIndicator();
    }

    [ClientRpc]
    public void RpcSetTrickColor(CardColor trickColor)
    {
        isPlayable = trickColor == CardColor.NONE || color == CardColor.ROOK || color == trickColor;
        setIndicator();
    }

    private void setIndicator()
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

    public override void OnStartAuthority() {
        setVisibility(true);
    }

    public override void OnStopAuthority() {
        setVisibility(shouldBeVisible);
    }
}
