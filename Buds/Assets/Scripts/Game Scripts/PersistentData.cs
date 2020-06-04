using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements a singleton object that allows for the transfer of data between scenes.
/// Stores all entries in a hashtable accessible by user-defined keys (all of type string, e.g. "dataName")
/// Access the various methods by referencing the PersistentData.instance singleton, e.g. PersistentData.instance.StoreData("key", yourVarHere);
/// Available methods:
///		void StoreData(string key, object value)		-stores "value" at the specified index
///		object ReadData(string key)						-returns the data at the specified index (or null if there is no such entry)
///		void RemoveData(string key)						-removes the entry at the specified index
///		bool ContainsKey(string key)					-returns whether there's an entry for the given key
/// See PersistTest.cs and the TestPersistent1 and TestPersistent2 scenes for an example of how to use the system
/// </summary>
public class PersistentData : MonoBehaviour
{
	//Globally-accessible instance used to access the data.
	//To access its methods, just call PersistentData.instance.StoreData("key", yourVarHere)
    public static PersistentData instance;

    //big hashtable to store all data
    Hashtable dataStorage = new Hashtable();

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

    //If you need an entry to appear on startup, declare it here.
    //NOTE: to avoid merge conflicts, only ONE person should edit Initialize()
    private void Initialize()
    {
    	dataStorage["testInt"] = 0;
        dataStorage["TutorialDone"] = false;
    }

    //StoreData will write data to the master hashtable, which can then be accesed by a key.
    //It will automatically create an entry if one doesn't already exist.
    public void StoreData(string key, object value)
    {
    	dataStorage[key] = value;
    }

    //ReadData will retrieve the entry from the master hashtable with the given key.
    //Returns null if the entry is not found.
    public object ReadData(string key)
    {
    	if(dataStorage.ContainsKey(key))
    	{
    		return dataStorage[key];
    	}
    	else
    	{
    		Debug.LogError("Error: no persistent data entry for key: " + key);
    		return null;
    	}
    }

    //RemoveData will remove the specified entry in the master hashtable
    public void RemoveData(string key)
    {
    	if(dataStorage.ContainsKey(key))
    	{
    		dataStorage.Remove(key);
    	}
    	else
    	{
    		Debug.Log("Cannot delete persistent entry for \"" + key + "\" as there is no such entry");
    	}
    }

    //ContainsKey will return whether or not there is an entry for the given key
    public bool ContainsKey(string key)
    {
    	return dataStorage.ContainsKey(key);
    }

    //Clear removes all entries from the master hastable
    public void Clear() {
        dataStorage.Clear();
    }
}
