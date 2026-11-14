using UnityEngine;

namespace RacingGame.Audio
{
    [System.Serializable]
    public class SoundEffect
    {
        [SerializeField] private AudioClip[] clips;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField] private Vector2 pitchRange = new(0.95f, 1.05f);
        
        public void Play(Vector3 position, bool isPlayer)
        {
            if (isPlayer) Play2D();
            else Play(position);
        }

        public void Play(Vector3 position)
        {
            if (!TryGetClip(out AudioClip clip)) return;
            AudioManager.Instance.PlaySfx3D(clip, position, volume, RandomPitch());
        }

        public void Play2D()
        {
            if (!TryGetClip(out AudioClip clip)) return;
            AudioManager.Instance.PlaySfx2D(clip, volume, RandomPitch());
        }

        private bool TryGetClip(out AudioClip clip)
        {
            clip = null;
            if (clips == null || clips.Length == 0 || !AudioManager.Instance) return false;

            clip = clips[Random.Range(0, clips.Length)];
            return clip;
        }

        private float RandomPitch() => Random.Range(pitchRange.x, pitchRange.y);
    }
}