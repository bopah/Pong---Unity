using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    [SerializeField] private AudioSource collisionSource;
    [SerializeField] List<AudioClip> collisionClips;
    AudioClip clip;

    public void HandlePaddleAudio(string surface)
    {
        if (surface == "PlayerPaddle" || surface == "ComputerPaddle")
        {
            clip = collisionClips[0];
            collisionSource.PlayOneShot(clip);
            Debug.Log("paddle sound, go!");
        }
        else if (surface == "TopBotWall")
        {
            clip = collisionClips[1];
            collisionSource.PlayOneShot(clip);
            Debug.Log("top/bot wall sound, go!");
        }
        else if (surface == "LeftRightWall")
        {
            clip = collisionClips[2];
            collisionSource.PlayOneShot(clip);
            Debug.Log("left/right wall sound, go!");
        }
        else
        {
            Debug.Log("hello, no sound it seems :(");
        }
    }
}
