using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject DropZone;

    private bool isDragging;
    private GameObject startParent;
    private Vector2 startPosition;
    private GameObject dropZone;
    private bool isOverDropZone;

    void Start()
    {
        Canvas = GameObject.Find("Main Canvas");
        DropZone = GameObject.Find("DropZone");
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
        isDragging = true;
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
    }

    public void endDrag()
    {
        isDragging = false;
        if (isOverDropZone)
        {
            transform.SetParent(dropZone.transform, false);
            transform.rotation = Quaternion.Euler(x: 0, y:0,z:0);
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(Canvas.transform, true);
        }
    }
}
