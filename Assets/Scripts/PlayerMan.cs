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

    private GameObject myArea;
    private Quaternion rotation;

    private readonly List<GameObject> cards = new List<GameObject>();

    [SyncVar(hook = nameof(CltOnPlayerPosUpdate))]
    public int playerPosition = -1;

    public override void OnStartClient()
    {
        Debug.Log("OnStart");
        gameManager = GameObject.Find("GameManger").GetComponent<GameMan>();
        gameManager.CltRegisterPlayer(this);
        enemyArea1 = gameManager.enemyArea1;
        enemyArea2 = gameManager.enemyArea2;
        enemyArea3 = gameManager.enemyArea3;
        playerArea = gameManager.playerArea;
        CltUpdateMyArea();
        base.OnStartClient();
    }

    // Called when a card is moved to this player's hand
    [ClientRpc]
    public void RpcCardMoved(GameObject card, CardAreas area)
    {
        card.transform.SetParent(myArea.transform, false);
        card.transform.rotation = rotation;
    }

    /// Determine which player slot that the this player should be played in
    ///
    /// If the player is the same player that the client controls, then they
    /// will be placed in the playerArea
    ///
    /// Otherwise, one of the enemyAreas will be used for this player. The
    /// enemyArea chosen is determined by the distance away from the client's
    /// player so that all players apear in a counter-clockwise fashion
    [Client]
    public void CltUpdateMyArea() 
    {
        if (gameManager == null || playerPosition == -1 || gameManager.CltGetPlayerOwner() == -1) {
            Debug.Log("Player Manager not yet initialized");
            return;
        }
        Debug.Log("Updating my area");
        int playerOwner = gameManager.CltGetPlayerOwner();
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

    [Client]
    void CltOnPlayerPosUpdate(int oldPos, int newPos)
    {
        CltUpdateMyArea();
    }

}
