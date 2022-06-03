using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private AudioClip musicClip;
        [SerializeField] [Range(0f, 1f)] private float globalMiusicVolume = 1f;

        [Header("Sounds")]
        [SerializeField] private float muteAutioAtStrt = 1f;
        [SerializeField] [Range(0f, 1f)] private float globalSoundsVolume = 1f;


        public static AudioManager instance;
        private void Awake()
        {
            if (instance == null)
                instance = this;

            SetSoundsValues();
            SetMusicValues();
        }

        private void Update()
        {
            UpdateSoundsTime();
        }

        #region -sounds-

        private readonly List<AudioObject> usingAudioObjects = new List<AudioObject>();
        private readonly List<AudioObject> availableAudioObject = new List<AudioObject>();

        private int soundID = 0;
        private Transform activeSoundsT;
        private Transform availableSoundsT;

        private float? defaultGlobalSoundsVolume = null;

        private void SetSoundsValues()
        {
            activeSoundsT = new GameObject().transform;
            activeSoundsT.name = "ActiveSounds";
            activeSoundsT.parent = transform;

            availableSoundsT = new GameObject().transform;
            availableSoundsT.name = "AvailableSounds";
            availableSoundsT.parent = transform;

            if (muteAutioAtStrt > 0)
            {
                defaultGlobalSoundsVolume = globalSoundsVolume;
                globalSoundsVolume = 0f;
                Invoke(nameof(TrunOnSounds), muteAutioAtStrt);
            }
        }

        private void TrunOnSounds()
        {
            globalSoundsVolume = defaultGlobalSoundsVolume.Value;
            defaultGlobalSoundsVolume = null;
            UpdateVolumeOfAllAudioObject();
        }

        private void UpdateSoundsTime()
        {
            int l = usingAudioObjects.Count;
            for (int i = 0; i < l; i++)
            {
                AudioObject audioObject = usingAudioObjects[i];
                audioObject.timeToEnd -= Time.deltaTime;
                if (audioObject.timeToEnd < 0)
                {
                    StopAudioObject(audioObject);
                    i--;
                    l--;
                }
            }
        }

        public void SetSoundsVolume(float value)
        {
            if (defaultGlobalSoundsVolume != null)
                defaultGlobalSoundsVolume = value;
            else
            {
                globalSoundsVolume = value;
                UpdateVolumeOfAllAudioObject();
            }
        }

        public int PlaySound(Sound sound)
        {
            if (!sound.IsGood)
            {
                Debug.LogWarning("The sound was created incorrectly");
                return 0;
            }

            soundID++;

            if (availableAudioObject.Count == 0)
                CreateNewAudioObject();


            AudioObject audioObject = availableAudioObject[0];
            availableAudioObject.RemoveAt(0);

            audioObject.gameObject.SetActive(true);
            if (sound.paerent != null)
            {
                audioObject.gameObject.transform.parent = sound.paerent;
                audioObject.gameObject.transform.localPosition = sound.position;
            }
            else
            {
                audioObject.gameObject.transform.parent = activeSoundsT;
                audioObject.gameObject.transform.position = sound.position;
            }
            

            audioObject.soundID = soundID;

            audioObject.timeToEnd = sound.loop ? sound.time : sound.clip.length;

            audioObject.givenVolume = sound.volume;

            audioObject.source.clip = sound.clip;
            audioObject.source.volume = sound.volume * globalSoundsVolume;
            audioObject.source.minDistance = sound.distanceScale;
            audioObject.source.loop = sound.loop;
            audioObject.source.spatialBlend = sound.spatialBlend;
            audioObject.source.Play();

            usingAudioObjects.Add(audioObject);

            return soundID;
        }

        public void StopSound(int soundID)
        {
            AudioObject audioObject;
            for (int i = usingAudioObjects.Count - 1; i >= 0; i--)
            {
                audioObject = usingAudioObjects[i];
                if (audioObject.soundID == soundID)
                {
                    StopAudioObject(audioObject);
                    return;
                }
            }
        }
        public void StopSound(AudioClip clip)
        {
            AudioObject audioObject;
            for (int i = usingAudioObjects.Count - 1; i >= 0; i--)
            {
                audioObject = usingAudioObjects[i];
                if (audioObject.source.clip == clip)
                {
                    StopAudioObject(audioObject);
                }
            }
        }
        public void StopSound(AudioClip clip, GameObject gameObject)
        {
            AudioObject audioObject;
            Transform parent = gameObject.transform;
            for (int i = usingAudioObjects.Count - 1; i >= 0; i--)
            {
                audioObject = usingAudioObjects[i];
                if (audioObject.source.clip == clip || audioObject.gameObject.transform.parent == parent)
                {
                    StopAudioObject(audioObject);
                }
            }
        }

        private void CreateNewAudioObject()
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "AudioObject";
            gameObject.transform.parent = availableSoundsT;
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.rolloffMode = AudioRolloffMode.Logarithmic;

            availableAudioObject.Add(new AudioObject(source, gameObject, 0));
        }

        private void StopAudioObject(AudioObject audioObject)
        {
            if (audioObject.gameObject == null)
            {
                usingAudioObjects.Remove(audioObject);
                return;
            }

            audioObject.gameObject.SetActive(false);
            audioObject.gameObject.transform.parent = availableSoundsT;
            audioObject.source.Stop();
            usingAudioObjects.Remove(audioObject);
            availableAudioObject.Add(audioObject);
        }


        private void UpdateVolumeOfAllAudioObject()
        {
            foreach (AudioObject audioObject in usingAudioObjects)
                audioObject.source.volume = audioObject.givenVolume * globalSoundsVolume;
        }

        private class AudioObject
        {
            public float givenVolume;
            public AudioSource source;
            public GameObject gameObject;
            public float timeToEnd;
            public int soundID;

            public AudioObject(AudioSource source, GameObject sObject, float timeToEnd)
            {
                this.source = source;
                this.gameObject = sObject;
                this.timeToEnd = timeToEnd;
            }
        }

        #endregion

        #region -music-

        private AudioSource musicSource;

        private void SetMusicValues()
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.spatialBlend = 0;
            musicSource.playOnAwake = false;
            musicSource.volume = globalMiusicVolume;
            musicSource.clip = musicClip;
            if (musicClip != null)
                musicSource.Play();
        }

        public void SetMiusicVolume(float value)
        {
            globalMiusicVolume = value;
            musicSource.volume = globalMiusicVolume;
        }

        #endregion
    }

    public class Sound
    {
        public bool IsGood { get; private set; }

        public AudioClip clip;
        public float volume;
        public float spatialBlend;
        public float distanceScale;

        public bool loop = false;
        public float time = -1;

        public Transform paerent = null;
        public Vector3 position = Vector3.zero;
        
        public Sound GlobalSound(AudioClip clip, float volume)
        {
            this.clip = clip;
            this.volume = volume;
            this.spatialBlend = 0;
            this.distanceScale = 1;

            IsGood = true;
            return this;
        }

        public Sound PositionSound(AudioClip clip, float volume, float distanceScale, Vector3 position)
        {
            this.clip = clip;
            this.volume = volume;
            this.spatialBlend = 1;
            this.distanceScale = distanceScale;
            this.position = position;

            IsGood = true;
            return this;
        }

        public Sound ObjectSound(AudioClip clip, float volume, float distanceScale, GameObject obj)
        {
            this.clip = clip;
            this.volume = volume;
            this.spatialBlend = 1;
            this.distanceScale = distanceScale;
            this.paerent = obj.transform;

            IsGood = true;
            return this;
        }

        public Sound Loop(float time)
        {
            this.loop = true;
            this.time = time;
            return this;
        }
    }
}
