using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{

    private bool isDragging = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void beginDrag() {
        isDragging = true;
    }

    public void endDrag() {
        isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging) {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }
}
