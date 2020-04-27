using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notebook : MonoBehaviour
{
    public GameObject nextButton;
    public GameObject prevButton;
    public GameObject notebookPage;
    public Sprite coverImage;

    public GameObject PersistentData;

    LinkedList<Page> book;
    LinkedListNode<Page> activePage;
    Text pageText;
    Image pageSprite;
    int index;

    void Start()
    {
        pageText = notebookPage.transform.GetChild(0).gameObject.GetComponent<Text>();
        pageSprite = notebookPage.transform.GetChild(1).gameObject.GetComponent<Image>();

        if (PersistentData == null)
        {
            book = new LinkedList<Page>();
            book.AddFirst(new LinkedListNode<Page>(new Page(null, coverImage)));
        }
        else
        {
            //book = book in persistent data
        }

        activePage = book.First;
        index = 0;
        UpdatePage();
    }
    
    void UpdatePage()
    {
        pageText.text = activePage.Value.getText();
        pageSprite.sprite = activePage.Value.getImage();

        if (!pageSprite.gameObject.activeSelf)
        {
            pageSprite.gameObject.SetActive(true);
        }

        if(pageSprite.sprite == null)
        {
            pageSprite.gameObject.SetActive(false);
        }
    }

    public void AddPage(string text, Sprite image)
    {
        book.AddLast(new LinkedListNode<Page>(new Page(text, image)));
    }

    public void PreviousPage()
    {
        //Debug.Log("prev page");
        activePage = activePage.Previous;
        --index;
        if(index == 0)
        {
            prevButton.SetActive(false);
        }
        if(!nextButton.activeSelf)
        {
            nextButton.SetActive(true);
        }
        UpdatePage();
    }

    public void NextPage()
    {
        //Debug.Log("next page");
        activePage = activePage.Next;
        ++index;
        //if we're at the last page
        if(index == book.Count - 1)
        {
            nextButton.SetActive(false);
        }
        if (!prevButton.activeSelf)
        {
            prevButton.SetActive(true);
        }
        UpdatePage();
    }

    public void ToggleNotebook()
    {
        //toggle the main page
        notebookPage.SetActive(!notebookPage.activeSelf);

        //if the notebook is now off, turn off buttons
        if (!notebookPage.activeSelf)
        {
            prevButton.SetActive(false);
            nextButton.SetActive(false);
        }
        //if the notebook is now on, turn on needed buttons
        else
        {
            if(index > 0)
            {
                prevButton.SetActive(true);
            }

            if (index < book.Count - 1)
            {
                nextButton.SetActive(true);
            }
        }
    }

    public void ExportBook()
    {
        //saves the Notebook to persistent data
    }
}

class Page
{
    string text;
    Sprite image;    //or some type to hold a picture

    public Page(string nText, Sprite nImage)
    {
        text = nText;
        image = nImage;
    }

    public string getText()
    {
        return text;
    }

    public Sprite getImage()
    {
        return image;
    }
}