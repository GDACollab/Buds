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
        }
            
    }

    public void ToggleOn() {
        on = !on;

        Start();

        blurObj.weight = blur = on ? 1f : 0f;
    }

    public void Off() {
        on = false;
    }

    public void StopHold() {
        hold = false;
    }
}
