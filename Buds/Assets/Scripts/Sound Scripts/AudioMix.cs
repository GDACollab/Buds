using UnityEngine;
using UnityEngine.Audio;

public class AudioMix : MonoBehaviour {

    public AudioMixer mixer;
    public string group;

    public void SetSound(float soundLevel) {
        mixer.SetFloat(group, Mathf.Log(soundLevel) * 20);
    }
}
