using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* <summary>
 * Plays an intro and then a loop for background music.
 * Intro and loop audio clips are set in the inspector.
 * </summary>
 */

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{

    public AudioClip intro;
    public AudioClip loop;
    private AudioSource audioSource;

    [HideInInspector]
    public float volumeMultiplier = 0.5f;

    private float originalVolume;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = intro;
        audioSource.loop = false;
        if (PersistentData.instance.ContainsKey("Volume")) {
            volumeMultiplier = (float)PersistentData.instance.ReadData("Volume");
        }
        originalVolume = audioSource.volume;
        audioSource.Play();


        GameObject volumeMenu = GameObject.Find("Options Submenu");

        if (volumeMenu != null) {
        //    UnityEngine.UI.Slider slider = volumeMenu.GetComponentInChildren<UnityEngine.UI.Slider>();
              volumeMenu.SetActive(false);

        //    slider.onValueChanged.AddListener((value) => {
        //        volumeMultiplier = value;
        //        audioSource.volume = originalVolume * volumeMultiplier;
        //    });
        //    slider.value = volumeMultiplier;
        }

        //audioSource.volume = originalVolume * volumeMultiplier;
    }

    void FixedUpdate()
    {
        if(!audioSource.isPlaying) {
            audioSource.clip = loop;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void OnDisable() {
        PersistentData.instance.StoreData("Volume", volumeMultiplier);
    }
}
