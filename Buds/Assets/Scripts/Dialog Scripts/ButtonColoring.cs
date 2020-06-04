using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonColoring : MonoBehaviour
{
    public Color mcButtons;
    public Color characterButtons;
    public Color highlighted;
    public string speaker = "MC";
    Color standard;
    TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = this.gameObject.GetComponent<TextMeshProUGUI>();
        //everytime I write a ternarry I live another 100 years
        standard = speaker.Equals("MC") ? mcButtons : characterButtons;
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
