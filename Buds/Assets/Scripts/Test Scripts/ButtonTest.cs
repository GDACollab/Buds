using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ButtonTest.cs
 * Written by Santiago Ponce
 * Email: saponce@ucsc.edu
 * 
 * This is a test of UI buttons w/ the placeholder function changePosition();
 * In the button component, this function can be selected so that when the button
 * is pressed, the function is performed.
 */


public class ButtonTest : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changePosition()
    {
        Vector3 newPosition = new Vector3(0, 0);
        gameObject.transform.position = newPosition;
    }
}
