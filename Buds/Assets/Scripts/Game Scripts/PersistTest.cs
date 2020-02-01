using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Primarily used to test the PersistentData singleton, but also serves as an example of how to use it.
/// NOTE: these functions were written assuming that they would be mapped to UI buttons.
/// See the TestPersistent1 and TestPersistent2 scenes for an example.
/// </summary>
public class PersistTest : MonoBehaviour
{
    //replace button text with the specified string
    public void SetText(string text)
    {
    	Text txt = transform.Find("Text").GetComponent<Text>(); //get the button's text component
    	txt.text = text; //replace its string with the one passed as an argument
    }

    //tests writing to persistent data
    public void StoreVal(int value)
    {
    	PersistentData.instance.StoreData("testInt", value); //general format for writing to the singleton
    	SetText("Set exampleVar to " + value); //update button text to show the value we stored in the singleton
    }

    //tests reading from persistent data
    public void readVal()
    {
    	SetText("Read exampleVar: " + PersistentData.instance.ReadData("testInt")); //general format for reading from the singleton
    }

    //changes between scenes to confirm persistence
    public void swapScene(string sceneName)
    {
    	SceneManager.LoadScene(sceneName); //just call Unity's SceneManager. Probably jank, but okay for testing purposes.
    }
}
