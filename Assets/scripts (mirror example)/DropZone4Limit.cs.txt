using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZone4Limit : MonoBehaviour
{
    private int ALLOWED_CHILDREN = 4;

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount >= ALLOWED_CHILDREN){
            transform.GetComponent<BoxCollider2D>().enabled = false;
        }
        else{
            transform.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
