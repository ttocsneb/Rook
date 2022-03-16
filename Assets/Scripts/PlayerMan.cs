using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMan : NetworkBehaviour
{

    public GameObject enemyArea1;
    public GameObject enemyArea2;
    public GameObject enemyArea3;
    public GameObject playerArea;

    public GameObject dropArea;
    public GameObject kittyArea;
    public GameMan gameManager;

    public int playerPosition;

    [Command]
    public void PlayCard(GameObject card)
    {
        PlayFailed(card);
    }

    // Called when the 
    [TargetRpc]
    public void PlayFailed(GameObject card)
    {
        Debug.Log("Play Failed");
    }

    // Called when a card is moved to this player's hand
    [ClientRpc]
    public void CardMoved(GameObject card, CardAreas area)
    {
        Debug.Log("Card moved");
    }

}
