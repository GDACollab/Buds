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

        int lineTextIndex;

        void Start()
        {

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
            //Debug.Log(strings[line.ID]);
            //add command functionality
            //text.text = lineText
            for(int i = 0; i < lineText.Length; ++i)
            {
                if(lineText[i] == '{' && lineText[i+1] == '{')
                {
                    i = CreateTextButton(lineText, i);
                }
                else
                {
                    text.text += lineText[i];
                }
            }
            goToNextLine = false;
            yield return new WaitUntil(() => goToNextLine);
            //yield return new WaitForSeconds(3);
            onLineComplete();
        }

        public void NextLine()
        {
            Debug.Log("Dialogue advancement!");
            goToNextLine = true;
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
        //An in-line button is created with the following syntax
        //{{text that is highlighted| }} or {{text that is highlighted|YarnFileName.NextNodeName}}
        //Using | }} will go to the immediate next line of dialog, while using |YarnFileName.NextNodeName}} will jump to the NextNodeName node in the yarn file.
        private int CreateTextButton(string LineText, int index)
        {
            throw new NotImplementedException("IN TEXT OPTIONS NOT FINISHED");
            string buttonText;
            string nextNode;
            Button newButton;
            Text newText;
            bool breaker = false;
            ++index;
            //reads the text to be highlighted 
            while (!breaker)
            {
                if(LineText[index] == '|')
                {
                    breaker = true;
                }
                else
                {
                    buttonText += LineText[index];
                }
                ++index;
            }
            //reads the node to travel to when the button is clicked
            while(LineText[index] != '}' && LineText[index + 1] != '}')
            {
                //please make sure to put in this second brace I haven't written an exception yet and it WILL explode if it isn't found
                nextNode += LineText[index];
                ++index;
            }
            //creates the actual new button and sets it text
            newButton = Instantiate(button, DialogContainer.transform);
            newButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = buttonText;
            //gives the button proper functionality
            if (nextNode == " ")
            {
                //button advances dialogue if there's no node destination
                newButton.onClick.AddListener(this.NextLine);
            }
            else
            {
                //DialogueRunner.NodeExists(node) returns true/false if the node is, in fact, a node.
                //newButton.onClick.AddListener();//method that causes node jumps
                //the yarn way is really complicated and I don't understand it
                //need better examples
                //the secret lies in DialogueUI.SetOption, which I can't find in the DialogueUI script.
                //But the script it looks for is DialogueRunner...?
            }

            //creates a new text object for the rest of the line to go on.
            newText = Instantiate(text, DialogContainer.transform);
            text = newText;
            return index;
        }
    }
}