using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;

namespace Yarn.Unity
{ 
    public class DialogUICustom : Yarn.Unity.DialogueUIBehaviour
    {
        public Text text;
        public Button button;
        public Text buttonText;
        public bool goToNextLine;
        public GameObject DialogContainer;
        //public DialgoueRunner dialogueRunner;
        public GameObject DialogueRunner;

        Script dr;  //what's the thing that allows me to pull script components?

        //called when the object is loaded
        //This will be used to add universal custom commands, which are defined after all the public handler methods.
        void Start()
        {
            dr = DialogueRunner.GetComponent<DialogueRunner>();
            dr.AddCommandHandler("create_text_button", CreateTextButton);
        }

        public override Dialogue.HandlerExecutionType RunLine(Line line, IDictionary<string, string> strings, Action onLineComplete)
        {
            // Start displaying the line; it will call onComplete later
            // which will tell the dialogue to continue
            //goToNextLine = false;
            StartCoroutine(DoRunLine(line, strings, onLineComplete));
            return Dialogue.HandlerExecutionType.PauseExecution;
        }

        private IEnumerator DoRunLine(Line line, IDictionary<string, string> strings, Action onLineComplete)
        {
            var lineText = strings[line.ID];
            Debug.Log(strings[line.ID]);
            //add command functionality
            text.text = lineText;
            //yield return new WaitUntil(() => goToNextLine);
            yield return new WaitForSeconds(3);
            onLineComplete();
        }

        /// Run a command.
        public override Dialogue.HandlerExecutionType RunCommand(Yarn.Command command, System.Action onComplete)
        {
            throw new NotImplementedException("NO COMMAND SUPPORT");
        
            // Dispatch this command via the 'On Command' handler.
            //onCommand?.Invoke(command.Text);

            // Signal to the DialogueRunner that it should continue executing.
            //return Dialogue.HandlerExecutionType.ContinueExecution;
        }

        //RunOptions makes lists of buttons to choose options from. This game won't have that, so it's not a big deal.
        public override void RunOptions(OptionSet optionSet, IDictionary<string, string> strings, Action<int> onOptionSelected)
        {
            throw new NotImplementedException("NO OPTION SUPPORT");
            //StartCoroutine(DoRunOptions(optionsCollection, strings, selectOption));
        }

        //Creates buttons in-line with the text that can be clicked on. We can make them look like regular text later
        private void CreateTextButton(string[] parameters)
        {
            throw new NotImplementedException("IN TEXT OPTIONS NOT FINISHED");
        }
    
    }
}