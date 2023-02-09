// author:KIPKIPS
// date:2023.02.08 21:47
// describe:音效系统
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Framework {
    [MonoSingletonPath("AudioManager")]
    public class AudioManager : PersistentMonoSingleton<AudioManager> {
        [SerializeField]private AudioSource _bgmAudioSource;
        private readonly string _logTag = "AudioManager";
        public AudioSource BgmAudioSource {
            get {
                if (_bgmAudioSource is null) {
                    var trs = new GameObject().transform;
                    trs.SetParent(transform);
                    trs.localPosition = Vector3.zero;
                    trs.localRotation = quaternion.identity;
                    trs.localScale = Vector3.zero;
                    trs.name = DEF.AudioType.BGM.ToString();
                    _bgmAudioSource = trs.AddComponent<AudioSource>();
                    _bgmAudioSource.loop = true;
                    _bgmAudioSource.playOnAwake = false;
                }
                return _bgmAudioSource;
            }
        }

        #region 音量,播放控制
        [SerializeField][Range(0,1)] //全局音量
        private float _globalVolume = 1f;
        public float GlobalVolume {
            get => _globalVolume;
            set {
                _globalVolume = value;
                UpdateGlobalVolume();
            }
        }
        
        [SerializeField][Range(0,1)] //bgm音量
        private float _bgmVolume;
        public float BgmVolume {
            get => _bgmVolume;
            set {
                _bgmVolume = value;
                UpdateBgmVolume();
            }
        }
        
        [SerializeField][Range(0,1)] //特效音量
        private float _effectVolume;
        public float EffectVolume {
            get => _effectVolume;
            set {
                _effectVolume = value;
                UpdateEffectVolume();
            }
        }
        
        [SerializeField] //是否静音
        private bool _mute;
        public bool Mute {
            get => _mute;
            set {
                _mute = value;
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
        
        [SerializeField] //bgm是否循环
        private bool _loopBgm;
        public bool LoopBgm => _loopBgm;
        private Transform _effectAudioTrs;
        public Transform EffectAudioTrs {
            get {
                if (_effectAudioTrs is null) {
                    var trs = new GameObject().transform;
                    trs.SetParent(transform);
                    trs.localPosition = Vector3.zero;
                    trs.localRotation = quaternion.identity;
                    trs.localScale = Vector3.zero;
                    trs.name = DEF.AudioType.EFFECT.ToString();
                    _effectAudioTrs = trs;
                }
                return _effectAudioTrs;
            }
        }
        public List<AudioSource> effectAudioList = new();
        private PrefabPool<AudioSource> _audioSourcePool;

        private PrefabPool<AudioSource> AudioSourcePool => _audioSourcePool ??= new PrefabPool<AudioSource>(EffectAudioTrs);

        /// <summary>
        /// 更新全局音量
        /// </summary>
        void UpdateGlobalVolume() {
            UpdateBgmVolume();
            UpdateEffectVolume();
            Utils.Log(_logTag,GlobalVolume);
        }
        /// <summary>
        /// 更新背景音乐音量
        /// </summary>
        void UpdateBgmVolume() {
            BgmAudioSource.volume = BgmVolume * GlobalVolume;
            Utils.Log(_logTag,BgmAudioSource.volume);
        }
        /// <summary>
        /// 更新特效音乐音量
        /// </summary>
        void UpdateEffectVolume() {
            for (int i = effectAudioList.Count - 1; i >= 0; i--) {
                if (effectAudioList[i] != null) {
                    SetEffectAudioPlay(effectAudioList[i]);
                }
            }
        }

        public void SetEffectAudioPlay(AudioSource audioSource,float spatial = -1) {
            audioSource.mute = _mute;
            audioSource.volume = EffectVolume * GlobalVolume;
            if (spatial != -1) {
                audioSource.spatialBlend = spatial;
            }
            if (Pause) {
                audioSource.Pause();
            }
        }
        
        private void RecycleAudioPlay(AudioSource audioSource) {
            AudioSourcePool.Recycle(audioSource);
            effectAudioList.Remove(audioSource);
            audioSource.transform.SetParent(EffectAudioTrs);
        }
        
        private AudioSource GetAudioPlay(bool is3d,Transform trs) {
            AudioSource audioSource = AudioSourcePool.Allocate();
            var t = audioSource.transform;
            t.SetParent(trs);
            t.localPosition = Vector3.zero;
            t.localRotation = quaternion.identity;
            t.localScale = Vector3.zero;
            if (!t.name.StartsWith(EffectAudioTrs.name)) {
                t.name = $"{EffectAudioTrs.name}_{effectAudioList.Count}";
            }
            audioSource.spatialBlend = is3d ? 1 : 0;
            effectAudioList.Add(audioSource);
            return audioSource;
        }

        /// <summary>
        /// 播放特效音乐
        /// </summary>
        /// <param name="clip">音频资源</param>
        /// <param name="trs">播放的节点</param>
        /// <param name="volumeScale">音量调节</param>
        /// <param name="is3d">是否3D音效</param>
        /// <param name="callback">播放完回调</param>
        /// <param name="callbackDelaySecond">回调延时</param>
        public void PlayAudio(AudioClip clip,Transform trs = null,float volumeScale = 1,bool is3d = true,UnityAction callback = null,float callbackDelaySecond = 0) {
            trs ??= EffectAudioTrs;
            AudioSource audioSource = GetAudioPlay(is3d,trs);
            audioSource.PlayOneShot(clip,volumeScale);
            var clipMillisecond = clip != null ? clip.length * 1000 : 0;
            Timer.New(e => {
                callback?.Invoke();
                RecycleAudioPlay(audioSource);
                e.Destroy();
            }, (int)(callbackDelaySecond * 1000 + clipMillisecond)).Start();
        }

        /// <summary>
        /// 设置静音
        /// </summary>
        public void SetMute() {
            BgmAudioSource.mute = Mute;
        }
        #endregion
    }

}