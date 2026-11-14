using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace RacingGame.Audio
{
    public enum VolumeChannel
    {
        Master,
        Music,
        SFX
    }
    

    [DefaultExecutionOrder(-1000)]
    public class AudioManager : MonoBehaviour
    {
        [System.Serializable]
        private struct SceneMusicEntry
        {
            public string sceneName;
            public AudioClip[] tracks;
            public bool shuffle;
        }
        [Header("Default Volume")]
        [SerializeField, Range(0f, 1f)] private float defaultMasterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float defaultMusicVolume = 0.5f;
        [SerializeField, Range(0f, 1f)] private float defaultSfxVolume = 0.8f;
        

        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSourcePrefab;
        
        [SerializeField] private SceneMusicEntry[] sceneMusic;
        
        private AudioClip[] _playlist;
        private bool _shuffle;
        private int _trackIndex = -1;
        
        private const float MinVolume = 0.0001f;
        public static AudioManager Instance { get; private set; }
        

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (Instance != this) return;

            foreach (VolumeChannel channel in System.Enum.GetValues(typeof(VolumeChannel)))
                ApplyVolume(channel, GetVolume(channel));

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        
        


        private void Update()
        {
            if (Instance != this) return;
            if (_playlist == null || _playlist.Length == 0) return;

            if (!musicSource.isPlaying)
                PlayNextTrack();
        }

        public void PlayPlaylist(AudioClip[] tracks, bool shuffle)
        {
            if (tracks == null || tracks.Length == 0) return;
            if (_playlist == tracks && musicSource.isPlaying) return; 

            _playlist = tracks;
            _shuffle = shuffle;
            _trackIndex = -1;
            PlayNextTrack();
        }

        public void PlayMusic(AudioClip clip)
        {
            if (!clip) return;

            _playlist = null;
            if (musicSource.clip == clip && musicSource.isPlaying) return;

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void StopMusic()
        {
            _playlist = null;
            musicSource.Stop();
        }

        private void PlayNextTrack()
        {
            if (_shuffle && _playlist.Length > 1)
            {
                int next;
                do next = Random.Range(0, _playlist.Length);
                while (next == _trackIndex);
                _trackIndex = next;
            }
            else
            {
                _trackIndex = (_trackIndex + 1) % _playlist.Length;
            }

            musicSource.clip = _playlist[_trackIndex];
            musicSource.loop = false;
            musicSource.Play();
        }

        
        
        public void PlaySfx3D(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            AudioSource source = CreateSfxSource(clip, volume, pitch);
            if (!source) return;

            source.transform.position = position;
            source.Play();
        }

        public void PlaySfx2D(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            AudioSource source = CreateSfxSource(clip, volume, pitch);
            if (!source) return;

            source.spatialBlend = 0f;
            source.Play();
        }

        private AudioSource CreateSfxSource(AudioClip clip, float volume, float pitch)
        {
            if (!clip) return null;

            AudioSource source = Instantiate(sfxSourcePrefab, transform);
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;

            Destroy(source.gameObject, clip.length / Mathf.Max(Mathf.Abs(pitch), 0.01f));
            return source;
        }

        
        public float GetVolume(VolumeChannel channel)
        {
            return PlayerPrefs.GetFloat(GetPrefsKey(channel), GetDefaultVolume(channel));
        }

        private float GetDefaultVolume(VolumeChannel channel)
        {
            return channel switch
            {
                VolumeChannel.Master => defaultMasterVolume,
                VolumeChannel.Music => defaultMusicVolume,
                VolumeChannel.SFX => defaultSfxVolume,
                _ => 1f
            };
        }

        public void SetVolume(VolumeChannel channel, float volume)
        {
            volume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(GetPrefsKey(channel), volume);
            ApplyVolume(channel, volume);
        }

        private void ApplyVolume(VolumeChannel channel, float volume)
        {
            float decibels = Mathf.Log10(Mathf.Max(volume, MinVolume)) * 20f;
            mixer.SetFloat(GetMixerParameter(channel), decibels);
        }

        private static string GetMixerParameter(VolumeChannel channel) => channel + "Volume";
        private static string GetPrefsKey(VolumeChannel channel) => "Volume_" + channel;
        
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Instance != this) return;

            foreach (var entry in sceneMusic)
            {
                if (entry.sceneName.Trim() == scene.name)
                {
                    PlayPlaylist(entry.tracks, entry.shuffle);
                    return;
                }
            }
        }
    }
}