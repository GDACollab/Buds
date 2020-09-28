using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class VoicePlayer : MonoBehaviour
{

    private AudioSource audioSource;

    [HideInInspector]
    public float volumeMultiplier = 0.5f;

    private float originalVolume;

    //Variables for the audio files that are supposed to play.
    //Used before making a list was the decision
    //public AudioClip test1Playstation;
    //public AudioClip test2Bruh;

    public AudioClip[] voiceLines = null;

    //Dictionary for the audio files
    //Used before realizing how the character sprites are managed can also be used for this.
    //public Dictionary<string, AudioClip> voiceDictionary;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //The Yarn Command that allows us to reset the dictionary and change the audio files in it.
    //Used before realizing how the character sprites are managed can also be used for this.
    //[YarnCommand("SetDictionary")]
    //public void SetDictionary()
    //{
    //    voiceDictionary = new Dictionary<string, AudioClip>()
    //    {
    //        { "test1", test1Playstation},
    //        { "test2", test2Bruh }
    //    };
    //    Debug.LogWarning("Dictionary Created");
    //}

    //The Yarn Command that is supposed to access the dictionary for 
    [YarnCommand("PlayAudio")]
    public void PlayAudio(string voiceevent)
    {
        //First stops the previous voice or audio file from playing. 
        audioSource.Stop();

        //Searches for file being asked for. If it does not find the file, it will play the previous one that was set.
        if (voiceevent != null)
        {
            //Check for if the string name called is in the dictionary. Will stay null if the file name is not found.
            AudioClip keyFound = null;
            foreach (AudioClip audioKey in voiceLines)
            {
                if (voiceevent == audioKey.name)
                {
                    keyFound = audioKey;
                    break;
                }
            }
            if (keyFound != null)
            {
                //sets the new audio file to play
                audioSource.clip = keyFound;
            }
            else
            {
                Debug.LogWarning("No name detected in dictionary. Will repeat previous audio file. Repeated Audio: " + audioSource.clip);
            }
        }
        else
        {
            Debug.LogWarning("No new audio requested. Will repeat previous audio file. Repeated Audio: " + audioSource.clip);
        }


        if (PersistentData.instance.ContainsKey("Volume"))
        {
            volumeMultiplier = (float)PersistentData.instance.ReadData("Volume");
        }
        originalVolume = audioSource.volume;
        audioSource.Play();

        GameObject volumeMenu = GameObject.Find("Options Submenu");

        if (volumeMenu != null)
        {
            //    UnityEngine.UI.Slider slider = volumeMenu.GetComponentInChildren<UnityEngine.UI.Slider>();
            volumeMenu.SetActive(false);

            //    slider.onValueChanged.AddListener((value) => {
            //        volumeMultiplier = value;
            //        audioSource.volume = originalVolume * volumeMultiplier;
            //    });
            //    slider.value = volumeMultiplier;
        }
    }

    private void OnDisable()
    {
        PersistentData.instance.StoreData("Volume", volumeMultiplier);
    }
}
