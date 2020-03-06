using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

//get rid of me later
using System.Diagnostics;


public class WordFunctions : MonoBehaviour
{
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    /*// Update is called once per frame
    void Update()
    {
        
    }*/

    //currently this method doesn't actually do anything.
    //It should, and the asserts confirm that it does
    //but the effects never actually manifest.
    //please
    [YarnCommand("changeSpeaker")]
    public void changeSpeaker(string newSpeaker)
    {
        UnityEngine.Debug.Log("The command is actually executing");
        switch (newSpeaker)
        {
            case "MC":
                text.color = Color.white;
                System.Diagnostics.Debug.Assert(text.color == Color.white);
                break;

            case "SF":
                //throw new NotImplementedException("SILVER FOX TEXT COLOR NOT SET");
                UnityEngine.Debug.Log("SILVER FOX TEXT COLOR NOT SET");
                break;

            case "RF":
                text.color = Color.yellow;
                System.Diagnostics.Debug.Assert(text.color == Color.yellow);
                break;

            case "GB":
                //throw new NotImplementedException("GAMER BRAT TEXT COLOR NOT SET");
                UnityEngine.Debug.Log("GAMER BRAT TEXT COLOR NOT SET");
                break;

            default:
                UnityEngine.Debug.Log("Error: " + newSpeaker + " not a recognized speaker. Defaulting to MC as speaker");
                text.color = Color.white;
                break;
        }

    }
}
