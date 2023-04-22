// author:KIPKIPS
// date:2023.02.08 21:47
// describe:音效系统

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Framework.Pool;
using Framework.Singleton;

namespace Framework.Manager {
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable Unity.PerformanceCriticalCodeInvocation
    public class AudioManager : Singleton<AudioManager> {
        public void Launch() {
            //TODO:读取用户配置文件
            Mute = false;
            GlobalVolume = 1;
            EffectVolume = 1;
            BgmVolume = 1;
            Utils.Log(LOGTag,"the audio initialization settings are complete");
        }
        private AudioSource _bgmAudioSource;
        private const string LOGTag = "AudioManager";
        //特效音乐列表
        private readonly List<AudioSource> _effectAudioList = new();
        //特效音乐对象池
        private PrefabPool<AudioSource> _audioSourcePool;
        private PrefabPool<AudioSource> AudioSourcePool => _audioSourcePool ??= new PrefabPool<AudioSource>();
        
        private Transform _audioRoot;
        private Transform AudioRoot {
            get {
                // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
                if (_audioRoot != null) return _audioRoot;
                _audioRoot= new GameObject().transform;
                var t = _audioRoot;
                t.UDontDestroyOnLoad();
                t.position = Vector3.zero;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                t.name = "AudioRoot";
                return _audioRoot;
            }
        }
        public AudioSource BgmAudioSource {
            get {
                if (_bgmAudioSource is not null) return _bgmAudioSource;
                var t = new GameObject().transform;
                t.SetParent(AudioRoot);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                t.name = AudioType.BGM.ToString();
                _bgmAudioSource = t.UAddComponent<AudioSource>() as AudioSource;
                _bgmAudioSource.loop = true;
                _bgmAudioSource.playOnAwake = false;
                return _bgmAudioSource;
            }
        }

        #region 音量,播放控制
        //全局音量
        private float _globalVolume = 1f;
        public float GlobalVolume {
            get => _globalVolume;
            set {
                _globalVolume = value;
                UpdateGlobalVolume();
            }
        }
        
        //bgm音量
        private float _bgmVolume;
        public float BgmVolume {
            get => _bgmVolume;
            set {
                _bgmVolume = value;
                UpdateBgmVolume();
            }
        }
        
        //特效音量
        private float _effectVolume = 1;
        public float EffectVolume {
            get => _effectVolume;
            set {
                _effectVolume = value;
                UpdateEffectVolume();
            }
        }
        
        //是否静音
        private bool _mute;
        public bool Mute {
            get => _mute;
            set {
                _mute = value;
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                SetMute();
            }   
        }
        
        private bool _pause;
        public bool Pause {
            get => _pause;
            set {
                _pause = value;
                SetPause();
            }   
        }

        public void SetPause() {
            
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 更新全局音量
        /// </summary>
        private void UpdateGlobalVolume() {
            UpdateBgmVolume();
            UpdateEffectVolume();
        }
        /// <summary>
        /// 更新背景音乐音量
        /// </summary>
        private void UpdateBgmVolume() {
            BgmAudioSource.volume = BgmVolume * GlobalVolume;
        }
        /// <summary>
        /// 更新特效音乐音量
        /// </summary>
        private void UpdateEffectVolume() {
            for (var i = _effectAudioList.Count - 1; i >= 0; i--) {
                // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
                if (_effectAudioList[i] != null) {
                    SetEffectAudioPlay(_effectAudioList[i]);
                }
            }
        }

        public void SetEffectAudioPlay(AudioSource audioSource,float spatial = -1) {
            audioSource.mute = Mute;
            audioSource.volume = EffectVolume * GlobalVolume;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (spatial != -1) {
                audioSource.spatialBlend = spatial;
            }
            if (Pause) {
                audioSource.Pause();
            }
        }
        
        private void RecycleAudioPlay(AudioSource audioSource) {
            AudioSourcePool.Recycle(audioSource);
            _effectAudioList.Remove(audioSource);
        }
        
        private AudioSource GetAudioPlay(bool is3d,Vector3 position) {
            var audioSource = AudioSourcePool.Allocate();
            var t = audioSource.transform;
            t.SetParent(AudioRoot);
            t.position = position;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            if (!t.name.StartsWith($"{AudioType.EFFECT}")) {
                t.name = $"{AudioType.EFFECT}_{_effectAudioList.Count}";
            }
            audioSource.spatialBlend = is3d ? 1 : 0;
            _effectAudioList.Add(audioSource);
            return audioSource;
        }

        /// <summary>
        /// 播放特效音乐
        /// </summary>
        /// <param name="clip">音频资源</param>
        /// <param name="position">播放的位置</param>
        /// <param name="volumeScale">音量调节</param>
        /// <param name="is3d">是否3D音效</param>
        /// <param name="callback">播放完回调</param>
        /// <param name="callbackDelaySecond">回调延时</param>
        public void PlayAudio(AudioClip clip,Vector3 position,float volumeScale = 1,bool is3d = true,UnityAction callback = null,float callbackDelaySecond = 0) {
            var audioSource = GetAudioPlay(is3d,position);
            audioSource.mute = Mute;
            audioSource.PlayOneShot(clip,volumeScale);
            // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
            var clipMillisecond = clip != null ? clip.length * 1000 : 0;
            LTimer.New(e => {
                callback?.Invoke();
                RecycleAudioPlay(audioSource);
                e.Destroy();
            }, (int)(callbackDelaySecond * 1000 + clipMillisecond)).Start();
        }

        /// <summary>
        /// 设置静音
        /// </summary>
        public void SetMute() {
            UpdateEffectVolume();
            UpdateBgmVolume();
        }
        #endregion
    }

}