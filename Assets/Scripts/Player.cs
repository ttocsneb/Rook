using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{

    private GameObject enemyArea1;
    private GameObject enemyArea2;
    private GameObject enemyArea3;
    private GameObject playerArea;

    private GameObject dropArea;
    private GameObject kittyArea;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // enemyArea1 = GameObject.Find("EnemyArea_1");
        // enemyArea2 = GameObject.Find("EnemyArea_2");
        // enemyArea3 = GameObject.Find("EnemyArea_3");
        // playerArea = GameObject.Find("PlayerArea");
        // dropArea = GameObject.Find("DropArea");
        // kittyArea = GameObject.Find("Kitty");
        gameManager = GameObject.Find("NetworkManager").GetComponent<GameManager>();
        enemyArea1 = gameManager.enemyArea1;
        enemyArea1 = gameManager.enemyArea2;
        enemyArea1 = gameManager.enemyArea3;
        playerArea = gameManager.playerArea;
        dropArea = gameManager.dropArea;
        kittyArea = gameManager.kittyArea;
    }

    [Command]
    public void PlayCard(GameObject card)
    {
        PlayFailed(card);
    }

    [TargetRpc]
    public void PlayFailed(GameObject card)
    {
        Debug.Log("Play Failed");
    }

    [Command]
    public void BeginGame()
    {
        Debug.Log("Begin Game");
    }

    [ClientRpc]
    public void CardMoved(GameObject card, CardAreas area)
    {
        Debug.Log("Card moved");
    }

    [ClientRpc]
    public void TurnChange(int turnOrder, GameState state)
    {
        Debug.Log("Turn Change");
    }

    [ClientRpc]
    public void TrickFinished(int team0_score, int team1_score)
    {
        Debug.Log("Trick Finished");
    }

    [ClientRpc]
    public void TrumpChanged(CardColor color)
    {
        Debug.Log("Trump Changed");
    }
}
