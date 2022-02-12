using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public GameObject Card1;
    public GameObject Card2;
    public GameObject PlayerArea;
    public GameObject EnemyArea_1;
    public GameObject EnemyArea_2;
    public GameObject EnemyArea_3;

    void Start()
    {

    }

    public void onClick()
    {
        for (int i = 0; i < 5; i++) {
            GameObject playerCard = Instantiate(Card1, new Vector2(0,0), Quaternion.identity);
            playerCard.transform.SetParent(PlayerArea.transform, false);
            GameObject enemyCard = Instantiate(Card1, new Vector2(0,0), Quaternion.Euler(x: 0, y:0,z:90));
            enemyCard.transform.SetParent(EnemyArea_1.transform, false);
            GameObject enemyCard2 = Instantiate(Card2, new Vector2(0,0), Quaternion.Euler(x: 0, y:0,z:180));
            enemyCard2.transform.SetParent(EnemyArea_2.transform, false);
            GameObject enemyCard3 = Instantiate(Card1, new Vector2(0,0), Quaternion.Euler(x: 0, y:0,z:270));
            enemyCard3.transform.SetParent(EnemyArea_3.transform, false);
        }
    }
}
