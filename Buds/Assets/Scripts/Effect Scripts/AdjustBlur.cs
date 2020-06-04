using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustBlur : MonoBehaviour
{

    private UnityEngine.Rendering.PostProcessing.PostProcessVolume blurObj;

    public float blur;
    public bool on;
    public bool hold;

    public float blurTime = 1f;

    public DragAndDrop[] backgroundObjects;

    // Start is called before the first frame update
    void Start()
    {
        blurObj = Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!on && !hold) {
            blurObj.weight = blur;

            SetBGInteractivity(blur <= 0.5f);
        }   
    }

    public void ToggleOn() {
        on = !on;

        Start();

        blurObj.weight = blur = on ? 1f : 0f;

        SetBGInteractivity(blur <= 0.5f);
    }

    public void Off() {
        on = false;
    }

    public void StopHold() {
        hold = false;
    }

    // turn on or off drag and drop functionality for background elements
    public void SetBGInteractivity(bool state) {
        foreach (DragAndDrop element in backgroundObjects) {
            element.enabled = state;
        }
    }
}
