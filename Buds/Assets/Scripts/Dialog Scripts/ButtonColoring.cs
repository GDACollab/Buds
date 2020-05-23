using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonColoring : MonoBehaviour
{
    public Color standard;
    public Color highlighted;
    TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = this.gameObject.GetComponent<TextMeshProUGUI>();
        text.color = standard;
    }

    public void onPointerEnter()
    {
        Debug.Log("Mouse in");
        text.color = highlighted;
    }

    public void onPointerExit()
    {
        Debug.Log("Mouse out");
        text.color = standard;
    }
}
