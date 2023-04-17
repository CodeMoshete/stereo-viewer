using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayRandomSoundFromPool : MonoBehaviour
{
    public List<AudioClip> Clips;

    void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        int randomIndex = Random.Range(0, Clips.Count);
        source.clip = Clips[randomIndex];
        source.Play();
    }
}
