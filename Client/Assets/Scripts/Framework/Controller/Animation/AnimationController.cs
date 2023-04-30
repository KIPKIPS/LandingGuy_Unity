// author:KIPKIPS
// date:2023.04.29 17:03
// describe:
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework.Controller {
    public class AnimationController : MonoBehaviour {
        [SerializeField] private Animator _animator;
        private AnimationMixerPlayable _mixer;
        private PlayableGraph _graph;
        private bool _isFirstPlay = true;
        private AnimationClipPlayable clipPlayable1;
        private AnimationClipPlayable clipPlayable2;
        private bool currentIs1;
        private Coroutine transitionCoroutine;
        public void Init() {
            _graph = PlayableGraph.Create("AnimationController");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime); //设置时间模式
            _mixer = AnimationMixerPlayable.Create(_graph, 2); //创建混合器
            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(_graph, "Animation", _animator); //创建Output
            playableOutput.SetSourcePlayable(_mixer);
        }
        public void PlayAnimation(AnimationClip clip, float fixedTime = 0.25f) {
            if (_isFirstPlay) {
                clipPlayable1 = AnimationClipPlayable.Create(_graph, clip);
                _graph.Connect(clipPlayable1, 0, _mixer, 0);
                _mixer.SetInputWeight(0, 1);
                _isFirstPlay = false;
                currentIs1 = true;
            } else {
                if (currentIs1) { //1-> 2
                    _graph.Disconnect(_mixer, 1); //解除2
                    clipPlayable2 = AnimationClipPlayable.Create(_graph, clip);
                    _graph.Connect(clipPlayable2, 0, _mixer, 1);
                } else {
                    _graph.Disconnect(_mixer, 0); //解除1
                    clipPlayable1 = AnimationClipPlayable.Create(_graph, clip);
                    _graph.Connect(clipPlayable1, 0, _mixer, 0);
                }
                if (transitionCoroutine != null) {
                    StopCoroutine(transitionCoroutine);
                }
                transitionCoroutine = StartCoroutine(TransitionAnimation(fixedTime, currentIs1));
                currentIs1 = !currentIs1;
                //todo:过渡
            }
            if (!_graph.IsPlaying()) {
                _graph.Play();
            }
        }
        IEnumerator TransitionAnimation(float fixedTime, bool is1) {
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