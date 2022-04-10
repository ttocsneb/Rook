using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    DROPAREA,
    KITTY,
    DECK,
}

public class GameMan : NetworkBehaviour
{
    public BidSelect bidSelect;
    public TrumpSelect trumpSelect;

    public GameObject enemyArea1;
    public GameObject enemyArea2;
    public GameObject enemyArea3;
    public GameObject playerArea;
    
    public GameObject dropArea;
    public GameObject kittyArea;
    public GameObject deckArea;

    public GameObject turnIndicator;

    public GameObject cardPrefab;

    [SyncVar(hook = nameof(onPlayerTurn))]
    private int current_turn = -1;
    [SyncVar]
    private GameState game_state = GameState.SETUP;

    private readonly List<PlayerMan> players = new List<PlayerMan>();
    private readonly List<PlayerMan> localPlayers = new List<PlayerMan>();
    private PlayerMan localPlayer = null;

    private readonly List<GameObject> allCards = new List<GameObject>();
    private readonly List<GameObject> deck = new List<GameObject>();
    private readonly List<GameObject> drop = new List<GameObject>();
    private readonly List<GameObject> kitty = new List<GameObject>();

    private int playerOwner = -1;

    private bool hasDealt = false;
    private CardColor trickColor;
    [SyncVar]
    private CardColor trumpColor;


