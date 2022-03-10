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

public class GameManager : NetworkBehaviour
{
    private readonly List<GameObject> deck_cards = new List<GameObject>();
    private readonly List<GameObject> player0_cards = new List<GameObject>();
    private readonly List<GameObject> player1_cards = new List<GameObject>();
    private readonly List<GameObject> player2_cards = new List<GameObject>();
    private readonly List<GameObject> player3_cards = new List<GameObject>();
    private readonly List<GameObject> playarea_cards = new List<GameObject>();
    private readonly List<GameObject> kitty_cards = new List<GameObject>();

    private readonly List<GameObject> player0_tricks = new List<GameObject>();
    private readonly List<GameObject> player1_tricks = new List<GameObject>();
    private readonly List<GameObject> player2_tricks = new List<GameObject>();
    private readonly List<GameObject> player3_tricks = new List<GameObject>();

    private int team0_score = 0;
    private int team1_score = 0;
    private int player0_bid = 0;
    private int player1_bid = 0;
    private int player2_bid = 0;
    private int player3_bid = 0;
    private int current_turn = 0;
    private GameState game_state = GameState.SETUP;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
