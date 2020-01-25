using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DragAndDrop.cs
//Written by Santiago Ponce
//Email: saponce@ucsc.edu

public class DragAndDrop : MonoBehaviour
{
    public bool toggle = true; //Switch between toggle and hold modes
    bool dragging;
    Rect screenRect = new Rect(0, 0, Screen.width, Screen.height); //This is used to prevent the block from moving while the mouse is offscreen

    GameObject[] flowerpots;
    bool snapped;

    // Start is called before the first frame update
    void Start()
    {
        dragging = false;
        snapped = true;
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
        Vector2 moveTo = gameObject.transform.position;

        float difference = Mathf.Infinity;

        if(flowerpots.Length <= 0)
        {
            return false;
        }

        foreach(GameObject flowerpot in flowerpots)
        {
            float test = Vector2.Distance(flowerpot.transform.position, gameObject.transform.position);

            //Vector2 test = flowerpot.transform.position - gameObject.transform.position;
            if(test < difference)
            {
                moveTo = flowerpot.transform.position;
                difference = test;
            }
            
        }

        gameObject.transform.position = moveTo;

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
