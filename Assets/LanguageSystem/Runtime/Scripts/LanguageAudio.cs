using UnityEngine;
using System.Collections.Generic;
using Univrse.Studio.AudioPrioritySystem;
using UnivrseSdk.Services;

namespace Univrse.Studio.LanguageSystemV2
{
    public class LanguageAudio : AudioLauncherBase
    {
        [SerializeField] private bool _playOnAwake;
        [SerializeField] private LanguageAudioObject _languages;
        [SerializeField] private AudioSource ac;

        [SerializeField] private List<LanguageAudioObject> _clips;
        private ILanguageService _languageService;

        private int currentClipIndex;


        private void OnEnable()
        {
            if(_playOnAwake) ChangeClip();
        }

        private AudioClip GetClipByLanguage()
        {
            LanguageAudioData data = _languages.data.Find(x => x.language == _languageService.CurrentLanguage);
            return data == null ? null : data.clip;
        }

        public void ChangeClip()
        {
            if(_languageService == null)
            {
                _languageService = ServiceLocator.Instance.GetService<ILanguageService>();
            }
            ac.clip = GetClipByLanguage();

            ac.Play();
        }
    }
}
