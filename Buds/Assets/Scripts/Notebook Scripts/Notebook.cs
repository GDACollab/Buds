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

    //public GameObject PersistentData;

    LinkedList<Page> book;
    LinkedListNode<Page> activePage;
    Text pageText;
    Image pageSprite;
    Image mainPageSprite;
    Sprite blankPage;
    int index;

    void Start()
    {
        pageText = notebookPage.transform.GetChild(0).gameObject.GetComponent<Text>();
        pageSprite = notebookPage.transform.GetChild(1).gameObject.GetComponent<Image>();
        mainPageSprite = notebookPage.GetComponent<Image>();
        blankPage = mainPageSprite.sprite;

        //If the notebook has never been defined
        if (PersistentData.instance == null || !PersistentData.instance.ContainsKey("BOOK"))
        {
            book = new LinkedList<Page>();
            book.AddFirst(new LinkedListNode<Page>(new Page(null, coverImage)));
            PersistentData.instance.StoreData("BOOK", book);

            //setting default pages of the notebook
            AddPage(null, Resources.Load<Sprite>("notebookWaterPage_UI_Joann Long 1"));
            AddPage(null, Resources.Load<Sprite>("notebookPlacementPage_UI_Joann Long 1"));
        }
        //If the notebook has been defined
        else
        {
            book = (LinkedList<Page>)PersistentData.instance.ReadData("BOOK");
        }

        activePage = book.First;
        index = 0;
        UpdatePage();
    }
    
    void UpdatePage()
    {
        string pulledText = activePage.Value.getText();
        Sprite pulledImage = activePage.Value.getImage();

        //change the background depending on what page we're on
        if(index != 0)
        {
            mainPageSprite.sprite = blankPage;
        }
        else
        {
            mainPageSprite.sprite = coverImage;
        }

        //if the image is not active from the previous page
        if (!pageSprite.gameObject.activeSelf && pulledImage != null)
        {
            pageSprite.gameObject.SetActive(true);
        }

        //if neither
        if (pulledText == null && pulledImage == null)
        {
            //the thing to do if the page is blank
        }
        //if text and no image
        else if (pulledImage == null)
        {
            //pageSprite.gameObject.SetActive(false);
        }
        //if image and no text
        else if (pulledText == null)
        {
            pageSprite.gameObject.SetActive(false);
            pageText.text = "";
            mainPageSprite.sprite = pulledImage;
        }
        //if both
        else
        {
            pageText.text = pulledText;
            pageSprite.sprite = pulledImage;
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