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


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = intro;
        audioSource.loop = false;
        audioSource.Play();
    }

    void FixedUpdate()
    {
        if(!audioSource.isPlaying) {
            audioSource.clip = loop;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
