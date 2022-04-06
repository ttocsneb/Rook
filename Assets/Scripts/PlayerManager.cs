using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public GameObject card;
    public GameObject playerArea;
    public GameObject enemyArea1;
    public GameObject enemyArea2;
    public GameObject enemyArea3;
    public GameObject dropArea;
    public GameObject kittyArea;

    public readonly SyncList<PlayerManager> players = new SyncList<PlayerManager>();

    public void OnServerConnect(NetworkConnection conn){
        Debug.Log("Player connected to server");
        NetworkIdentity networkIdentity = conn.identity;
        PlayerManager playerManager = networkIdentity.GetComponent<PlayerManager>();
        players.Add(playerManager);
        Debug.Log("Player is munber " + players.IndexOf(playerManager));

        if(players.Count == 4){
            //start game
        }
    }

    public override void OnStartClient(){
        base.OnStartClient();
    }
}
