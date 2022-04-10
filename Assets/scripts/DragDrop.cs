using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    public GameObject Canvas;

    private bool isDragging;
    private GameObject startParent;
    private Vector2 startPosition;
    private GameObject dropZone;
    private bool isOverDropZone;

    void Start()
    {
        Canvas = GameObject.Find("Main Canvas");
    }

    /// Check if dropping the card over the dropZone should result in playing the card
    private bool canPlay()
    {
        Card card = GetComponent<Card>();
        Debug.Log("hasAuthority: " + card.hasAuthority + ", isPlayable" + card.CltIsPlayable() + ", myTurn: " + card.gameManager.CltMyTurn());
        return card.hasAuthority && card.CltIsPlayable() && card.gameManager.CltMyTurn() && card.gameManager.GetGameState() == GameState.PLAY;
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        isOverDropZone = true;
        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision) 
    {
        isOverDropZone = false;
        dropZone = null;
    }

    public void beginDrag()
    {
        // You can only even try to drag a card if you have authority over that card
        if (GetComponent<Card>().hasAuthority) {
            isDragging = true;
            startParent = transform.parent.gameObject;
            startPosition = transform.position;
        }
    }

    public void endDrag()
    {
        if (isDragging) {
            isDragging = false;
            if (isOverDropZone && canPlay())
            {
                transform.SetParent(Canvas.transform);
                GetComponent<Card>().CmdPlay();
            }
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }
}
