using Mirror;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMan : NetworkBehaviour
{
    private GameMan gameManager;

    private GameObject myArea;
    private Quaternion rotation;

    [SyncVar(hook = nameof(OnBidChanged))]
    private int bid = 0;

    [SyncVar(hook = nameof(OnPassChanged))]
    private bool hasPassed = false;

    private readonly List<GameObject> cards = new List<GameObject>();

    [SyncVar(hook = nameof(CltOnPlayerPosUpdate))]
    public int playerPosition = -1;

    private int relativePosition = -1;

    public override void OnStartClient()
    {
        Debug.Log("Finding Game Manager");
        gameManager = GameObject.Find("GameManger").GetComponent<GameMan>();
        gameManager.CltRegisterPlayer(this);
        CltUpdateMyArea();
        base.OnStartClient();
    }

    [Server]
    public void SrvCardMoved(GameObject card, CardAreas area)
    {
        cards.Add(card);
        card.GetComponent<Card>().SrvSetArea(area);
        RpcCardMoved(card, area);
    }

    // Called when a card is moved to this player's hand
    [ClientRpc]
    public void RpcCardMoved(GameObject card, CardAreas area)
    {
        cards.Add(card);
        card.transform.SetParent(myArea.transform, false);
        card.transform.rotation = rotation;
    }

    [Server]
    public void SrvCardRemoved(GameObject card)
    {
        cards.Remove(card);
        RpcCardRemoved(card);
    }

    [ClientRpc]
    public void RpcCardRemoved(GameObject card)
    {
        cards.Remove(card);
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
        relativePosition = playerPosition - playerOwner;
        if (relativePosition < 0) {
            relativePosition += 4;
        }
        Debug.Log("Player Position: " + playerOwner + ", my position: " + playerPosition + ", relative position: " + relativePosition);

        switch (relativePosition)
        {
            case 0:
                myArea = gameManager.playerArea;
                rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 1:
                myArea = gameManager.enemyArea1;
                rotation = Quaternion.Euler(0, 0, 90);
                break;
            case 2:
                myArea = gameManager.enemyArea2;
                rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 3:
                myArea = gameManager.enemyArea3;
                rotation = Quaternion.Euler(0, 0, 270);
                break;
            default:
                Debug.Log("Invalid relative position " + relativePosition + "!");
                break;
        }

        foreach (GameObject card in cards) {
            card.transform.SetParent(myArea.transform, false);
            card.transform.rotation = rotation;
        }

    }

    [Client]
    public void CltOnBidMade(int bid)
    {
        gameManager.bidSelect.CanBid(false);
        CmdMakeBid(bid);
    }

    [Client]
    public void CltOnBidPass()
    {
        gameManager.bidSelect.CanBid(false);
        CmdPassBid();
    }

    [Command]
    public void CmdMakeBid(int bid)
    {
        this.bid = bid;
        gameManager.SrvNextBid(bid);
        gameManager.SrvNextTurn();
    }

    [Command]
    public void CmdPassBid()
    {
        hasPassed = true;
        gameManager.SrvNextTurn();
    }

    [ClientRpc]
    public void RpcStartBidding()
    {
        Debug.Log("Starting bidding");
        if (hasAuthority) {
            Debug.Log("Activating Bid menu");
            BidSelect bidSelect = gameManager.bidSelect;

            bidSelect.CanBid(gameManager.CltMyTurn());
            bidSelect.playerBidTxt.gameObject.SetActive(false);
            bidSelect.SetMinimumBid(70);
        } 
    }

    [Server]
    public void SrvStartBidding()
    {
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
        TextMeshProUGUI myBid = cltGetBidText();
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

    [Client]
    private TextMeshProUGUI cltGetBidText()
    {
        Debug.Log("Getting bid text");
        if (gameManager == null) {
            Debug.LogWarning("gameManager is null!");
            return null;
        }
        BidSelect bidSelect = gameManager.bidSelect;
        return relativePosition switch
        {
            0 => bidSelect.playerBidTxt,
            1 => bidSelect.enemyBid1BidTxt,
            2 => bidSelect.enemyBid2BidTxt,
            3 => bidSelect.enemyBid3BidTxt,
            _ => null,
        };
    }

    [Server]
    public void SrvOnMyTurn()
    {
        Debug.Log("Received On My Turn Message");
        RpcOnMyTurn();
    }

    [ClientRpc]
    void RpcOnMyTurn()
    {
        if (hasAuthority) {
            Debug.Log("It's My Turn!");
            if (gameManager.GetGameState() == GameState.BID) {
                BidSelect bidSelect = gameManager.bidSelect;
                if (hasPassed) {
                    CmdPassBid();
                } else if (gameManager.MaxBid() >= 120) {
                    CltOnBidPass();
                } else {
                    bidSelect.CanBid(true);
                }
            }
        }
    }

    [Client]
    void OnBidChanged(int oldBid, int newBid)
    {
        Debug.Log("Bid Changed");
        updateBid();
    }

    [Client]
    void OnPassChanged(bool oldBid, bool newBid)
    {
        Debug.Log("Pass Changed");
        updateBid();
        if (newBid && hasAuthority) {
            gameManager.bidSelect.ShowInterface(false);
        }
    }

    public int GetBid()
    {
        return bid;
    }

    public bool HasPassed()
    {
        return hasPassed;
    }

}
