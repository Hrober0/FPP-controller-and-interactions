using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class FootstepsSounds : MonoBehaviour
    {
        [SerializeField] [Range(0f, 1f)] private float stepsVolume = 0.5f;
        [SerializeField] private float distance = 4;
        [SerializeField] private float stepsDelay = 0.05f;
        [SerializeField] private AudioClip[] footsteps;

        private AudioSource _footstepSource;

        private const float updateTime = 0.05f;
        private Vector3 _lastPosition;
        private float _currSpeed;

        private void Start()
        {
            _footstepSource = gameObject.AddComponent<AudioSource>();
            _footstepSource.playOnAwake = false;
            _footstepSource.spatialBlend = 1;
            _footstepSource.minDistance = distance;

            InvokeRepeating(nameof(CheckSpeed), updateTime, updateTime);
        }

        private void OnEnable()
        {
            _lastPosition = transform.position;
            _currSpeed = 0f;
            StartCoroutine(Footstep());
        }

        private void CheckSpeed()
        {
            _currSpeed = (_lastPosition - transform.position).magnitude / updateTime;
            _lastPosition = transform.position;
        }

        private IEnumerator Footstep()
        {
            while (gameObject.activeSelf)
            {
                if (_currSpeed > 1f)
                {
                    _footstepSource.clip = footsteps[Random.Range(0, footsteps.Length)];
                    _footstepSource.Play();
                    _footstepSource.volume = stepsVolume * Random.Range(0.8f, 1f);
                    _footstepSource.pitch = Random.Range(0.9f, 1.1f);
                }
                yield return new WaitForSeconds(stepsDelay);
            }
        }
    }
}
