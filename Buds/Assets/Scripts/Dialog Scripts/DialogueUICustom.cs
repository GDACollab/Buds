using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUICustom : Yarn.Unity.DialogueUIBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override Dialogue.HandlerExecutionType RunLine(Yarn.Line line, IDictionary<string, string> strings, System.Action onComplete)
    {
        // Start displaying the line; it will call onComplete later
        // which will tell the dialogue to continue
        StartCoroutine(DoRunLine(line, strings, onComplete));
        return Dialogue.HandlerExecutionType.PauseExecution;
    }

    private IEnumerator DoRunLine(Yarn.Line line, IDictionary<string, string> strings, System.Action onComplete)
    {
        Debug.Log(strings[line.ID]);    //this was a strange work-around Tino figured out. Maybe?
        yield return new WaitForSeconds(1);
        onLineComplete();
    }

    public override void RunOptions(Yarn.OptionSet optionsCollection, IDictionary<string, string> strings, System.Action<int> selectOption)
    {
        throw new System.NotImplementedException("NO OPTIONS SUPPORTED");
    }

    public override Dialogue.HandlerExecutionType RunCommand(Yarn.Command command, System.Action onComplete)
    {
        throw new System.NotImplementedException("NO COMMANDS SUPPORTED");
    }

}
