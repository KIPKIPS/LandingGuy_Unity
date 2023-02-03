// author:KIPKIPS
// date:2023.02.04 01:51
// describe:
using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Core.Singleton;
using Framework.Core.Pool;
using UnityEngine;

namespace Framework.Core.Timer {
    public class Timer : MonoSingleton<Timer> {
        /// <summary>
        /// Timer计时器实体
        /// </summary>
        public class Entity : IPoolAble {
            private Action<Entity> _callback;
            private float _startTime;
            private float _liveTime;
            private int _id;
            private int _times;
            private int _curTimes;
            private bool isStart;
            private int Id => _id;
            private bool Loop => _times != 1;
            // private Slice _timeSlice;
            public void Start() {
                _startTime = Time.time;
                isStart = true;
            }
            public void Destroy() {
                Timer.Destroy(Id);
                isStart = false;
            }
            public void OnRecycled() {
                isStart = false;
            }
            public bool IsRecycled { get; set; }
            internal void SetEntity(int liveTime, Action<Entity> func, int id, int times = 1) {
                _liveTime = (float)liveTime / 1000;
                _id = id;
                _callback = func;
                _times = times;
            }
            internal bool DoUpdate() {
                if (IsRecycled) return false;
                if (isStart) {
                    if (Loop) {
                        //无限循环
                        if (_times < 1) {
                            if (Time.time - _startTime >= _liveTime) {
                                _startTime = Time.time; //重置
                                _callback?.Invoke(this);
                            }
                            return true;
                        }
                        //循环次数耗尽
                        if (_curTimes >= _times) {
                            return false;
                        }
                        //执行循环
                        if (Time.time - _startTime >= _liveTime) {
                            _startTime = Time.time; //重置
                            _curTimes++;
                            _callback?.Invoke(this);
                        }
                        return true;
                    }
                    //单次计时器
                    if (Time.time - _startTime >= _liveTime) {
                        _callback?.Invoke(this);
                        return false;
                    }
                }
                return true;
            }
        }
        
        private static readonly string logTag = "Timer";
        private static readonly Dictionary<int, Entity> _timerEntityDict = new();
        private static readonly Stack<int> _removeStack = new();
        private static readonly SimplePool<Entity> _timerEntityPool = new();
        // private static readonly SimplePool<Slice> _timeSlicePool = new ();
        private static int _allocateTimerId;
        /// <summary>
        /// 创建计时器
        /// </summary>
        /// <param name="millisecond">执行间隔毫秒</param>
        /// <param name="callback">回调函数</param>
        /// <param name="times">循环执行次数 默认1次</param>
        /// <returns>计时器id</returns>
        public static Entity New(Action<Entity> callback, int millisecond, int times = 1) {
            _allocateTimerId++;
            Entity e = _timerEntityPool.Allocate();
            e.SetEntity(millisecond, callback, _allocateTimerId, times);
            _timerEntityDict[_allocateTimerId] = e;
            return e;
        }
        public static void Destroy(int timerId) {
            if (_timerEntityDict.ContainsKey(timerId)) {
                _timerEntityPool.Recycle(_timerEntityDict[timerId]);
            } else {
                Utils.Log(logTag, "Timer " + timerId + " is not exist !");
            }
        }
        public override void Initialize() {
            StartCoroutine(TriggerTimer());
        }
        IEnumerator TriggerTimer() {
            int i, count;
            while (true) {
                foreach (KeyValuePair<int, Entity> e in _timerEntityDict) {
                    if (!e.Value.DoUpdate()) {
                        _removeStack.Push(e.Key);
                    }
                }
                if (_removeStack.Count > 0) {
                    count = _removeStack.Count;
                    for (i = 0; i < count; i++) {
                        int id = _removeStack.Pop();
                        if (_timerEntityDict.ContainsKey(id)) {
                            // timerEventDict[id].OnRecycled();
                            // timerEventDict.Remove(id);
                            _timerEntityPool.Recycle(_timerEntityDict[id]);
                        }
                    }
                    if (_timerEntityDict.Count <= 0) {
                        StopCoroutine(TriggerTimer());
                    }
                }
                yield return null;
            }
        }
    }
}