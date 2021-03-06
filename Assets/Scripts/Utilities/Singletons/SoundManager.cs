namespace redd096
{
    using UnityEngine;
    using System.Collections;

    [System.Serializable]
    public struct AudioStruct
    {
        public AudioClip audioClip;
        [Range(0f, 1f)] public float volume;
    }

    [AddComponentMenu("redd096/Singletons/Sound Manager")]
    [DefaultExecutionOrder(-2)]
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("Instantiate sound at point")]
        [SerializeField] AudioSource audioPrefab = default;

        private AudioSource backgroundAudioSource;
        AudioSource BackgroundAudioSource
        {
            get
            {
                //create audio source if null
                if (backgroundAudioSource == null)
                    backgroundAudioSource = gameObject.AddComponent<AudioSource>();

                //return audio source
                return backgroundAudioSource;
            }
        }

        private Transform soundsParent;
        Transform SoundsParent
        {
            get
            {
                if (soundsParent == null)
                    soundsParent = new GameObject("Sounds Parent").transform;

                return soundsParent;
            }
        }

        #region static Play

        /// <summary>
        /// Start audio clip. Can set volume and loop
        /// </summary>
        public static void Play(AudioSource audioSource, AudioClip clip, bool forceReplay, float volume = 1, bool loop = false)
        {
            //be sure to have audio source
            if (audioSource == null)
                return;

            //change only if different clip (so we can have same music in different scenes without stop)
            if (forceReplay || audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.loop = loop;

                audioSource.Play();
            }
        }

        #endregion

        /// <summary>
        /// Start audio clip for background. Can set volume and loop
        /// </summary>
        public void PlayBackgroundMusic(AudioClip clip, float volume = 1, bool loop = false)
        {
            //start music from this audio source
            Play(BackgroundAudioSource, clip, false, volume, loop);
        }

        /// <summary>
        /// Start audio clip at point. Can set volume
        /// </summary>
        public void Play(Pooling<AudioSource> pool, AudioClip clip, Vector3 position, float volume = 1)
        {
            if (clip == null)
                return;

            //instantiate (if didn't find deactivated, take first one in the pool)
            AudioSource audioSource = pool.Instantiate(audioPrefab);
            if (audioSource == null && pool.PooledObjects.Count > 0)
                audioSource = pool.PooledObjects[0];

            //if still null, return
            if (audioSource == null)
                return;

            //set position, rotation and parent
            audioSource.transform.position = position;
            audioSource.transform.SetParent(SoundsParent);

            //play and start coroutine to deactivate
            Play(audioSource, clip, true, volume);
            StartCoroutine(DeactiveSoundAtPointCoroutine(audioSource));
        }

        IEnumerator DeactiveSoundAtPointCoroutine(AudioSource audioToDeactivate)
        {
            //wait to end the clip
            if (audioToDeactivate)
                yield return new WaitForSeconds(audioToDeactivate.clip.length);

            //and deactive
            if(audioToDeactivate)
                audioToDeactivate.gameObject.SetActive(false);
        }
    }
}