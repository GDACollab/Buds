using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SpriteFunctions : MonoBehaviour
{
    string objectName;

    // Start is called before the first frame update
    void Start()
    {
        objectName = gameObject.name;
    }

    /*// Update is called once per frame
    void Update()
    {
        
    }*/
    //I want to plug objectName into the "name" field, but it gives me an error whenever I try.
    [YarnCommand("name")]
    public void ChangeSprite(string nextSprite)
    {
        //some script stuff that will change the sprite of the current character.
    }
}
