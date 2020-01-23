using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    public bool toggle = true; //Switch between toggle and hold modes
    bool dragging;
    Rect screenRect = new Rect(0, 0, Screen.width, Screen.height); //This is used to prevent the block from moving while the mouse is offscreen

    // Start is called before the first frame update
    void Start()
    {
        dragging = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (dragging && screenRect.Contains(Input.mousePosition))
        {
            //Mouse position on the screen uses different coordinates, needs to be corrected
            gameObject.transform.position = mousePos;
        }
    }

    private void OnMouseDown()
    {
        if (!dragging)
            dragging = true;
        else if(dragging && toggle)
           dragging = false;
    }

    private void OnMouseUp()
    {
        if(!toggle)
            dragging = false;
    }

}
