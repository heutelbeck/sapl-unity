using System.Threading.Tasks;
using UnityEngine;

namespace FTK.UIToolkit.Util
{
    [RequireComponent(typeof(AudioSource))]
    public class LazyAudioSource : MonoBehaviour
    {
        [Min(0f)]
        [SerializeField]
        private float delaySeconds = 0.5f;

        private bool playAudio = true;

        public void PlayOneShot(AudioClip audioClip)
        {
            if (playAudio)
            {
                playAudio = false;
                var delayMillis = (int)(1000f * delaySeconds);
                GetComponent<AudioSource>()?.PlayOneShot(audioClip);
                Task.Run(async () => { await Task.Delay(delayMillis); playAudio = true; });
            }
        }
    }
}