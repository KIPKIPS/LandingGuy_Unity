// author:KIPKIPS
// date:2023.02.04 14:36
// describe:事件机制
using System.Collections.Generic;
using System;
namespace Framework {
    public class Event : Singleton<Event> {
        private static readonly string _logTag = "Event";
        private static readonly SimplePool<Entity> _eventEntityPool = new ();
        private static readonly Dictionary<EventType, Entity> _eventDict = new ();
        private Event(){
            
        }
        /// <summary>
        /// 事件注册器
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">有参事件触发回调</param>
        public static void Register(EventType type, Action<dynamic> callback) {
            Utils.Log(_logTag,$"register => EventType.{type.ToString()}");
            if (!_eventDict.ContainsKey(type)) {
                Entity e = _eventEntityPool.Allocate();
                _eventDict[type] = e;
                e.AddCallback(callback);
            } else {
                _eventDict[type].AddCallback(callback);
            }
        }
        /// <summary>
        /// 事件注册器
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">无参事件触发回调</param>
        public static void Register(EventType type, Action callback) {
            Utils.Log(_logTag,$"register => EventType.{type.ToString()}");
            if (!_eventDict.ContainsKey(type)) {
                Entity e = _eventEntityPool.Allocate();
                _eventDict[type] = e;
                e.AddCallback(callback);
            } else {
                _eventDict[type].AddCallback(callback);
            }
        }
        
        /// <summary>
        /// 事件移除
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">有参事件触发回调</param>
        public static void Remove(EventType type, Action<dynamic> callback) {
            Utils.Log(_logTag,$"remove => EventType.{type.ToString()}");
            if (_eventDict.ContainsKey(type)) {
                _eventDict[type].RemoveCallback(callback);
                if (_eventDict[type].CanRemove) {
                    _eventEntityPool.Recycle(_eventDict[type]);
                    // EventQueue.Remove(type);
                }
            }
        }
        
        /// <summary>
        /// 事件移除
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">无参事件触发回调</param>
        public static void Remove(EventType type, Action callback) {
            Utils.Log(_logTag,$"remove => EventType.{type.ToString()}");
            if (_eventDict.ContainsKey(type)) {
                _eventDict[type].RemoveCallback(callback);
                if (_eventDict[type].CanRemove) {
                    // EventQueue.Remove(type);
                    _eventEntityPool.Recycle(_eventDict[type]);
                }
            }
        }
        
        /// <summary>
        /// 事件派发
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="data">事件传递数据</param>
        public static void Dispatch(EventType type, dynamic data = null) {
            Utils.Log(_logTag,$"dispatch => EventType.{type.ToString()}");
            if (_eventDict != null && _eventDict.ContainsKey(type)) {
                _eventDict[type].Execute(data);
            }
        }
        
        /// <summary>
        /// 事件实体
        /// </summary>
        private class Entity : IPoolAble {
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