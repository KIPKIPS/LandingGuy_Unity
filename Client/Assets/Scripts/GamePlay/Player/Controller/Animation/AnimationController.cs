// author:KIPKIPS
// date:2023.04.29 17:03
// describe:
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GamePlay.Player {
    public class AnimationController : MonoBehaviour {
        [SerializeField] private Animator _animator;
        private AnimationMixerPlayable _mixer;
        private PlayableGraph _graph;
        private bool _isInit = true;
        private AnimationClipPlayable currentClipPlayable;
        private AnimationClipPlayable targetClipPlayable;
        private bool isCurrent;
        private Coroutine transitionCoroutine;
        public void Init() {
            _graph = PlayableGraph.Create("AnimationController");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime); //设置时间模式
            _mixer = AnimationMixerPlayable.Create(_graph, 2); //创建混合器
            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(_graph, "Animation", _animator); //创建Output
            playableOutput.SetSourcePlayable(_mixer);
        }
        public void PlayAnimation(AnimationClip clip, float fixedTime = 0.25f) {
            if (_isInit) {
                currentClipPlayable = AnimationClipPlayable.Create(_graph, clip);
                _graph.Connect(currentClipPlayable, 0, _mixer, 0);
                _mixer.SetInputWeight(0, 1);
                _isInit = false;
                isCurrent = true;
            } else {
                if (isCurrent) { //1-> 2
                    _graph.Disconnect(_mixer, 1); //解除2
                    targetClipPlayable = AnimationClipPlayable.Create(_graph, clip);
                    _graph.Connect(targetClipPlayable, 0, _mixer, 1);
                } else {
                    _graph.Disconnect(_mixer, 0); //解除1
                    currentClipPlayable = AnimationClipPlayable.Create(_graph, clip);
                    _graph.Connect(currentClipPlayable, 0, _mixer, 0);
                }
                if (transitionCoroutine != null) {
                    StopCoroutine(transitionCoroutine);
                }
                transitionCoroutine = StartCoroutine(TransitionAnimation(fixedTime, isCurrent));
                isCurrent = !isCurrent;
            }
            if (!_graph.IsPlaying()) {
                _graph.Play();
            }
        }
        private IEnumerator TransitionAnimation(float fixedTime, bool is1) {
            float currentWight = 1;
            float speed = 1 / fixedTime;
            while (currentWight > 0) {
                currentWight = Mathf.Clamp01(currentWight - Time.deltaTime * speed);
                if (is1) {
                    _mixer.SetInputWeight(0, currentWight);
                    _mixer.SetInputWeight(1, 1 - currentWight);
                } else {
                    _mixer.SetInputWeight(0, 1 - currentWight);
                    _mixer.SetInputWeight(1, currentWight);
                }
                yield return null;
            }
        }
        private void OnDestroy() {
            _graph.Destroy();
        }
        private void OnDisable() {
            _graph.Stop();
        }
    }
}