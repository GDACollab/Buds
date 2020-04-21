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
    public bool offsetOn = true;
    public bool yMovementOnly;
    public float maxY;
    public float minY;
    public float animationTime;

    [HideInInspector]
    public bool background;

    bool finished;
    bool dragging;

    bool animating;
    Vector2 targetPosition;
    Vector2 sourcePosition;
    float animationTimer;

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
        dragging = false;
        snapped = true;
        oldPosition = this.transform.position; //Stores the last location of this object before it was moved by the mouse.

        if (maxSnapDistance == 0) {
            maxSnapDistance = Mathf.Infinity;
        }
        if (maxY == 0) {
            maxY = Mathf.Infinity;
        }
        if (minY == 0) {
            minY = Mathf.NegativeInfinity;
        }

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        initialSortingOrders = new int[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            initialSortingOrders[i] = spriteRenderers[i].sortingOrder;

        itemBeingDragged = gameObject.GetComponent<IDraggable>();

        sourcePosition = transform.position;
        targetPosition = transform.position;

        if (gameObject.GetComponent<RectTransform>() == null) {
            SnapToPot();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 offset = offsetOn ? new Vector2(-0.5f, 0.5f) : Vector2.zero;
        if (dragging && screenRect.Contains(Input.mousePosition))
        {
            //Mouse position on the screen uses different coordinates, needs to be corrected
            if (mousePos.y >= minY && mousePos.y <= maxY) {
                transform.position = !yMovementOnly
                ? mousePos + offset
                : new Vector2(transform.position.x, mousePos.y);
            }
            else if (mousePos.y <= minY) {
                transform.position = new Vector2(transform.position.x, minY);
            }
            else {
                transform.position = new Vector2(transform.position.x, maxY);
            }

            
        }

        if (dragging) {
            transform.position = new Vector3(transform.position.x, transform.position.y, -1.0f);
        }
        else if (Mathf.Abs(transform.position.z) > Mathf.Epsilon) {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
        }

        if (Vector2.SqrMagnitude(sourcePosition - targetPosition) > Mathf.Epsilon && animationTime != 0.0f && animationTimer <= 1.0f) {
            transform.position = Vector2.Lerp(sourcePosition, targetPosition, animationTimer);
            animationTimer += Time.fixedDeltaTime / animationTime;
            animating = true;
        }
        else {
            animating = false;
            sourcePosition = targetPosition;
            animationTimer = 0.0f;
            if (!dragging && background && !dropAndHold) {
                SnapToPot();
                background = false;
            }
        }
    }

    private void OnMouseDown()
    {
        if (enabled) {
            if (!animating) {
                if (!dragging) {
                    dragging = true;
                    snapped = false;
                }
                else if (dragging && toggle) {

                    snapped = SnapToPot();
                    dragging = false;

                }

                for (int i = 0; i < spriteRenderers.Length; i++)
                    spriteRenderers[i].sortingOrder = !finished ? initialSortingOrders[i] + 2 : initialSortingOrders[i];

                if (dropAndHold) {
                    dragging = !finished;
                    finished = !dragging ? dragging : finished;
                }
            }
        }
        
    }

    private void OnMouseUp()
    {
        if (enabled) {
            if (!animating) {
                if (!toggle) {
                    snapped = SnapToPot();
                    dragging = false;
                }

                if (dragging && dropAndHold) {
                    if (lastMoveTo != null && itemBeingDragged != null) {
                        itemBeingDragged.Lift(from: lastMoveTo);
                    }
                }


                for (int i = 0; i < spriteRenderers.Length; i++)
                    spriteRenderers[i].sortingOrder = dragging ? initialSortingOrders[i] + 2 : initialSortingOrders[i];
            }
        }
        
    }

    public void SetTarget() {
        oldPosition = this.transform.position;
        sourcePosition = transform.position;
        targetPosition = transform.position;
    }

    //Snaps the object to the nearest flowerpot
    private bool SnapToPot()
    {
        sourcePosition = transform.position;

        flowerpots = GameObject.FindGameObjectsWithTag("Flowerpot");
        GameObject moveTo = null;

        float difference = Mathf.Infinity;

        if (flowerpots.Length <= 0)
        {
            targetPosition = transform.position;
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

        if (gameObject.tag != "Flower" && (!dropAndHold || (dragging && Vector2.Distance(oldPosition, gameObject.transform.position) < 1.0f))) {
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
            if(Vector3.SqrMagnitude(flower.transform.position - transform.position) < 0.01f && flower != gameObject && gameObject.tag == "Flower")
            {
                DragAndDrop flowerScript = flower.GetComponent<DragAndDrop>();
                flowerScript.sourcePosition = flowerScript.transform.position;

                flower.transform.position = animationTime == 0
                    ? this.oldPosition
                    : flowerScript.sourcePosition;
                flowerScript.targetPosition = this.oldPosition;

                flowerScript.background = true;
                flowerScript.oldPosition = flowerScript.targetPosition;
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

        targetPosition = transform.position;
        transform.position = animationTime == 0
            ? new Vector2(transform.position.x, transform.position.y)
            : sourcePosition;

        return true;
    }
}
