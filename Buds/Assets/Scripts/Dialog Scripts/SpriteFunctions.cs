using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;


public class SpriteFunctions : MonoBehaviour
{
    public Sprite[] sprites = null; 

    // Start is called before the first frame update
    void Start()
    {

    }

    /*// Update is called once per frame
    void Update()
    {
        
    }*/


    [YarnCommand("changeFace")]
    public void ChangeSprite(string nextSprite)
    {
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

        gameObject.GetComponent<Image>().sprite = match;
    }

    public void startFade(float duration)
    {
        StartCoroutine(fadeSprite(duration));
    }

    IEnumerator fadeSprite(float duration)
    {
        Debug.Log("Starting Fade");
        Image sprite = this.gameObject.GetComponent<Image>();
        for(float fade = 1; fade > 0.0; fade -= Time.deltaTime / duration)
        {
            Debug.Log("Current fade level: " + fade);
            sprite.color = new Color(sprite.color.r, sprite.color.b, sprite.color.g, fade);
            Debug.Log("Character Faded to " + sprite.color.a);
            yield return null;
        }
        Debug.Log("CharacterFade Done");
    }
}
