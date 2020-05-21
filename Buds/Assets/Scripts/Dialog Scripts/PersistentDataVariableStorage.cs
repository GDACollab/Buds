using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentDataVariableStorage : Yarn.Unity.VariableStorageBehaviour
{
    //Despite what it may seem, this script doesn't store variables
    //Rather, it is an interface that allows PersistentData and Yarn to talk to each other

    /// A default value to apply when the object wakes up, or when
    /// ResetToDefaults is called
    [System.Serializable]
    public class DefaultVariable
    {
        /// Name of the variable
        public string name;
        /// Value of the variable
        public string value;
        /// Type of the variable
        public Yarn.Value.Type type;
    }

    /// Our list of default variables, for debugging.
    public DefaultVariable[] defaultVariables;

    // Store a value into a variable
    public override void SetValue(string variableName, Yarn.Value value)
    {
        PersistentData.instance.StoreData(variableName, value);
    }

    // Return a value, given a variable name
    public override Yarn.Value GetValue(string variableName)
    {
        Yarn.Value newValue = null;

        switch (variableName)
        {
            case "$narcissus_growth_stage":
                Plant narcissus = (Plant)PersistentData.instance.ReadData("Narcissus");
                newValue = new Yarn.Value((float)narcissus.growthStage);
                break;

            case "$cyclamen_growth_stage":
                Plant cyclamen = (Plant)PersistentData.instance.ReadData("Cyclamen");
                newValue = new Yarn.Value((float)cyclamen.growthStage);
                break;

            default:
                newValue = (Yarn.Value)PersistentData.instance.ReadData(variableName);
                break;
        }

        return newValue; 
    }

    // Return to the original state
    public override void ResetToDefaults()
    {
        //Clear();

        // For each default variable that's been defined, parse the
        // string that the user typed in in Unity and store the
        // variable
        foreach (var variable in defaultVariables)
        {

            object value;

            switch (variable.type)
            {
                case Yarn.Value.Type.Number:
                    float f = 0.0f;
                    float.TryParse(variable.value, out f);
                    value = f;
                    break;

                case Yarn.Value.Type.String:
                    value = variable.value;
                    break;

                case Yarn.Value.Type.Bool:
                    bool b = false;
                    bool.TryParse(variable.value, out b);
                    value = b;
                    break;

                case Yarn.Value.Type.Variable:
                    // We don't support assigning default variables from
                    // other variables yet
                    Debug.LogErrorFormat("Can't set variable {0} to {1}: You can't " +
                        "set a default variable to be another variable, because it " +
                        "may not have been initialised yet.", variable.name, variable.value);
                    continue;

                case Yarn.Value.Type.Null:
                    value = null;
                    break;

                default:
                    throw new System.ArgumentOutOfRangeException();

            }

            var v = new Yarn.Value(value);

            if (!PersistentData.instance.ContainsKey("$" + variable.name))
            {
                //Debug.Log("PersistentData doesn't have a " + variable.name);
                SetValue("$" + variable.name, v);
            }          
        }
    }

    //this method exists to remove all twine data from the storage method
    //I'm sure there's a way to enumerate through a hashtable and remove all the data
    //of type Yarn.Value, but I'm not gonna do it right now
    public override void Clear()
    {
        //method
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
