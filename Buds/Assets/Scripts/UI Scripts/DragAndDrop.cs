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
    public bool dropAndHold;
    bool finished;
    bool Dragging {
        get { return thisDragging; }
        set {
            if (locked == thisDragging) {
                locked = value;
                thisDragging = value;
            }
                
        }
    }
    bool thisDragging;
    static bool locked;

    Rect screenRect = new Rect(0, 0, Screen.width, Screen.height); //This is used to prevent the block from moving while the mouse is offscreen

    GameObject[] flowerpots;
    GameObject[] flowers; //Used for swapping if a flower is already occupying a pot
    bool snapped;
    public float maxSnapDistance = 1.5f; //The maximum distance a flower can be from a pot before it will snap. Set to 0 if snap distance is infinite;

    Vector2 oldPosition;

    SpriteRenderer[] spriteRenderers;
    int[] initialSortingOrders;

    IDraggable itemBeingDragged;
    GameObject lastMoveTo;

    // Start is called before the first frame update
    void Start()
    {
        Dragging = false;
        snapped = true;
        oldPosition = this.transform.position; //Stores the last location of this object before it was moved by the mouse.

        if(maxSnapDistance == 0)
        {
            maxSnapDistance = Mathf.Infinity;
        }

        SnapToPot();

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        initialSortingOrders = new int[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            initialSortingOrders[i] = spriteRenderers[i].sortingOrder;

        itemBeingDragged = gameObject.GetComponent<IDraggable>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 offset = new Vector2(-0.5f, 0.5f);

        if (Dragging && screenRect.Contains(Input.mousePosition))
        {
            //Mouse position on the screen uses different coordinates, needs to be corrected
            gameObject.transform.position = mousePos + offset;
        }
    }

    private void OnMouseDown()
    {
        if (!Dragging) {
            Dragging = true;
            snapped = false;
            if (itemBeingDragged != null) {
                itemBeingDragged.Lift(from: lastMoveTo);
            }
        }
        else if (Dragging && toggle) {
            snapped = SnapToPot();
            Dragging = false;

        }

        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteRenderers[i].sortingOrder = !finished ? initialSortingOrders[i] + 2 : initialSortingOrders[i];

        if (dropAndHold) {
            Dragging = !finished;
            finished = !Dragging ? Dragging : finished;
        }
    }

    private void OnMouseUp()
    {
        if (!toggle) {
            snapped = SnapToPot();
            Dragging = false;
        }

        if (Dragging && dropAndHold) {
            if (lastMoveTo != null && itemBeingDragged != null) {
                itemBeingDragged.Lift(from: lastMoveTo);
            }
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
            spriteRenderers[i].sortingOrder = Dragging ? initialSortingOrders[i] + 2 : initialSortingOrders[i];
    }

    //Snaps the object to the nearest flowerpot
    private bool SnapToPot()
    {
        if (itemBeingDragged == null) {
            itemBeingDragged = gameObject.GetComponent<IDraggable>();
        }

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
            if (test < difference && test <= maxSnapDistance) {
                moveTo = flowerpot;
                difference = test;
            }
        }

        if (gameObject.tag != "Flower" && (!dropAndHold || (Dragging && Vector2.Distance(oldPosition, gameObject.transform.position) < 1.0f))) {
            gameObject.transform.position = oldPosition;
            finished = true;
        }
        else {
            finished = false;
        }

        if (moveTo != null && gameObject.tag == "Flower") {
            gameObject.transform.position = moveTo.transform.position;
        }


        //Check if overlapping another flower
        //Swapping will only occur if both objects have the tag "flower"
        //Admittedly this probably isn't the best way to do swapping but for the purposes of this game it likely won't matter
        flowers = GameObject.FindGameObjectsWithTag("Flower");
        foreach(GameObject flower in flowers)
        {
            if(flower.transform.position == transform.position && flower != gameObject && gameObject.tag == "Flower")
            {
                flower.transform.position = this.oldPosition;
                DragAndDrop flowerScript = flower.GetComponent<DragAndDrop>();
                IDraggable flowerDraggable = flower.GetComponent<IDraggable>();
                flowerScript.oldPosition = flower.transform.position;
                if (flowerDraggable != null) {
                    flowerDraggable.Lift(from: flowerScript.lastMoveTo);
                }
                if (flowerScript.itemBeingDragged != null) {
                    flowerScript.itemBeingDragged.Drop(onto: lastMoveTo);
                }
            }
        }

        // Allows the plant or watering can to know when it has been dropped and where
        if (itemBeingDragged != null) {
            itemBeingDragged.Drop(onto: moveTo);
            lastMoveTo = moveTo;
        }

        if (gameObject.tag == "Flower") {
            oldPosition = this.transform.position;
        }

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
