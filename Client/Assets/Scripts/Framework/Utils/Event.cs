﻿// author:KIPKIPS
// date:2023.02.11 02:05
// describe:事件机制
using System;
using Framework.Manager;
namespace Framework {
    // ReSharper disable ClassNeverInstantiated.Global
    #region EVENT
    public enum EventType {
        GameLaunch,
        SceneLoadFinished,//场景加载完成
        SceneLoading,//场景加载中
        SceneFailure,//场景加载失败
    }

    #endregion
    public class Event {
        public static void Register(EventType type, Action<dynamic> callback) => EventManager.Instance.Register(type,callback);

        public static void Register(EventType type, Action callback) => EventManager.Instance.Register(type,callback);
        
        public static void Remove(EventType type, Action<dynamic> callback) => EventManager.Instance.Remove(type,callback);
        
        public static void Remove(EventType type, Action callback) => EventManager.Instance.Remove(type,callback);

        public static void Dispatch(EventType type, dynamic data = null) => EventManager.Instance.Dispatch(type,data);
    }
}