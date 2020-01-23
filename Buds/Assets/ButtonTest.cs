using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
