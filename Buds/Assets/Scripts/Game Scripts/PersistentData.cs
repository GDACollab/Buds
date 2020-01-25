using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a singleton object that allows for the transfer of data between scenes.
/// HOW TO USE:
/// -To add a variable, simply declare it in the indicated area (line 19 as of this writing)
///    -OPTIONAL: initialize it in the Initialize() function
/// -To access a variable, reference "PersistentData.instance.yourVariableNameHere"
/// See PersistTest.cs and the TestPersistent1 and TestPersistent2 scenes for an example of how to use the system
/// </summary>
public class PersistentData : MonoBehaviour
{
	//globally-accessible instance used to access the data.
	//To access a variable, reference "PersistentData.instance.variableName"
    public static PersistentData instance;

    //put persistent variables here, ideally accompanied by a description of what they go to
    public int exampleVar; //test variable. Used by PersistTest.cs to confirm basic functionality of the singleton 

    private void Awake ()
    {
    	//implement the singleton pattern
    	if (instance != null)
    	{
    		//if another singleton already exists, destroy the new one
    		Destroy(this.gameObject);
    	}
    	else
    	{
    		//if there's not already another singleton, we become the singleton
    		instance = this;
    		DontDestroyOnLoad(this.gameObject); //make sure we persist through scene changes
    		Initialize(); //initialize variables
    	}
    }

    //Initialize variables here
    private void Initialize()
    {
    	exampleVar = 0;
    }
}
