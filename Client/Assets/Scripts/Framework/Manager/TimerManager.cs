// author:KIPKIPS
// date:2023.02.04 01:51
// describe:
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Pool;
using Framework.Singleton;
namespace Framework.Manager {
    public class TimerManager: Singleton<TimerManager> {
        private readonly string _logTag = "TimerManager";
        private readonly Dictionary<int, TimerEntity> _timerEntityDict = new();
        private readonly Stack<int> _removeStack = new();
        
        private readonly SimplePool<TimerEntity> _timerEntityPool = new();
        private int _allocateTimerId;
        private Coroutine triggerTimer;
        public void Launch() {
            triggerTimer = this.StartCoroutine(TriggerTimer());
            Utils.Log(_logTag,"timer manager is start");
        }
        /// <summary>
        /// 释放定时器的相关数据
        /// </summary>
        public override void Dispose() {
            this.StopCoroutine(triggerTimer);
            _timerEntityDict.Clear();
            _removeStack.Clear();
            _timerEntityPool.Clear();
            _allocateTimerId = 0;
        }
        /// <summary>
        /// 创建计时器
        /// </summary>
        /// <param name="millisecond">执行间隔毫秒</param>
        /// <param name="callback">回调函数</param>
        /// <param name="times">循环执行次数 默认1次</param>
        /// <returns>计时器对象</returns>
        public TimerEntity CreateTimer(Action<TimerEntity> callback, int millisecond, int times = 1) {
            if (_timerEntityDict.Count == 0) {
                _allocateTimerId = 0;
            }
            _allocateTimerId++;
            TimerEntity e = _timerEntityPool.Allocate();
            e.SetEntity(millisecond, callback, _allocateTimerId, times);
            _timerEntityDict[_allocateTimerId] = e;
            return e;
        }
        
        /// <summary>
        /// 销毁定时器
        /// </summary>
        /// <param name="timerId"></param>
        private void DestroyTimer(int timerId) {
            if (_timerEntityDict.ContainsKey(timerId)) {
                _timerEntityPool.Recycle(_timerEntityDict[timerId]);
            } else {
                Utils.LogError(_logTag, "Timer " + timerId + " is not exist !");
            }
        }
        
        /// <summary>
        /// 循环触发器
        /// </summary>
        /// <returns></returns>
        private IEnumerator TriggerTimer() {
            int i, count;
            while (true) {
                foreach (KeyValuePair<int, TimerEntity> e in _timerEntityDict) {
                    if (!e.Value.Check()) {
                        _removeStack.Push(e.Key);
                    }
                }
                if (_removeStack.Count > 0) {
                    count = _removeStack.Count;
                    for (i = 0; i < count; i++) {
                        int id = _removeStack.Pop();
                        if (_timerEntityDict.ContainsKey(id)) {
                            _timerEntityPool.Recycle(_timerEntityDict[id]);
                        }
                    }
                }
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }
        
        /// <summary>
        /// Timer计时器实体
        /// </summary>
        public class TimerEntity : IPoolAble {
            private Action<TimerEntity> _callback;
            private float _startTime;
            private float _gap;
            private int _id;
            private int _times;
            private int _curTimes;
            private bool isStart;
            private bool Loop => _times != 1;
 
            /// <summary>
            /// 启动定时器
            /// </summary>
            public void Start() {
                _startTime = Time.time;
                isStart = true;
            }
            /// <summary>
            /// 销毁定时器
            /// </summary>
            public void Destroy() {
                Instance.DestroyTimer(_id);
                isStart = false;
            }
            /// <summary>
            /// 回收定时器
            /// </summary>
            public void OnRecycled() {
                isStart = false;
            }
            public bool IsRecycled { get; set; }
            /// <summary>
            /// 设置定时器
            /// </summary>
            /// <param name="gap">执行间隔时间</param>
            /// <param name="callback">执行回调</param>
            /// <param name="id">定时器id</param>
            /// <param name="times">执行次数</param>
            internal void SetEntity(int gap, Action<TimerEntity> callback, int id, int times = 1) {
                _gap = (float)gap / 1000;
                _id = id;
                _callback = callback;
                _times = times;
            }
            /// <summary>
            /// 执行检测方法
            /// </summary>
            /// <returns>是否继续执行</returns>
            internal bool Check() {
                if (IsRecycled) return false;
                if (isStart) {
                    if (Loop) {
                        //无限循环
                        if (_times < 1) {
                            if (Time.time - _startTime >= _gap) {
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
                        if (Time.time - _startTime >= _gap) {
                            _startTime = Time.time; //重置
                            _curTimes++;
                            _callback?.Invoke(this);
                        }
                        return true;
                    }
                    //单次计时器
                    if (Time.time - _startTime >= _gap) {
                        _callback?.Invoke(this);
                        return false;
                    }
                }
                return true;
            }
        }
    }
}