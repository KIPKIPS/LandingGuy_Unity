// author:KIPKIPS
// date:2023.02.11 21:50
// describe:Mono事件管理类
using System;
using Framework.Singleton;
using UnityEngine;

namespace Framework.Manager {
    // ReSharper disable InconsistentNaming
    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable MemberCanBeMadeStatic.Local
    // ReSharper disable MemberCanBeMadeStatic.Global
    // ReSharper disable EventNeverSubscribedTo.Global
    public class MonoManager:PersistentMonoSingleton<MonoManager> {
        public event Action UPDATE;
        public event Action FIXEDUPDATE;
        public event Action ONGUI;
        public event Action LATEUPDATE;
        private const string LOGTag = "MonoManager";
        public void Launch() {
            Utils.Log(LOGTag,"mono manager is start");
        }

        #region Unity生命周期函数

        private void Update() => UPDATE?.Invoke();
        private void FixedUpdate() => FIXEDUPDATE?.Invoke();
        private void OnGUI() => ONGUI?.Invoke();
        private void LateUpdate() => LATEUPDATE?.Invoke();

        #endregion

        #region Mono函数
        /// <summary>
        /// 切换场景时不销毁
        /// </summary>
        /// <param name="obj">不销毁的对象</param>
        public void UDontDestroyOnLoad(UnityEngine.Object obj) => DontDestroyOnLoad(obj);
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 添加组件
        /// </summary>
        /// <param name="trs">挂载的Transform组件</param>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>添加的组件</returns>
        public Component UAddComponent<T>(Transform trs) where T:Component => trs.gameObject.AddComponent<T>();
            
        #endregion
    }
}