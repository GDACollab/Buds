using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;


public class SpriteFunctions : MonoBehaviour
{
    public Sprite[] expressions = null; 

    // Start is called before the first frame update
    void Start()
    {

    }

    /*// Update is called once per frame
    void Update()
    {
        
    }*/
    
    
    [YarnCommand("changeFace")]
    public void ChangeSprite(string nextSprite)
    {
        //some script stuff that will change the sprite of the current character.
        Sprite match = null;
        foreach(Sprite check in expressions)
        {
            if(check.name == nextSprite)
            {
                match = check;
                break;
            }
        }

        //only triggers if the expression you named isn't in the list of expression.
        if(match == null)
        {
            //throw the whole damn thing away idk I don't know how to write C# exceptions
            match = expressions[0];
        }

        gameObject.GetComponent<Image>().sprite = match;
    }
}
