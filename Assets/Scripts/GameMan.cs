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
    PLAYAREA,
}

public class GameMan : NetworkBehaviour
{

    public GameObject enemyArea1;
    public GameObject enemyArea2;
    public GameObject enemyArea3;
    public GameObject playerArea;
    
    public GameObject dropArea;
    public GameObject kittyArea;
    public GameObject deckArea;

    public GameObject cardPrefab;

    private int current_turn = 0;
    private GameState game_state = GameState.SETUP;

    private List<PlayerMan> players = new List<PlayerMan>();

    [Server]
    public void BeginGame() 
    {
        game_state = GameState.SETUP;
        // Spawn the cards
        List<GameObject> cards = spawnCards();
        // Shuffle the cards
        int n = cards.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n);
            GameObject tmp = cards[k];
            cards[k] = cards[n];
            cards[n] = tmp;
        }
        // Deal the cards
        dealCards(cards, new CardAreas[] {CardAreas.PLAYER0, CardAreas.PLAYER1, CardAreas.PLAYER2, CardAreas.PLAYER3, CardAreas.KITTY}, 5);
        dealCards(cards, new CardAreas[] {CardAreas.PLAYER0, CardAreas.PLAYER1, CardAreas.PLAYER2, CardAreas.PLAYER3}, 8);
    }

    [Server]
    private void dealCards(List<GameObject> cards, CardAreas[] areas, int count)
    {
        for (int i = 0; i < count; i++) {
            foreach (CardAreas area in areas) {
                GameObject nextCard = cards[cards.Count - 1];
                cards.RemoveAt(cards.Count - 1);
                moveCard(nextCard, area);
            }
        }
    }

    [Server]
    public void PlayerJoined(NetworkConnection conn)
    {
        PlayerMan player = conn.identity.GetComponent<PlayerMan>();
        player.enemyArea1 = enemyArea1;
        player.enemyArea2 = enemyArea2;
        player.enemyArea3 = enemyArea3;
        player.playerArea = playerArea;
        player.dropArea = dropArea;
        player.kittyArea = kittyArea;
        player.gameManager = this;
        player.playerPosition = players.Count;

        players.Add(player);
        if (players.Count == 4) {
            BeginGame();
        }
    }
    
    [Server]
    private void moveCard(GameObject card, CardAreas destination) 
    {
        // TODO: Let the card know where it belongs

        // Let the player know where the card belongs
        switch (destination)
        {
            case CardAreas.KITTY:
            case CardAreas.PLAYAREA:
                RpcCardMoved(card, destination);
                break;
            case CardAreas.PLAYER0:
                players[0].RpcCardMoved(card, destination);
                break;
            case CardAreas.PLAYER1:
                players[1].RpcCardMoved(card, destination);
                break;
            case CardAreas.PLAYER2:
                players[2].RpcCardMoved(card, destination);
                break;
            case CardAreas.PLAYER3:
                players[3].RpcCardMoved(card, destination);
                break;
        }
    }

    // Called when a card is moved to either the kitty deck or the play area
    // 
    // If a card moved to a player's hand, then that player should get their
    // CardMoved method called
    [ClientRpc]
    public void RpcCardMoved(GameObject card, CardAreas destination) 
    {
        // TODO: add the card to the internal card list
        // TODO: move the card physically to its destination
    }

    [Server]
    private GameObject spawnCard(CardColor color, int number) {
        GameObject obj = Instantiate(cardPrefab, new Vector2(0, 0), Quaternion.Euler(x: 0, y:0, z:0));
        Card c = obj.GetComponent<Card>();
        c.SetCard(color, number);
        return obj;
    }

    [Server]
    private List<GameObject> spawnHouse(CardColor color) {
        List<GameObject> cards = new List<GameObject>();
        for (int i = 1; i <= 14; i++) {
            cards.Add(spawnCard(color, i));
        }
        return cards;
    }

    [Server]
    private List<GameObject> spawnCards() {
        List<GameObject> cards = new List<GameObject>();
        cards.AddRange(spawnHouse(CardColor.GREEN));
        cards.AddRange(spawnHouse(CardColor.YELLOW));
        cards.AddRange(spawnHouse(CardColor.RED));
        cards.AddRange(spawnHouse(CardColor.BLACK));
        cards.Add(spawnCard(CardColor.ROOK, 1));
        return cards;
    }
}
