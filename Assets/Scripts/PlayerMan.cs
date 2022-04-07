using Mirror;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMan : NetworkBehaviour
{

    public BidSelect bidSelect = null;
    public TrumpSelect trumpSelect = null;
    private GameMan gameManager;

    private GameObject myArea;
    private TextMeshProUGUI myBid;
    private Quaternion rotation;

    [SyncVar(hook = nameof(OnBidChanged))]
    private int bid = 0;

    [SyncVar(hook = nameof(OnPassChanged))]
    private bool hasPassed = false;

    private readonly List<GameObject> cards = new List<GameObject>();

    [SyncVar(hook = nameof(CltOnPlayerPosUpdate))]
    public int playerPosition = -1;

    public override void OnStartClient()
    {
        gameManager = GameObject.Find("GameManger").GetComponent<GameMan>();
        gameManager.CltRegisterPlayer(this);
        if (hasAuthority) {
            bidSelect = gameManager.bidSelect;
            trumpSelect = gameManager.trumpSelect;

            // Register the bid callbacks
            bidSelect.AddBidCallback(OnBidMade);
            bidSelect.AddPassCallback(OnBidPass);
        }
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
                myArea = gameManager.playerArea;
                myBid = gameManager.player_bid;
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 1:
                myArea = gameManager.enemyArea1;
                myBid = gameManager.enemy0_bid;
                rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 2:
                myArea = gameManager.enemyArea2;
                myBid = gameManager.enemy1_bid;
                rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 3:
                myArea = gameManager.enemyArea3;
                myBid = gameManager.enemy2_bid;
                rotation = Quaternion.Euler(0, 0, 270);
                break;
            default:
                Debug.Log("Invalid relative position " + relativePosition + "!");
                break;
        }
    }

    [Client]
    public void OnBidMade(int bid)
    {
        bidSelect.CanBid(false);
        MakeBid(bid);
    }

    [Client]
    public void OnBidPass()
    {
        bidSelect.CanBid(false);
        PassBid();
    }

    [Command]
    public void MakeBid(int bid)
    {
        this.bid = bid;
        gameManager.SrvNextTurn();
    }

    [Command]
    public void PassBid()
    {
        hasPassed = true;
        gameManager.SrvNextTurn();
    }

    [ClientRpc]
    public void RpcStartBidding()
    {
        myBid.text = "...";
        if (hasAuthority) {
            if (bidSelect == null) {
                Debug.LogWarning("bidSelect is null even though I have authority");
                return;
            }
            bidSelect.gameObject.SetActive(true);
            bidSelect.CanBid(gameManager.CltMyTurn());
            myBid.gameObject.SetActive(false);
        } else {
            myBid.gameObject.SetActive(true);
        }
    }

    [Server]
    public void SrvStartBidding()
    {
        bid = 70;
        hasPassed = false;
        RpcStartBidding();
    }

    [Client]
    void CltOnPlayerPosUpdate(int oldPos, int newPos)
    {
        CltUpdateMyArea();
    }

    [Client]
    private void updateBid()
    {
        if (myBid == null) {
            Debug.Log("My bid is still null...");
            return;
        }
        string text = "" + bid;
        if (hasPassed) {
            text += " (passed)";
        }
        myBid.text = text;
    }

    [Server]
    public void SrvOnMyTurn()
    {
        RpcOnMyTurn();
    }

    [ClientRpc]
    void RpcOnMyTurn()
    {
        if (hasAuthority) {
            if (gameManager.GetGameState() == GameState.BID) {
                bidSelect.CanBid(true);
            }
        }
    }

    [Client]
    void OnBidChanged(int oldBid, int newBid)
    {
        updateBid();
    }

    [Client]
    void OnPassChanged(bool oldBid, bool newBid)
    {
        updateBid();
        if (newBid && hasAuthority) {
            bidSelect.gameObject.SetActive(false);
            myBid.gameObject.SetActive(true);
        }
    }

}
