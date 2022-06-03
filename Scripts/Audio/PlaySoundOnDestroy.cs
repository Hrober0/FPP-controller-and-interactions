using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Objects.Sounds
{
    public class PlaySoundOnDestroy : MonoBehaviour
    {
        [SerializeField] [Range(0f, 1f)] private float soundVolume = 1f;
        [SerializeField] private float soundDistanceScale = 3f;
        [SerializeField] private AudioClip[] clips;
        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying == true && EditorApplication.isPlayingOrWillChangePlaymode == false)
                return;
#endif

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
}