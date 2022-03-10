using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public GameObject Card1;
    public GameObject Card2;
    public GameObject PlayerArea;
    public GameObject EnemyArea_1;
    public GameObject EnemyArea_2;
    public GameObject EnemyArea_3;
    public GameObject DropArea;

    List<GameObject> cards = new List<GameObject>();

    public override void OnStartClient(){
        base.OnStartClient();

        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea_1 = GameObject.Find("EnemyArea_1");
        EnemyArea_2 = GameObject.Find("EnemyArea_2");
        EnemyArea_3 = GameObject.Find("EnemyArea_3");
        DropArea = GameObject.Find("DropArea");
    }

    [Server]
    public override void OnStartServer(){
        base.OnStartServer();

        cards.Add(Card1);
        cards.Add(Card2);
        Debug.Log(cards);

        //an association of players connections + areas assembled here?
        //clients assigned a cardinal direction when they join, 
    }

    [Command]
    public void CmdDealCards()
    {
        //foreach(KeyValuePair<int, NetworkConnectionToClient> entry in NetworkServer.connections){
            for (int i = 0; i < 13; i++) {
                GameObject playerCard = Instantiate(cards[0], new Vector2(0,0), Quaternion.identity);
                NetworkServer.Spawn(playerCard, connectionToClient);
                RpcShowCard(playerCard, "dealt");
            




            /*GameObject enemyCard = Instantiate(cards[Random.range(0, cards.Count)], new Vector2(0,0), Quaternion.Euler(x: 0, y:0,z:90));
            enemyCard.transform.SetParent(EnemyArea_1.transform, false);
            GameObject enemyCard2 = Instantiate(cards[Random.range(0, cards.Count)], new Vector2(0,0), Quaternion.Euler(x: 0, y:0,z:180));
            enemyCard2.transform.SetParent(EnemyArea_2.transform, false);
            GameObject enemyCard3 = Instantiate(cards[Random.range(0, cards.Count)], new Vector2(0,0), Quaternion.Euler(x: 0, y:0,z:270));
            enemyCard3.transform.SetParent(EnemyArea_3.transform, false);*/
            }
        //}
        
    }

    [ClientRpc]
    void RpcShowCard(GameObject card, string type){
        if(type == "dealt"){
            if(hasAuthority){
                card.transform.SetParent(PlayerArea.transform, false);
            }
            
        }
        else if(type == "played"){}

    }
}
