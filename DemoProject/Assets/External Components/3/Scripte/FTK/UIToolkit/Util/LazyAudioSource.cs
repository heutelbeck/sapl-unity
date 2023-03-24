/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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