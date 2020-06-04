using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    GameObject[] tutorialObjects;
    bool nextSection = false;

    /*
     *      This script runs a very basic tutorial. It looks for
     *  all children of the container object and, starting with 
     *  the first one, activates it until a child button is
     *  clicked. It then does the next one, until it runs out.
     *  Once it runs out, the tutorial is done.
     */


    // Start is called before the first frame update
    void Start()
    {
        if(!(bool)PersistentData.instance.ReadData("TutorialDone"))
        {
            PersistentData.instance.StoreData("TutorialDone", true);
            tutorialObjects = GetChildrenAsObjects(this.gameObject);
            StartCoroutine(TutorialRunner());
        }
    }

    IEnumerator TutorialRunner()
    {
        SetBGandUIInteractivity(false);

        for (int i = 0; i < tutorialObjects.Length; ++i)
        {
            tutorialObjects[i].SetActive(true);
            yield return new WaitUntil( () => nextSection);
            tutorialObjects[i].SetActive(false);
            nextSection = false;
        }

        SetBGandUIInteractivity(true);

        Debug.Log("Tutorial done!");
    }

    private void SetBGandUIInteractivity(bool state) {
        FindObjectOfType<AdjustBlur>().SetBGInteractivity(state);
        GameObject.Find("Phone Button").GetComponent<UnityEngine.UI.Button>().interactable = state;
        GameObject.Find("NotebookToggle").GetComponent<UnityEngine.UI.Button>().interactable = state;
    }

    //smol helper to grab all of the tutorial's objects
    //as an array, for easy batch activation/deactivation
    GameObject[] GetChildrenAsObjects(GameObject parent)
    {
        GameObject[] childs = new GameObject[parent.transform.childCount];

        for(int i = 0; i < parent.transform.childCount; ++i)
        {
            childs[i] = parent.transform.GetChild(i).gameObject;
        }

        return childs;
    }

    //When clicked, buttons activate this method,
    //which continues the tutorial. I could use
    //Unity Events, but uhhhh I don't know
    //how to use those. I should learn.
    public void ContiuneTutorial()
    {
        nextSection = true;
    }
}
