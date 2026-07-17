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

        [SerializeField] private List<LanguageAudioObject> _clips;
        private ILanguageService _languageService;

        private int currentClipIndex;

        protected override void Start()
        {
            base.Start();
            _languageService = ServiceLocator.Instance.GetService<ILanguageService>();
            _languageService.OnCurrentLanguageChange += OnCurrentLanguageChange;
            _audioObject.SetAudioClip(GetClipByLanguage());
        }

        private void OnEnable()
        {
            if(_playOnAwake) Play();
        }

        private void OnCurrentLanguageChange(LanguageObject languageObject)
        {
            _audioObject.SetAudioClip(GetClipByLanguage());
            if (_audioObject.Playing)
            {
                _audioObject.Stop();
                PlayIgnoringLayer();
            }
        }

        private AudioClip GetClipByLanguage()
        {
            LanguageAudioData data = _languages.data.Find(x => x.language == _languageService.CurrentLanguage);
            return data == null ? null : data.clip;
        }

        public void ChangeClip()
        {
            _audioObject.SetAudioClip(GetClipByLanguage());

            currentClipIndex++;
            if (currentClipIndex >= _clips.Count)
            {
                currentClipIndex = 0;
            }
            _languages = _clips[currentClipIndex];

            Play();

        }
    }
}
