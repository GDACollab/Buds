using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;

public class DialogUICustom : Yarn.Unity.DialogueUIBehaviour
{
    public Text text;
    public Button button;
    public Text buttonText;
    public bool goToNextLine;
    public override Dialogue.HandlerExecutionType RunCommand(Command command, Action onCommandComplete)
    {
        throw new NotImplementedException();
    }

    public override Dialogue.HandlerExecutionType RunLine(Line line, IDictionary<string, string> strings, Action onLineComplete)
    {
        // Start displaying the line; it will call onComplete later
        // which will tell the dialogue to continue
        goToNextLine = false;
        StartCoroutine(DoRunLine(line, strings, onLineComplete));
        return Dialogue.HandlerExecutionType.PauseExecution;
    }

    private IEnumerator DoRunLine(Line line, IDictionary<string, string> strings, Action onLineComplete)
    {
        var lineText = strings[line.ID];
        Debug.Log(strings[line.ID]);
        text.text = lineText;
        //yield return new WaitUntil(() => goToNextLine);
        yield return new WaitForSeconds(3);
        onLineComplete();
    }

    public void GoToNext()
    {
        goToNextLine = true;
    }


    public override void RunOptions(OptionSet optionSet, IDictionary<string, string> strings, Action<int> onOptionSelected)
    {
        throw new NotImplementedException("NO OPTION SUPPORT");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