    [SyncVar(hook = nameof(CltOnMaxBid))]
    private int maxBid = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        bidSelect.AddBidCallback(CltOnBidMade);
        bidSelect.AddPassCallback(CltOnBidPass);
        trumpSelect.AddCallback(CltOnTrumpUpdated);
    }

    [Server]
    public void SrvBeginGame() 
    {
        game_state = GameState.SETUP;
        srvSpawnCards();
        srvDealAllCards();
        SrvChangeGameState(GameState.BID);
        current_turn = 0;
    }

    private int dealPosition = 0;

    [Server]
    private void srvDealCards(CardAreas[] areas, int count = -1)
    {
        // We use allCards so that we can deal from any previous state 
        int numCards = allCards.Count - dealPosition;

        int toDeal = count < 0 ? numCards / areas.Length : count;
        int expected = numCards - (toDeal * areas.Length);
        for (int i = 0; i < toDeal; i++) {
            foreach (CardAreas area in areas) {
                GameObject nextCard = allCards[dealPosition];
                dealPosition += 1;
                SrvMoveCard(nextCard, area);
            }
        }
        numCards = allCards.Count - dealPosition;
        if (numCards != expected) {
            Debug.LogError("There are not the same number of expected cards! expected " + expected + " cards, but found " + deck.Count + " cards.");
        }
    }

    /// Deal all cards in the deck
    ///
    /// This will first deal 5 cards to each hand including the kitty.
    /// Then deal the rest of the cards in the deck to each player
    [Server]
    private void srvDealAllCards() {
        // Shuffle
        int n = allCards.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n);
            GameObject tmp = allCards[k];
            allCards[k] = allCards[n];
            allCards[n] = tmp;
        }

        dealPosition = 0;
        CardAreas[] allDests = new CardAreas[] {CardAreas.PLAYER0, CardAreas.PLAYER1, CardAreas.PLAYER2, CardAreas.PLAYER3, CardAreas.KITTY};
        CardAreas[] players = new CardAreas[] {CardAreas.PLAYER0, CardAreas.PLAYER1, CardAreas.PLAYER2, CardAreas.PLAYER3};
        srvDealCards(allDests, 5);
        srvDealCards(players);
    }

    /// A command to allow any client to deal cards to everyone
    [Command(requiresAuthority = false)]
    public void CmdDealCards()
    {
        if (players.Count == 4 && !hasDealt) {
            srvDealAllCards();
            hasDealt = true;
            current_turn = 0;
        }
    }

    /// Move to the next turn
    [Server]
    public void SrvNextTurn()
    {
        if (game_state == GameState.BID) {
            // Check if the winning bidder has been found
            int passed_count = 0;
            int winning_player = 0;
            int bid = 0;
            foreach (PlayerMan player in players) {
                if (player.GetBid() > bid) {
                    winning_player = player.playerPosition;
                    bid = player.GetBid();
                }
                if (player.HasPassed()) {
                    passed_count += 1;
                }
            }
            if (passed_count >= 3 || bid >= 120) {
                RpcStopBidding();
                SrvChangeGameState(GameState.TRUMP_SELECT);
                current_turn = winning_player;
                return;
            }
        }

        int turn = current_turn + 1;
        if (turn >= 4) {
            turn = 0;
        }
        current_turn = turn;
        players[current_turn].SrvOnMyTurn();
    }

    /// A new player has joined the game
    ///
    /// This should be called by NetMan
    [Server]
    public void SrvPlayerJoined(NetworkConnection conn)
    {
        int playerPosition = players.Count;
        PlayerMan player = conn.identity.GetComponent<PlayerMan>();
        player.playerPosition = playerPosition; // This should automatically sync with all clients

        TgtSetPlayerOwner(conn, playerPosition);

        players.Add(player);
        if (players.Count == 4) {
            SrvBeginGame();
        }
    }

    /// A card needs to be moved to a new destination 
    ///
    /// @param card card to move
    /// @param destination the area to move the card to
    [Server]
    public void SrvMoveCard(GameObject card, CardAreas destination) 
    {
        srvRemoveCard(card);

        Card c = card.GetComponent<Card>();
        NetworkIdentity ni = card.GetComponent<NetworkIdentity>();
        // Remove authority just in case the card had authority from before
        ni.RemoveClientAuthority();
        PlayerMan player;
        switch (destination)
        {
            case CardAreas.DROPAREA:
                c.SrvSetArea(CardAreas.DROPAREA);
                c.RpcSetVisible(true);
                drop.Add(card);
                //if this is the first card played, its color is the trick color
                if (dropArea.transform.childCount == 0) 
                {
                    RpcUpdateClientHand(card);
                }
                RpcCardMoved(card, destination);
                return;
            case CardAreas.KITTY:
                c.SrvSetArea(CardAreas.KITTY);
                c.RpcSetVisible(false);
                kitty.Add(card);
                RpcCardMoved(card, destination);
                return;
            case CardAreas.DECK:
                // Remove client authority and make sure the card is not visible
                c.SrvSetArea(CardAreas.DECK);
                c.RpcSetVisible(false);
                deck.Add(card);
                RpcCardMoved(card, destination);
                return;
            case CardAreas.PLAYER0:
                player = players[0];
                break;
            case CardAreas.PLAYER1:
                player = players[1];
                break;
            case CardAreas.PLAYER2:
                player = players[2];
                break;
            case CardAreas.PLAYER3:
                player = players[3];
                break;
            default:
                Debug.LogError("Invalid Destination");
                return;
        }
        ni.AssignClientAuthority(player.connectionToClient);
        player.SrvCardMoved(card, destination);
    }

    [Server]
    private void srvRemoveCard(GameObject card)
    {
        Card c = card.GetComponent<Card>();
        CardAreas oldArea = c.getArea();

        PlayerMan player = null;
        switch (oldArea)
        {
            case CardAreas.DECK:
                deck.Remove(card);
                RpcCardRemoved(card, CardAreas.DECK);
                return;
            case CardAreas.DROPAREA:
                drop.Remove(card);
                RpcCardRemoved(card, CardAreas.DROPAREA);
                return;
            case CardAreas.KITTY:
                kitty.Remove(card);
                RpcCardRemoved(card, CardAreas.KITTY);
                return;
            case CardAreas.PLAYER0:
                player = players[0];
                break;
            case CardAreas.PLAYER1:
                player = players[1];
                break;
            case CardAreas.PLAYER2:
                player = players[2];
                break;
            case CardAreas.PLAYER3:
                player = players[3];
                break;
        }
        // remove card from player inventory
        if (player != null) {
            player.SrvCardRemoved(card);
        }

    }

    /// Called when a card is moved to either the kitty deck or the play area
    /// 
    /// Note: If a card moved to a player's hand, then that player should get their
    ///       CardMoved method called instead
    [ClientRpc]
    public void RpcCardMoved(GameObject card, CardAreas destination) 
    {
        // move the card physically to its destination
        switch (destination) {
            case CardAreas.DROPAREA:
                card.transform.SetParent(dropArea.transform, false);
                drop.Add(card);
                break;
            case CardAreas.DECK:
                card.transform.SetParent(deckArea.transform, false);
                deck.Add(card);
                break;
            case CardAreas.KITTY:
                kitty.Add(card);
                card.transform.SetParent(kittyArea.transform, false);
                if (playerOwner == current_turn && game_state == GameState.TRUMP_SELECT) {
                    if (trumpSelect.DiscardOne()) {
                        kittyArea.GetComponent<BoxCollider2D>().enabled = false;
                    }
                }
                break;
            default:
                Debug.LogError("Expected to move card to invalid position");
                break;
        }
        card.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    [ClientRpc]
    public void RpcCardRemoved(GameObject card, CardAreas destination)
    {
        switch (destination) {
            case CardAreas.DROPAREA:
                drop.Remove(card);
                break;
            case CardAreas.DECK:
                deck.Remove(card);
                break;
            case CardAreas.KITTY:
                kitty.Remove(card);
                break;
            default:
                Debug.LogError("Expected to move card to invalid position");
                break;
        }
    }

    [ClientRpc]
    public void RpcUpdateClientHand(GameObject card)
    {
        trickColor = card.GetComponent<Card>().GetColor();
        Debug.Log("trick color set to " + trickColor);
        bool hasLeadColor = false;
        // playerArea.GetComponent<Image>().color = new Color32(255,255,225,100);
        for (int i = 0; i < playerArea.transform.childCount; i++)
        {
            Card handCard = playerArea.transform.GetChild(i).gameObject.GetComponent(typeof(Card)) as Card;
            CardColor handColor = handCard.GetColorAlways();
            if (handColor == trickColor || handColor == trumpColor || handColor == CardColor.ROOK)
            {
                hasLeadColor = true;
                continue;
            }
        }
        if (hasLeadColor)
        {
            Debug.Log("has trick color");
            for (int i = 0; i < playerArea.transform.childCount; i++)
            {
                Card handCard = playerArea.transform.GetChild(i).gameObject.GetComponent(typeof(Card)) as Card;
                handCard.CltSetTrickColor(trickColor);
            }

        } else
        {
            Debug.Log("all cards playable");
            for (int i = 0; i < playerArea.transform.childCount; i++)
            {
                Card handCard = playerArea.transform.GetChild(i).gameObject.GetComponent(typeof(Card)) as Card;
                handCard.SetPlayable(true);
            }
        }
    }

    /// Called at the beginning of the game to help match clients with player ids
    ///
    /// This will set playerOwner to the player id of the client.
    [TargetRpc]
    public void TgtSetPlayerOwner(NetworkConnection conn, int playerOwner) {
        this.playerOwner = playerOwner;
        foreach (PlayerMan player in localPlayers) {
            player.CltUpdateMyArea();
        }
    }

    /// Called on the client when a new player has joined the game. 
    ///
    /// For some reason SyncVars does not support Lists of GameObjects, so we need to implement
    /// a manual system for the local playerlist object
    [Client]
    public void CltRegisterPlayer(PlayerMan playerMan) {
        localPlayers.Add(playerMan);
        if (playerMan.hasAuthority) {
            localPlayer = playerMan;
        }
    }

    /// Get the player id of the current client
    [Client]
    public int CltGetPlayerOwner()
    {
        return playerOwner;
    }

    /// Check if it is the current client's turn
    [Client]
    public bool CltMyTurn()
    {
        return current_turn == playerOwner;
    }

    /// Show or hide the turn indicator
    [Client]
    private void cltUpdateTurnIndicator()
    {
        Text text = turnIndicator.GetComponent<Text>();
        if (CltMyTurn()) {
            text.enabled = true;
        } else {
            text.enabled = false;
        }
    }

    /// Spawn a new card
    ///
    /// This is an internal function that spawns a singular card into the game. You should instead use SrvSpawnCards()
    ///
    /// @param color card color
    /// @param number card number
    [Server]
    private GameObject srvSpawnCard(CardColor color, int number) {
        GameObject obj = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.Euler(x: 0, y:0, z:0));
        Card c = obj.GetComponent<Card>();
        c.gameManager = this;
        NetworkServer.Spawn(obj);
        c.SrvSetCard(color, number);
        SrvMoveCard(obj, CardAreas.DECK);
        allCards.Add(obj);
        return obj;
    }

    /// Spawn an entire house of cards
    ///
    /// This is an internal function that spawns all cards of a house into the game. You should instead use SrvSpawnCards()
    ///
    /// @param color card color to spawn
    [Server]
    private List<GameObject> srvSpawnHouse(CardColor color) {
        List<GameObject> cards = new List<GameObject>();
        for (int i = 1; i <= 14; i++) {
            cards.Add(srvSpawnCard(color, i));
        }
        return cards;
    }

    /// Spawn all cards into the game
    ///
    /// This will spawn one of each card of the deck into the game.
    [Server]
    private void srvSpawnCards() {
        List<GameObject> cards = new List<GameObject>();

        cards.AddRange(srvSpawnHouse(CardColor.GREEN));
        cards.AddRange(srvSpawnHouse(CardColor.YELLOW));
        cards.AddRange(srvSpawnHouse(CardColor.RED));
        cards.AddRange(srvSpawnHouse(CardColor.BLACK));
        cards.Add(srvSpawnCard(CardColor.ROOK, 1));

        foreach (GameObject obj in cards) {
            Card card = obj.GetComponent<Card>();
            card.SrvSetArea(CardAreas.DECK);
        }
        deck.AddRange(cards);

    }

    public GameState GetGameState()
    {
        return game_state;
    }

    [Client]
    void onPlayerTurn(int oldValue, int newValue) 
    {
        cltUpdateTurnIndicator();
    }

    [Server]
    void SrvChangeGameState(GameState newValue)
    {
        game_state = newValue;
        switch (newValue) {
            case GameState.SETUP:
                Debug.Log("Setup Mode");
                break;
            case GameState.BID:
                Debug.Log("Bid Mode");
                RpcStartBidding();
                foreach (PlayerMan player in players) {
                    player.SrvStartBidding();
                }
                break;
            case GameState.TRUMP_SELECT:
                Debug.Log("Trump Select Mode");
                // Move cards from kitty into the bid winner's hand
                CardAreas dest = GetPlayerArea(current_turn);
                List<GameObject> fromKitty = new List<GameObject>();
                fromKitty.AddRange(kitty);
                foreach (GameObject card in fromKitty) {
                    SrvMoveCard(card, dest);
                }
                // Temporarily give authority to the winning bidder while they choose the trump color
                GetComponent<NetworkIdentity>().AssignClientAuthority(players[current_turn].connectionToClient);
                TgtOnStartTrumpSelect(players[current_turn].connectionToClient);
                break;
            case GameState.PLAY:
                Debug.Log("Play Mode");
                GetComponent<NetworkIdentity>().RemoveClientAuthority();
                RpcBeginPlay();
                break;
        }
    }
    [ClientRpc]
    void RpcStartBidding()
    {
        bidSelect.enemyBid1BidTxt.text = "...";
        bidSelect.enemyBid2BidTxt.text = "...";
        bidSelect.enemyBid3BidTxt.text = "...";
        bidSelect.gameObject.SetActive(true);
        bidSelect.ShowInterface(true);
    }

    [ClientRpc]
    void RpcStopBidding()
    {
        bidSelect.gameObject.SetActive(false);
    }

    [Client]
    public void CltOnBidMade(int bid)
    {
        // Called from bidSelect
        localPlayer.CltOnBidMade(bid);
    }

    [Client]
    public void CltOnBidPass()
    {
        // Called from bidPass
        localPlayer.CltOnBidPass();
    }

    [Server]
    public void SrvNextBid(int bid)
    {
        // Keep track of the minimum bid for each following player
        // This is the maximum bid made so far
        if (bid > maxBid) {
            maxBid = bid;
        }
    }

    public int MaxBid()
    {
        return maxBid;
    }

    [Client]
    public void CltOnMaxBid(int oldValue, int newValue)
    {
        // Make sure that the minimum bid is more than the current maximum
        bidSelect.SetMinimumBid(newValue + 5);
    }

    [TargetRpc]
    public void TgtOnStartTrumpSelect(NetworkConnection conn)
    {
        // Make sure that the kitty area collider is enabled so that the player can discard their cards
        kittyArea.GetComponent<BoxCollider2D>().enabled = true;
        trumpSelect.gameObject.SetActive(true);
    }

    [TargetRpc]
    public void TgtOnStopTrumpSelect(NetworkConnection conn)
    {
        kittyArea.GetComponent<BoxCollider2D>().enabled = false;
        trumpSelect.gameObject.SetActive(false);
    }

    [Client]
    public void CltOnTrumpUpdated(CardColor trump, bool isReady)
    {
        // Called from trumpSelect
        if (isReady) {
            CmdOnTrumpSelectReady(trump);
            trumpSelect.gameObject.SetActive(false);
        } else {
            localPlayer.CltSetTrumpColor(trump);
        }
    }

    [Command]
    public void CmdOnTrumpSelectReady(CardColor trump)
    {
        // Once the trump has been selected and the player is ready, then the game state can be changed to PLAY
        SrvSetTrumpColor(trump);
        SrvChangeGameState(GameState.PLAY);
    }

    [Server]
    public void SrvSetTrumpColor(CardColor trump)
    {
        trumpColor = trump;
        foreach (GameObject c in allCards) {
            Card card = c.GetComponent<Card>();
            card.RpcSetTrump(trumpColor);
        }
    }

    public CardAreas GetPlayerArea(int position) {
        return position switch
        {
            0 => CardAreas.PLAYER0,
            1 => CardAreas.PLAYER1,
            2 => CardAreas.PLAYER2,
            3 => CardAreas.PLAYER3,
            _ => CardAreas.DECK,
        };
    }

    [ClientRpc]
    public void RpcBeginPlay()
    {
        dropArea.GetComponent<BoxCollider2D>().enabled = true;
    }
}
