using MatteoBenaissaLibrary.SingletonClassBase;
using UnityEngine;

namespace MatteoBenaissaLibrary.AudioManager
{
    public class SoundManager : Singleton<SoundManager>
    {
        [field:SerializeField] public AudioClip MenuMusic { get; private set; }
        [field:SerializeField] public AudioClip GameplayMusic { get; private set; }

        public float GlobalVolume
        {
            get => _globalVolume;
            set
            {
                _globalVolume = value;
                _musicSource.volume = _baseMusicVolume * _globalVolume;
            }
        }
        
        [SerializeField] private AudioSource _musicSource;

        private float _globalVolume;
        private float _baseMusicVolume;
        
        protected override void InternalAwake()
        {
            GlobalVolume = 1f;
            PlayMusic(MenuMusic);
        }

        public void PlaySound(SoundEnum sound, float volume = 0.1f)
        {
            volume *= GlobalVolume;
            
            GameObject soundObject = new GameObject($"Sound {sound.ToString()}");
            soundObject.transform.parent = null;
            soundObject.transform.position = Vector3.zero;

            AudioSource source = soundObject.AddComponent<AudioSource>();
            AudioClip clip = SoundResourceManager.Instance.GetAudioClip(sound);
            source.clip = clip;
            source.volume = volume;
            source.Play();
            
            Object.Destroy(soundObject, clip.length);
        }

        public void PlayMusic(AudioClip music, float volume = 0.05f)
        {
            _baseMusicVolume = volume;

            _musicSource.clip = music;
            _musicSource.loop = true;
            _musicSource.volume = _baseMusicVolume * _globalVolume;
            _musicSource.Play();
        }
    }
}