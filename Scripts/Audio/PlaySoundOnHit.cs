using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

public class PlaySoundOnHit : MonoBehaviour
{
    [SerializeField] private float minHitHit = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float soundVolume = 1f;
    [SerializeField] private float soundDistanceScale = 3f;
    [SerializeField] private AudioClip[] clips;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude < minHitHit)
            return;

        if (clips.Length == 0)
        {
            Debug.LogWarning(name + ": has no audio clips!");
            return;
        }

        if (AudioManager.instance != null)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            AudioManager.instance.PlaySound(new Sound().PositionSound(clip, soundVolume, soundDistanceScale, transform.position));
        }
    }
}