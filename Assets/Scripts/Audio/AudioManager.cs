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
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSourcePrefab;
        
        
        [System.Serializable]
        private struct SceneMusicEntry
        {
            public string sceneName;
            public AudioClip music;
        }

        [SerializeField] private SceneMusicEntry[] sceneMusic;


        private const float MinVolume = 0.0001f;

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


        public void PlayMusic(AudioClip clip)
        {
            Debug.Log($"[Audio] PlayMusic: {(clip ? clip.name : "null")}");
            if (!clip) return;
            if (musicSource.clip == clip && musicSource.isPlaying) return; 

            musicSource.clip = clip;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
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
            return PlayerPrefs.GetFloat(GetPrefsKey(channel), 1f);
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
            Debug.Log($"[Audio] OnSceneLoaded: '{scene.name}', записей: {sceneMusic.Length}");
            if (Instance != this) return;

            foreach (var entry in sceneMusic)
            {
                Debug.Log($"[Audio] сравниваю с '{entry.sceneName}'");
                if (entry.sceneName.Trim() == scene.name)
                {
                    PlayMusic(entry.music);
                    return;
                }
            }

            Debug.Log("[Audio] музыка для сцены не найдена");
        }
    }
}