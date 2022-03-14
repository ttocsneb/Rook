using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    SETUP,
    BID,
    TRUMP_SELECT,
    PLAY,
}

public enum CardAreas
{
    PLAYER0,
    PLAYER1,
    PLAYER2,
    PLAYER3,
    KITTY,
    DECK,
}

public class GameManager : NetworkManager
{
    public GameObject enemyArea1;
    public GameObject enemyArea2;
    public GameObject enemyArea3;
    public GameObject playerArea;
    
    public GameObject dropArea;
    public GameObject kittyArea;

    private int team0_score = 0;
    private int team1_score = 0;
    private readonly int[] player_bids = new int[] {
        0,
        0,
        0,
        0
    };
    private int current_turn = 0;
    private GameState game_state = GameState.SETUP;

    private readonly List<NetworkConnection> players = new List<NetworkConnection>();

    [Server]
    public void BeginGame() {
        
    }

    public override void OnStartServer()
    {
        Debug.Log("Server Started");
    }

    public override void OnStopServer()
    {
        Debug.Log("Server Stopped");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("Connected to server");
        players.Add(conn);
        if (players.Count == 4) {
            BeginGame();
        }
    }
}
