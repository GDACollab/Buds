using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonColoring : MonoBehaviour
{
    public Color standard;
    public Color highlighted;
    Color textColor;
    

    /*
     *  Hi. Thomas again. onMouseEnter doesn't actually work with TMP assets
     *      because they don't have colliders, but they can't use the standard
     *      OnPointerEnter event that most UI uses because they don't extend
     *      Selectable. So, I'm leaving the easier stuff here, probably for
     *      myself, to solve later.
     */

    // Start is called before the first frame update
    void Start()
    {
        textColor = this.gameObject.GetComponent<TextMeshProUGUI>().color;
        textColor = standard;
    }

    void onMouseEnter()
    {
        Debug.Log("Mouse in");
        textColor = highlighted;
    }

    void onMouseExit()
    {
        Debug.Log("Mouse out");
        textColor = standard;
    }
}
