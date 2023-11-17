using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    // Fields for ball hitting paddle/walls
    [SerializeField] private AudioSource collisionSource;
    [SerializeField] List<AudioClip> collisionClips;
    AudioClip clip;

    // Fields for fire burning
    [SerializeField] private AudioSource fireSource; 
    [SerializeField] private AudioClip fireClip;

    private void Start()
    {
        fireSource.clip = fireClip;
        fireSource.loop = true; // If you want the fire sound to loop
        fireSource.volume = 0.0f; // Start with no volume (this does not mean that the audio clip is playing with no sound. Simply means we set the volume to 0)
    }

    public void HandlePaddleAudio(string surface)
    {
        if (surface == "PlayerPaddle" || surface == "ComputerPaddle")
        {
            clip = collisionClips[0];
            collisionSource.PlayOneShot(clip);
            
        }
        else if (surface == "TopBotWall")
        {
            clip = collisionClips[1];
            collisionSource.PlayOneShot(clip);
            
        }
        else if (surface == "LeftRightWall")
        {
            clip = collisionClips[2];
            collisionSource.PlayOneShot(clip);
            
        }
    }


    public void HandleFireSound(float magnitude)
    {
        if (!fireSource.isPlaying)
        {
            fireSource.Play();
        }

        // Adjust the volume based on the magnitude, for example:
        // (This is a simple mapping, you might want to create a more complex mapping function)
        fireSource.volume = Mathf.InverseLerp(6f, 10f, magnitude);
    }

    public void StopFireSound()
    {
        if (fireSource.isPlaying)
        {
            fireSource.Stop();
        }
    }
}
