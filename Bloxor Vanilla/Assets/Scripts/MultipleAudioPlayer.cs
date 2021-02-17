using UnityEngine;
using System.Collections;

public class MultipleAudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] clips; // Use this for initialization 

    void Start () {
        clips = Resources.LoadAll<AudioClip>("Music");
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
    } 
    
    // Update is called once per frame
    void Update () {
        if (!audioSource.isPlaying)
        { audioSource.clip = clips[Random.Range(0, clips.Length)];
            audioSource.Play();
        }
    }

}