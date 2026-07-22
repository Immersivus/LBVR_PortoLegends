using UnityEngine;
using Univrse.Studio.AudioPrioritySystem;
using UnivrseSdk.Services;


namespace Univrse.Studio.LanguageSystemV2
{

    public class ActivateChangeClip : MonoBehaviour
    {
        [SerializeField] private LanguageAudio _languageAudio;

        void OnEnable()
        {
            if (_languageAudio == null)
            {
                Debug.LogError("LanguageAudio reference is not set in ActivateChangeClip.");
            }
            else
            {
                _languageAudio.ChangeClip();            
            }
        }
    }
}
