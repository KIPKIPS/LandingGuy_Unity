// author:KIPKIPS
// date:2023.02.04 14:36
// describe:事件机制
using System.Collections.Generic;
using System;
using System.Linq;
using Framework.Pool;
using Framework.Singleton;
namespace Framework.Manager {
    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable MemberCanBeMadeStatic.Local
    // ReSharper disable MemberCanBeMadeStatic.Global
    public class EventManager : Singleton<EventManager> {
        private const string LOGTag = "EventManager";
        private static readonly SimplePool<EventEntity> EventEntityPool = new ();
        //Dictionary<EventType, EventEntity>
        //这里不使用EventType作为键值的原因是枚举没有时限IEquatable接口,字典使用Enum为键时会触发装箱
        private static readonly Dictionary<int, EventEntity> EventDict = new ();
        public void Launch() {
            Utils.Log(LOGTag,"event manager is start");
        }
        
        /// <summary>
        /// 事件注册器
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">有参事件触发回调</param>
        public void Register(EventType type, Action<dynamic> callback) {
            Utils.Log(LOGTag,$"register => EventType.{type.ToString()}");
            var type2Int = (int) type;
            if (!EventDict.ContainsKey(type2Int)) {
                var e = EventEntityPool.Allocate();
                EventDict[type2Int] = e;
                e.AddCallback(callback);
            } else {
                EventDict[type2Int].AddCallback(callback);
            }
        }
        /// <summary>
        /// 事件注册器
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">无参事件触发回调</param>
        public void Register(EventType type, Action callback) {
            Utils.Log(LOGTag,$"register => EventType.{type.ToString()}");
            var type2Int = (int) type;
            if (!EventDict.ContainsKey(type2Int)) {
                var e = EventEntityPool.Allocate();
                EventDict[type2Int] = e;
                e.AddCallback(callback);
            } else {
                EventDict[type2Int].AddCallback(callback);
            }
        }
        
        /// <summary>
        /// 事件移除
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">有参事件触发回调</param>
        public void Remove(EventType type, Action<dynamic> callback) {
            Utils.Log(LOGTag,$"remove => EventType.{type.ToString()}");
            var type2Int = (int) type;
            if (!EventDict.ContainsKey(type2Int)) return;
            EventDict[type2Int].RemoveCallback(callback);
            if (EventDict[type2Int].CanRemove) {
                EventEntityPool.Recycle(EventDict[type2Int]);
                // EventQueue.Remove(type);
            }
        }
        
        /// <summary>
        /// 事件移除
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="callback">无参事件触发回调</param>
        public void Remove(EventType type, Action callback) {
            Utils.Log(LOGTag,$"remove => EventType.{type.ToString()}");
            var type2Int = (int) type;
            if (!EventDict.ContainsKey(type2Int)) return;
            EventDict[type2Int].RemoveCallback(callback);
            if (EventDict[type2Int].CanRemove) {
                // EventQueue.Remove(type);
                EventEntityPool.Recycle(EventDict[type2Int]);
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 事件派发
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <param name="data">事件传递数据</param>
        public void Dispatch(EventType type, dynamic data = null) {
            Utils.Log(LOGTag,$"dispatch => EventType.{type.ToString()}");
            var type2Int = (int) type;
            if (EventDict != null && EventDict.ContainsKey(type2Int)) {
                EventDict[type2Int].Execute(data);
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
                foreach (var c in DynamicCallbackList.Where(c => !DynamicRemoveList.Contains(c))) {
                    c?.Invoke(data);
                }
                foreach (var c in CallbackList.Where(c => !RemoveList.Contains(c))) {
                    c?.Invoke();
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