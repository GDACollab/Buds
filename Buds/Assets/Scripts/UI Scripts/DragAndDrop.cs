using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * DragAndDrop.cs
 * Written by Santiago Ponce
 * Email: saponce@ucsc.edu
 * 
 * This code is used to give objects drag-and-drop functionality with the mouse.
 * Primarily intended for moving flowers from flowerpot to flowerpot in the gardening minigame.
 */

public class DragAndDrop : MonoBehaviour
{
    public bool toggle = true; //Switch between toggle and hold modes
    bool dragging;
    Rect screenRect = new Rect(0, 0, Screen.width, Screen.height); //This is used to prevent the block from moving while the mouse is offscreen

    GameObject[] flowerpots;
    GameObject[] flowers; //Used for swapping if a flower is already occupying a pot
    bool snapped;
    public float maxSnapDistance = 1.5f; //The maximum distance a flower can be from a pot before it will snap. Set to 0 if snap distance is infinite;

    Vector2 oldPosition;

    // Start is called before the first frame update
    void Start()
    {
        dragging = false;
        snapped = true;
        oldPosition = this.transform.position; //Stores the last location of this object before it was moved by the mouse.

        if(maxSnapDistance == 0)
        {
            maxSnapDistance = Mathf.Infinity;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 offset = new Vector2(-0.5f, 0.5f);

        if (dragging && screenRect.Contains(Input.mousePosition))
        {
            //Mouse position on the screen uses different coordinates, needs to be corrected
            gameObject.transform.position = mousePos + offset;
        }
    }

    private void OnMouseDown()
    {
        if (!dragging)
        {
            dragging = true;
            snapped = false;
        }
        else if (dragging && toggle)
        {
            dragging = false;
            snapped = SnapToPot();
        }
    }

    private void OnMouseUp()
    {
        if(!toggle)
        {
            dragging = false;
            snapped = SnapToPot();
        } 
    }

    //Snaps the object to the nearest flowerpot
    private bool SnapToPot()
    {
        flowerpots = GameObject.FindGameObjectsWithTag("Flowerpot");
        GameObject moveTo = null;

        float difference = Mathf.Infinity;

        if(flowerpots.Length <= 0)
        {
            return false;
        }

        foreach(GameObject flowerpot in flowerpots)
        {
            float test = Vector2.Distance(flowerpot.transform.position, gameObject.transform.position);

            //Vector2 test = flowerpot.transform.position - gameObject.transform.position;
            if(test < difference && test <= maxSnapDistance)
            {
                moveTo = flowerpot;
                difference = test;
            }
            
        }

        if (moveTo != null)
        {
            gameObject.transform.position = moveTo.transform.position;
        }

        //Check if overlapping another flower
        //Swapping will only occur if both objects have the tag "flower"
        //Admittedly this probably isn't the best way to do swapping but for the purposes of this game it likely won't matter
        flowers = GameObject.FindGameObjectsWithTag("Flower");
        foreach(GameObject flower in flowers)
        {
            if(flower.transform.position == this.transform.position && flower != this.gameObject)
            {
                flower.transform.position = this.oldPosition;
                DragAndDrop flowerScript = flower.GetComponent<DragAndDrop>();
                flowerScript.oldPosition = flower.transform.position;
            }
        }
        
        oldPosition = this.transform.position;

        return true;
    }
}

//Simple explanation of single variables
/*
 * if(main == NULL)
 * {
 *      this == Main
 * }
 * else
 *      this.destroy
 */
