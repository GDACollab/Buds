using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn;

namespace Yarn.Unity
{
    public class DialogUICustom : Yarn.Unity.DialogueUIBehaviour
    {
        public TextMeshProUGUI DialogTextPrefab;
        public Button DialogButtonPrefab;
        public GameObject DialogContainerPrefab;
        public GameObject DialogSuperContainer;
        public bool goToNextLine;
        public int lineTextIndex;

        DialogueRunner DialogueRunner;
        int activeButtons;
        public string currentSpeaker;
        Color textColor = Color.yellow;

        TextMeshProUGUI text;
        Button button;
        TextMeshProUGUI buttonText;
        GameObject DialogContainer;

        OrderedSceneNavigator phoneFunctions;
        SpriteFunctions characterFunctions;

        /// How quickly to show the text, in seconds per character
        [Tooltip("How quickly to show the text, in seconds per character")]
        public float textSpeed = 0.025f;

        /// The buttons that let the user choose an option
        public List<Button> optionButtons;

        //A whole lot of weird YarnSpinner things. I don't know what they do, so 
        //I use them as I need them

        //Yarn things I am using
        public UnityEngine.Events.UnityEvent onDialogueStart;

        public UnityEngine.Events.UnityEvent onDialogueEnd;

        public UnityEngine.Events.UnityEvent onLineStart;
        public UnityEngine.Events.UnityEvent onLineFinishDisplaying;
        public DialogueRunner.StringUnityEvent onLineUpdate;
        public UnityEngine.Events.UnityEvent onLineEnd;

        public UnityEngine.Events.UnityEvent onOptionsStart;
        public UnityEngine.Events.UnityEvent onOptionsEnd;

        public DialogueRunner.StringUnityEvent onCommand;

        void Awake()
        {
            DialogueRunner = this.gameObject.GetComponent<DialogueRunner>();
            if(DialogueRunner == null)
            {
                Debug.LogWarning("Dialogue Runner not found");
            }

            //phoneFunctions = 
            if(GameObject.Find("PhoneButton") == null)
            {
                Debug.LogWarning("Phone (or its Ordered Scene Navigator) not found");
                phoneFunctions = null;
            }
            else
            {
                phoneFunctions = GameObject.Find("PhoneButton").transform.GetChild(1).gameObject.GetComponent<OrderedSceneNavigator>();
            }

            characterFunctions = DialogSuperContainer.transform.parent.GetChild(1).gameObject.GetComponent<SpriteFunctions>();
            if(characterFunctions == null)
            {
                Debug.LogWarning("Character (or its Sprite Functions) not found");
            }

            DialogueRunner.AddCommandHandler("changeSpeaker", changeSpeaker);

            TextUIReset();
        }

        /*
         * Default YarnSpinner Methods (with some modification)
         */

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
            int charsOnLine = 0;
            TextUIReset();

            for (int i = 0; i < lineText.Length; ++i)
            {
                if (charsOnLine > 50 && lineText[i] == ' ')
                {
                    //checking whether or not there is a started italics in the original line
                    if(Regex.Match(text.text, @"<i>").Success)
                    {
                        text.text += "</i>";
                        TextNewLine(true);
                    }
                    else
                    {
                        TextNewLine(false);
                    }
                    charsOnLine = 0;
                }
                else if (lineText[i] == '{' && i + 1 < lineText.Length -1 && lineText[i + 1] == '{')
                {
                    int oldI = i;
                    i = CreateTextButton(lineText, i);
                    charsOnLine += i - oldI;

                }
                else if(lineText[i] == ' ')
                {
                    //TextMeshPro needs Non-Breaking Spaces to properly render lines, so I'm replacing all spaces with them
                    text.text += '\u00A0';
                }
                else
                {
                    text.text += lineText[i];
                }
                ++charsOnLine;
                //Debug.Log(i);
            }

            //If the line has no buttons, continue in 3 seconds
            //If it does, wait until one is clicked 
            goToNextLine = false;
            if (activeButtons == 0)
            {
                yield return new WaitForSeconds(3);
                Debug.Log("Waiting...");
            }
            else
            {
                yield return new WaitUntil(() => goToNextLine);
            }
            onLineComplete();
        }

        public void NextLine()
        {
            //Debug.Log("Dialogue advancement!");
            goToNextLine = true;
        }

        /// Run a command.
        public override Dialogue.HandlerExecutionType RunCommand(Yarn.Command command, System.Action onComplete)
        {
            //throw new NotImplementedException("NO COMMAND SUPPORT");

            // Dispatch this command via the 'On Command' handler.
            onCommand?.Invoke(command.Text);

            // Signal to the DialogueRunner that it should continue executing.
            return Dialogue.HandlerExecutionType.ContinueExecution;
        }

        //RunOptions makes lists of buttons to choose options from. This game won't have that, so it's not a big deal.
        public override void RunOptions(OptionSet optionSet, IDictionary<string, string> strings, Action<int> onOptionSelected)
        {
            throw new NotImplementedException("NO OPTION SUPPORT");
            //StartCoroutine(DoRunOptions(optionsCollection, strings, selectOption));
        }

