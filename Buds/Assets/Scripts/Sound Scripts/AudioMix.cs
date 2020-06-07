using UnityEngine;
using UnityEngine.Audio;

public class AudioMix : MonoBehaviour {

    public AudioMixer mixer;
    public string group;

    public void SetSound(float soundLevel) {
        mixer.SetFloat(group, Mathf.Log(soundLevel + 0.01f) * 20f);
    }

    void Start() {
        mixer.GetFloat(group, out float val);
        GetComponent<UnityEngine.UI.Slider>().value = Mathf.Exp(val / 20f + 0.01f);
    }
}
