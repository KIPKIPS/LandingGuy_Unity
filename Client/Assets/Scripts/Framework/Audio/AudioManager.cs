// author:KIPKIPS
// date:2023.02.08 21:47
// describe:
using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

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
        
        [SerializeField] //bgm是否循环
        private bool _loopBgm;
        public bool LoopBgm => _loopBgm;

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
        }

        /// <summary>
        /// 设置静音
        /// </summary>
        void SetMute() {
            BgmAudioSource.mute = Mute;
        }
        #endregion
    }

}