        // Called when the dialogue system has finished running.
        public override void DialogueComplete()
        {
            onDialogueEnd?.Invoke();

            // Hide the dialogue interface.
            foreach (Transform child in DialogSuperContainer.transform) {
                GameObject.Destroy(child.gameObject);
            }

            //fade the character being spoken to
            characterFunctions.startFade(1.0f);

            //open the phone
            phoneFunctions.ShowMenu();
        }

        /*
         * Private Add-On Functions
         */


        //Creates buttons in-line with the text that can be clicked on. We can make them look like regular text later
        //An in-line button is created with the following syntax
        //{{text that is highlighted| }} or {{text that is highlighted|YarnFileName.NextNodeName}}
        //Using | }} will go to the immediate next line of dialog, while using |YarnFileName.NextNodeName}} will jump to the NextNodeName node in the yarn file.
        private int CreateTextButton(string LineText, int index)
        {
            //local variables
            string buttonText = "";
            string nextNode = "";
            bool breaker = false;

            //This keeps both braces from being printed
            ++index;
            ++index;

            //reads the text to be highlighted 
            while (!breaker)
            {
                if (LineText[index] == '|')
                {
                    breaker = true;
                }
                else
                {
                    buttonText += LineText[index];
                    /*if (LineText[index] == ' ')
                    {
                        //TextMeshPro needs Non-Breaking Spaces to properly render lines, so I'm replacing all spaces with them
                        text.text += '\u00A0';
                    }
                    else
                    {
                        buttonText += LineText[index];
                    }*/
                }
                ++index;
            }
            //reads the node to travel to when the button is clicked
            while (LineText[index] != '}' || LineText[index + 1] != '}')
            {
                //please make sure to put in this second brace I haven't written an exception yet and it WILL explode if it isn't found
                nextNode += LineText[index];
                ++index;
            }

            //creates the actual new button and sets it text
            button = Instantiate(DialogButtonPrefab, DialogContainer.transform);
            var newButtonText = button.transform.GetChild(0).gameObject;
            //I'm certain there's a better way to move this information, but uhhh....
            newButtonText.GetComponent<ButtonColoring>().speaker = currentSpeaker;
            newButtonText.GetComponent<TextMeshProUGUI>().text = buttonText;

            //gives the button proper functionality
            Debug.Log("NextNode is >" + nextNode + "<");
            if (nextNode == " " || nextNode == "")
            {
                //button advances dialogue if there's no node destination
                button.onClick.AddListener(this.NextLine);
            }
            else
            {
                //checks if the listed node destination actually exists
                if (!DialogueRunner.NodeExists(nextNode))
                {
                    //lol this is an exception but I don't know how to write
                    Debug.Log("Get ready for a null reference exception!");
                }
                else
                {
                    //changes the node to the destination and continues dialogue
                    //ha! a lambda! am I a c# programmer now?
                    button.onClick.AddListener(() => { JumpNode(nextNode); });

                }
            }

            //creates a new, blank, text object for the rest of the line to go on.
            text = Instantiate(DialogTextPrefab, DialogContainer.transform);
            text.text = "";
            text.color = textColor;

            ++activeButtons;
            text.gameObject.SetActive(true);
            button.gameObject.SetActive(true);
            //we return index + 2 so the trailing braces aren't printed
            return index + 1;
        }

        //Resets the objects in the UI so that new ones can be used for new lines
        private void TextUIReset()
        {
            //There were many issues that were fixed.

            //This script was also fixed

            //It was fixed by Thomas Applewhite, who had matured.

            foreach (Transform child in DialogSuperContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            DialogContainer = Instantiate(DialogContainerPrefab, DialogSuperContainer.transform);
            text = Instantiate(DialogTextPrefab, DialogContainer.transform);
            button = Instantiate(DialogButtonPrefab, DialogContainer.transform);
            buttonText = button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

            text.text = "";
            text.color = textColor;
            activeButtons = 0;
        }

        //Creates a new line of text. May God have mercy on my immortal soul
        private void TextNewLine(bool italics)
        {

            DialogContainer = Instantiate(DialogContainerPrefab, DialogSuperContainer.transform);
            text = Instantiate(DialogTextPrefab, DialogContainer.transform);
            text.color = textColor;
            button = Instantiate(DialogButtonPrefab, DialogContainer.transform);
            buttonText = button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

            //adds italics if needed due to a newline operation
            if (italics)
            {
                text.text += "<i>";
            }
        }

        //Changes the dialogue to a different Yarn Node
        private void JumpNode(string node)
        {
            DialogueRunner.dialogue.Stop();
            DialogueRunner.dialogue.SetNode(node);
            DialogueRunner.dialogue.Continue();
        }

        //changes the color of the text to denote who's talking
        private void changeSpeaker(string[] parameters)
        {
            string newSpeaker = parameters[0];
            currentSpeaker = newSpeaker;
            switch (newSpeaker)
            {
                case "MC":
                    textColor = Color.white;
                    break;

                case "SF":
                    //throw new NotImplementedException("SILVER FOX TEXT COLOR NOT SET");
                    UnityEngine.Debug.Log("SILVER FOX TEXT COLOR NOT SET");
                    break;

                case "RF":
                    textColor = Color.yellow;
                    break;

                case "GB":
                    textColor = Color.yellow;
                    break;

                default:
                    UnityEngine.Debug.Log("Error: " + newSpeaker + " not a recognized speaker. Defaulting to MC as speaker");
                    textColor = Color.white;
                    break;
            }

        }
    }
}