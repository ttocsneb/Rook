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

    private GameMan gameManager;

    public GameObject myArea;
    public Quaternion rotation;

    [SyncVar(hook = nameof(OnPlayerPosUpdate))]
    public int playerPosition = -1;

    public override void OnStartClient()
    {
        gameManager = GameObject.Find("GameManger").GetComponent<GameMan>();
        gameManager.ClientRegisterPlayer(this);
        enemyArea1 = gameManager.enemyArea1;
        enemyArea2 = gameManager.enemyArea2;
        enemyArea3 = gameManager.enemyArea3;
        playerArea = gameManager.playerArea;
        Debug.Log("OnStart");
        UpdateMyArea();
    }

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
        Debug.Log("Card moved");
        card.transform.SetParent(myArea.transform, false);
        card.transform.rotation = rotation;
    }

    [Client]
    public void UpdateMyArea() 
    {
        if (gameManager == null || playerPosition == -1 || gameManager.GetPlayerOwner() == -1) {
            Debug.Log("Player Manager not yet initialized");
            return;
        }
        Debug.Log("Updating my area");
        int playerOwner = gameManager.GetPlayerOwner();
        int relativePosition = playerPosition - playerOwner;
        if (relativePosition < 0) {
            relativePosition += 4;
        }
        Debug.Log("Player Position: " + playerOwner + ", my position: " + playerPosition + ", relative position: " + relativePosition);

        switch (relativePosition)
        {
            case 0:
                myArea = playerArea;
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 1:
                myArea = enemyArea1;
                rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 2:
                myArea = enemyArea2;
                rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 3:
                myArea = enemyArea3;
                rotation = Quaternion.Euler(0, 0, 270);
                break;
            default:
                Debug.Log("Invalid relative position " + relativePosition + "!");
                break;
        }
    }

    [ClientRpc]
    public void RpcPlayerPosUpdate(int pos)
    {
        playerPosition = pos;
        UpdateMyArea();
    }

    void OnPlayerPosUpdate(int oldPos, int newPos)
    {
        UpdateMyArea();
    }

}
