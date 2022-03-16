using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMan : NetworkManager
{
    public GameMan gameMan;

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
        Debug.Log("Player connected to server");
        // TODO: Get or create the PlayerMan object for PlayerJoined
        // gameMan.PlayerJoined(conn);
    }
}
