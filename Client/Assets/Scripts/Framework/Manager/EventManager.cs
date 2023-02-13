﻿// author:KIPKIPS
// date:2023.02.04 14:36
// describe:事件机制
using System.Collections.Generic;
using System;
using Framework.Pool;
using Framework.Singleton;
namespace Framework.Manager {
    public class EventManager : Singleton<EventManager> {
        private static readonly string _logTag = "EventManager";
        private static readonly SimplePool<EventEntity> _eventEntityPool = new ();
        //Dictionary<EventType, EventEntity>
        //这里不使用EventType作为键值的原因是枚举没有时限IEquatable接口,字典使用Enum为键时会触发装箱
        private static readonly Dictionary<int, EventEntity> _eventDict = new ();
        public void Launch() {
            Utils.Log(_logTag,"event manager is start");
        }
        
        /// <summary>
        /// 事件注册器
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">有参事件触发回调</param>
        public void Register(EventType type, Action<dynamic> callback) {
            Utils.Log(_logTag,$"register => EventType.{type.ToString()}");
            int type2Int = (int) type;
            if (!_eventDict.ContainsKey(type2Int)) {
                EventEntity e = _eventEntityPool.Allocate();
                _eventDict[type2Int] = e;
                e.AddCallback(callback);
            } else {
                _eventDict[type2Int].AddCallback(callback);
            }
        }
        /// <summary>
        /// 事件注册器
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">无参事件触发回调</param>
        public void Register(EventType type, Action callback) {
            Utils.Log(_logTag,$"register => EventType.{type.ToString()}");
            int type2Int = (int) type;
            if (!_eventDict.ContainsKey(type2Int)) {
                EventEntity e = _eventEntityPool.Allocate();
                _eventDict[type2Int] = e;
                e.AddCallback(callback);
            } else {
                _eventDict[type2Int].AddCallback(callback);
            }
        }
        
        /// <summary>
        /// 事件移除
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">有参事件触发回调</param>
        public void Remove(EventType type, Action<dynamic> callback) {
            Utils.Log(_logTag,$"remove => EventType.{type.ToString()}");
            int type2Int = (int) type;
            if (_eventDict.ContainsKey(type2Int)) {
                _eventDict[type2Int].RemoveCallback(callback);
                if (_eventDict[type2Int].CanRemove) {
                    _eventEntityPool.Recycle(_eventDict[type2Int]);
                    // EventQueue.Remove(type);
                }
            }
        }
        
        /// <summary>
        /// 事件移除
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">无参事件触发回调</param>
        public void Remove(EventType type, Action callback) {
            Utils.Log(_logTag,$"remove => EventType.{type.ToString()}");
            int type2Int = (int) type;
            if (_eventDict.ContainsKey(type2Int)) {
                _eventDict[type2Int].RemoveCallback(callback);
                if (_eventDict[type2Int].CanRemove) {
                    // EventQueue.Remove(type);
                    _eventEntityPool.Recycle(_eventDict[type2Int]);
                }
            }
        }
        
        /// <summary>
        /// 事件派发
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="data">事件传递数据</param>
        public void Dispatch(EventType type, dynamic data = null) {
            Utils.Log(_logTag,$"dispatch => EventType.{type.ToString()}");
            int type2Int = (int) type;
            if (_eventDict != null && _eventDict.ContainsKey(type2Int)) {
                _eventDict[type2Int].Execute(data);
            }
        }
        
        /// <summary>
        /// 事件实体
        /// </summary>
        private class EventEntity : IPoolAble {
            private List<Action<dynamic>> _dynamicCallbackList;
            private List<Action> _callbackList;
            private List<Action<dynamic>> DynamicCallbackList => _dynamicCallbackList ??= new List<Action<dynamic>>();
            private List<Action> CallbackList => _callbackList ??= new List<Action>();
            internal bool CanRemove => CallbackList.Count == 0 && DynamicCallbackList.Count == 0;
            private List<Action<dynamic>> _dynamicRemoveList;
            private List<Action> _removeList;
            private List<Action<dynamic>> DynamicRemoveList => _dynamicRemoveList ??= new List<Action<dynamic>>();
            private List<Action> RemoveList => _removeList ??= new List<Action>();
            public void OnRecycled() {
                DynamicCallbackList.Clear();
                CallbackList.Clear();
                DynamicRemoveList.Clear();
                RemoveList.Clear();
            }
            public bool IsRecycled { get; set; }
            private bool _lock;
            /// <summary>
            /// 执行事件
            /// </summary>
            /// <param name="data"></param>
            internal void Execute(dynamic data) {
                _lock = true;
                foreach (var c in DynamicCallbackList) {
                    if (!DynamicRemoveList.Contains(c)) {
                        c?.Invoke(data);
                    }
                }
                foreach (var c in CallbackList) {
                    if (!RemoveList.Contains(c)) {
                        c?.Invoke();
                    }
                }
                //true remove
                foreach (var c in DynamicRemoveList) {
                    DynamicCallbackList.Remove(c);
                }
                DynamicRemoveList.Clear();
                foreach (var c in RemoveList) {
                    CallbackList.Remove(c);
                }
                RemoveList.Clear();
                _lock = false;
            }
            /// <summary>
            /// 注册有参回调
            /// </summary>
            /// <param name="dynamicCallback">有参回调函数</param>
            internal void AddCallback(Action<dynamic> dynamicCallback) {
                if (!DynamicCallbackList.Contains(dynamicCallback)) {
                    DynamicCallbackList.Add(dynamicCallback);
                }
            }
            /// <summary>
            /// 注册无参回调
            /// </summary>
            /// <param name="callback">无参回调函数</param>
            internal void AddCallback(Action callback) {
                if (!CallbackList.Contains(callback)) {
                    CallbackList.Add(callback);
                }
            }
            /// <summary>
            /// 移除有参回调
            /// </summary>
            /// <param name="dynamicCallback">有参回调函数</param>
            internal void RemoveCallback(Action<dynamic> dynamicCallback) {
                if (_lock) {
                    DynamicRemoveList.Add(dynamicCallback);
                } else {
                    if (DynamicCallbackList.Contains(dynamicCallback)) {
                        DynamicCallbackList.Remove(dynamicCallback);
                    }
                }
            }
            /// <summary>
            /// 移除无参回调
            /// </summary>
            /// <param name="callback">无参回调函数</param>
            internal void RemoveCallback(Action callback) {
                if (_lock) {
                    RemoveList.Add(callback);
                } else {
                    if (CallbackList.Contains(callback)) {
                        CallbackList.Remove(callback);
                    }
                }
            }
        }
    }
}