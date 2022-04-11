using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class CardDisplay : MonoBehaviour
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

    private int id = 0;

    private bool isVisible = false;
    private bool isTrump = false;
    private bool isPlayable = true;
    private bool isLoaded = false;

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

    /// Set the card color and number for this card
    ///
    /// @param color color of this card
    /// @param number number of this card
    public void SetCard(CardColor color, int number)
    {
        id = getColorIndex(color) + number - 1;
    }

    private void loadImage()
    {
        /// Calculate the coordinates of the card
        int x = id % width;
        int y = id / width;
        int w = texture.width / width;
        int h = texture.height / height;
        Rect rect = new Rect(x * w, texture.height - y * h - h, w, h);

        front = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

        isLoaded = true;
    }
    
    /// Make the card show its back face or its front face
    ///
    /// @param visible whether the card should be showing its front face or not
    public void SetVisibility(bool visible)
    {
        isVisible = visible;
        Image i = gameObject.GetComponent<Image>();
        if (visible)
        {
            if (!isLoaded) {
                loadImage();
            }
            if (front == null)
            {
                Debug.LogError("The card has not been loaded yet!");
            }
            i.sprite = front;
        }
        else
        {
            i.sprite = back;
        }
        setIndicator();
    }

    /// Mark this card as a trump card
    ///
    /// This will make the card appear differently from other cards to indicate that it
    /// is better than everyone else
    public void SetTrump(bool trump)
    {
        isTrump = trump;
        setIndicator();
    }

    /// Mark whether the card is playable
    /// 
    /// This will make the card appear differently from other cards to indicate that it
    /// should not be played for this trick
    public void SetPlayable(bool playable)
    {
        isPlayable = playable;
        setIndicator();
    }

    /// Update the markers for this card
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

    /// Check if the card is currently visible or not
    public bool IsVisible()
    {
        return isVisible;
    }
}