using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentDataVariableStorage : Yarn.Unity.VariableStorageBehaviour
{
    //Despite what it may seem, this script doesn't store variables
    //Rather, it is an interface that allows PersistentData and Yarn to talk to each other

    // Store a value into a variable
    public override void SetValue(string variableName, Yarn.Value value)
    {
        PersistentData.instance.StoreData(variableName, value);
    }

    // Return a value, given a variable name
    public override Yarn.Value GetValue(string variableName)
    {
        return (Yarn.Value)PersistentData.instance.ReadData(variableName);
    }

    // Return to the original state
    public override void ResetToDefaults()
    {
        //aha.... nothing....? Not sure what this does or how I should do it so oh well
    }
    /*// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
