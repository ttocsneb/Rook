using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMan : NetworkBehaviour
{

    // Make private, using searches
    public GameObject enemyArea1;
    public GameObject enemyArea2;
    public GameObject enemyArea3;
    public GameObject playerArea;

    public GameObject dropArea;
    public GameObject kittyArea;
    public GameMan gameManager;

    private GameObject myArea;

    public int playerPosition;

    [Command]
    public void CmdPlayCard(GameObject card)
    {
        TargetPlayFailed(connectionToClient, card);
    }

    // Called when the 
    [TargetRpc]
    public void TargetPlayFailed(NetworkConnection target, GameObject card)
    {
        Debug.Log("Play Failed");
    }

    // Called when a card is moved to this player's hand
    [ClientRpc]
    public void RpcCardMoved(GameObject card, CardAreas area)
    {
        bool is_owner = hasAuthority; // Remeber this!
        Debug.Log("Card moved");
    }

}
