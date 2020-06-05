using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;


public class SpriteFunctions : MonoBehaviour
{
    //images to switch between
    public Sprite[] sprites = null;

    //fade related variables
    public Image fadeOutUIImage;
    public enum FadeDirection { In, Out };

    private SortedList<float, int> sceneOrder;
    private int numCompleted;
    private bool menuEnabled;
    private System.DateTime date;
    private readonly float fadeSpeed = 0.8f;


    private float fadeStartValue;
    private float fadeEndValue;
    private bool fadeCompleted;
    private bool loading;
    private bool fadeStarted;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {

    }

    /*// Update is called once per frame
    void Update()
    {
        
    }*/


    [YarnCommand("changeFace")]
    public void ChangeSprite(string[] args)
    {
        string nextSprite = args[0];
        bool needsFade = false;
        bool needsInvisFade = false;
        if(args.Length > 1 && args[1].Equals("fade"))
        {
            needsFade = true;
        }
        else if(args.Length > 1 && args[1].Equals("invisFade"))
        {
            needsInvisFade = true;
        }

        //some script stuff that will change the sprite of the current character.
        Sprite match = null;
        foreach (Sprite check in sprites)
        {
            if (check.name == nextSprite)
            {
                match = check;
                break;
            }
        }

        //only triggers if the expression you named isn't in the list of expression.
        if (match == null)
        {
            //throw the whole damn thing away idk I don't know how to write C# exceptions
            Debug.LogWarning("Sprite asked for in changeFace command wasn't found");
            match = sprites[0];
        }

        if (needsFade)
        {
            StartCoroutine(Fade(FadeDirection.In));
            StartCoroutine(SecondHalfOfFadeInOut(FadeDirection.Out, match));
        }
        else if (needsInvisFade)
        {
            gameObject.GetComponent<Image>().CrossFadeAlpha(0, 0, true);
            gameObject.GetComponent<Image>().sprite = match;
            startFade(1.0f, 1f);

        }
        else
        {
            //do the non-fade ver
            gameObject.GetComponent<Image>().sprite = match;

        }

    }

    public void startFade(float duration, float finalAlpha)
    {
        //StartCoroutine(fadeSprite(duration, false));
        this.gameObject.GetComponent<Image>().CrossFadeAlpha(finalAlpha, duration, false);
    }

    private IEnumerator SecondHalfOfFadeInOut(FadeDirection direction, Sprite match) {
        yield return new WaitForSeconds(fadeSpeed + 0.1f);

        gameObject.GetComponent<Image>().sprite = match;
        StartCoroutine(Fade(direction));

    }

    // Coroutine to fade an Image or SpriteRenderer
    //because you can't fade one sprite into another so I'm just fading in and out some white
    //I stole everything below this from Ray because I trust him to write code that just works
    private IEnumerator Fade(FadeDirection direction, SpriteRenderer sr = null)
    {
        // Set start and end values if just beginning to fade
        if (!fadeStarted)
        {
            if (direction == FadeDirection.Out)
                fadeStartValue = 1;
            else
                fadeStartValue = 0;
            fadeEndValue = 1 - fadeStartValue;
        }
        fadeStarted = true;

        // Continue to fade in or out until done
        if (direction == FadeDirection.Out)
        {
            while (fadeStartValue >= fadeEndValue)
            {
                if (sr == null)
                    SetTransparencyImage(FadeDirection.Out);
                else
                    SetTransparencySR(FadeDirection.Out);
                yield return null;
            }

            // Disable Image/SR once it has disappeared 
            if (sr == null)
                fadeOutUIImage.enabled = false;
            else
                sr.enabled = false;
        }
        else
        {
            // Enable Image/SR before it appears
            if (sr == null)
                fadeOutUIImage.enabled = true;
            else
                sr.enabled = true;
            while (fadeStartValue <= fadeEndValue + 0.05f)
            {
                if (sr == null)
                    SetTransparencyImage(FadeDirection.In);
                else
                    SetTransparencySR(FadeDirection.In);
                yield return null;
            }
        }

        fadeCompleted = true;
        fadeStarted = false;
    }

    // Helper function for setting transparency on an Image (UI element)
    private void SetTransparencyImage(FadeDirection fadeDirection)
    {
        fadeOutUIImage.color = new Color(
            fadeOutUIImage.color.r,
            fadeOutUIImage.color.g,
            fadeOutUIImage.color.b,
            fadeStartValue
            );
        if (fadeDirection == FadeDirection.Out)
            fadeStartValue -= Time.deltaTime / fadeSpeed;
        else
            fadeStartValue += Time.deltaTime / fadeSpeed;
    }

    // Helper function for setting transparency on a SpriteRenderer
    private void SetTransparencySR(FadeDirection fadeDirection)
    {
        spriteRenderer.color = new Color(
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            fadeStartValue
            );
        if (fadeDirection == FadeDirection.Out)
            fadeStartValue -= Time.deltaTime / fadeSpeed;
        else
            fadeStartValue += Time.deltaTime / fadeSpeed;
    }
}
