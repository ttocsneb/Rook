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

    [SyncVar]
    public GameMan gameManager;

    private Sprite front;
    [SyncVar]
    private CardColor color = CardColor.NONE;
    [SyncVar]
    private int number = 1;

    private bool isTrump = false;
    private bool isPlayable = true;
    private bool shouldBeVisible = false;
    private bool isVisible = false;

    private CardDisplay display;

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

    public override void OnStartClient()
    {
        // Make sure that the card starts out hidden
        display = GetComponent<CardDisplay>();
        if (!isVisible) {
            display.SetVisibility(false);
        }
    }

    /// Change this card's face value
    /// 
    /// @param color card color
    /// @param number card number
    [Server]
    public void SrvSetCard(CardColor color, int number)
    {
        this.color = color;
        this.number = number;
        RpcCardChanged(color, number);
    }

    public CardColor GetColor()
    {
        if (isVisible || isServer) 
        {
            return color;
        }
        return CardColor.NONE;
    }

    //I had to add this for the hand highlighting because the other one was only returning NONE
    public CardColor GetColorAlways()
    {
        return color;
    }

    public int GetNumber()
    {
        if (isVisible || isServer) 
        {
            return number;
        }
        return 0;
    }

    /// Play this card and advance to the next turn
    [Command]
    public void CmdPlay()
    {
        Debug.Log("Play Card");
        gameManager.SrvMoveCard(gameObject, CardAreas.DROPAREA);
        gameManager.SrvNextTurn();
    }

    /// Called when the visibility of this card has changed
    ///
    /// It will remain visible to the client that has authority over this card
    ///
    /// @param visible whether or not the card should be visible
    [ClientRpc]
    public void RpcSetVisible(bool visible)
    {
        shouldBeVisible = visible;
        if (hasAuthority) {
            isVisible = true;
            display.SetVisibility(true);
        } else {
            isVisible = visible;
            display.SetVisibility(visible);
        }
    }


    public void SetPlayable(bool playable)
    {
        isPlayable = playable;
        display.SetPlayable(isPlayable);
    }

    /// Change the face color of trump
    ///
    /// @param trumpColor the trump color
    ///
    /// If trumpColor is NONE, then no trump color should be set
    [ClientRpc]
    public void RpcSetTrump(CardColor trumpColor)
    {
        isTrump = color == trumpColor;
        display.SetTrump(isTrump);
    }

    /// Change the face color of the current trick
    ///
    /// @param trickColor the trick color
    ///
    /// If trumpColor is NONE, then no trump color should be set
    public void SetTrickColor(CardColor trickColor)
    {
        isPlayable = trickColor == CardColor.NONE || color == CardColor.ROOK || color == trickColor;
        display.SetPlayable(isPlayable);
    }

    /// Called when the color or number has changed
    ///
    /// @param color the new color
    /// @param number the new number
    [ClientRpc]
    public void RpcCardChanged(CardColor color, int number)
    {
        display.SetCard(color, number);
    }

    /// Check if the card is playable
    [Client]
    public bool CltIsPlayable() {
        return isPlayable;
    }

    [Client]
    public override void OnStartAuthority() {
        // Now that the client has ownership of this card, the card should be made visible
        display.SetVisibility(true);
    }

    [Client]
    public override void OnStopAuthority() {
        // Now that the client does not own this card, the card's visibility should be
        // whatever the server expects it to be
        display.SetVisibility(shouldBeVisible);
    }

}
