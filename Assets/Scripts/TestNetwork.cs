using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TestNetwork : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField] private Vector2 movement = new Vector2();

    [Client]
    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) {return;}

        if (!Input.GetKeyDown(KeyCode.Space)) {return;}

        // transform.Translate(movement);

        CmdMove();
    }

    [Command]
    private void CmdMove() {
        RpcMove();
    }

    [ClientRpc]
    private void RpcMove() {
        transform.Translate(movement);
    }
}
