// author:KIPKIPS
// date:2023.02.11 21:50
// describe:Mono事件管理类
using System;
using Framework.Singleton;
using Unity.VisualScripting;
using UnityEngine;

namespace Framework.Manager {
    public class MonoManager:PersistentMonoSingleton<MonoManager> {
        public event Action UPDATE;
        public event Action FIXEDUPDATE;
        public event Action ONGUI;
        public event Action LATEUPDATE;
        private readonly string _logTag = "MonoManager";
        public void Launch() {
            Utils.Log(_logTag,"mono manager is start");
        }

        #region Unity生命周期函数

        private void Update() {
            UPDATE?.Invoke();
        }
        private void FixedUpdate() {
            FIXEDUPDATE?.Invoke();
        }
        private void OnGUI() {
            ONGUI?.Invoke();
        }
        private void LateUpdate() {
            LATEUPDATE?.Invoke();
        }

        #endregion

        #region Mono函数
        public void DontDestroy(UnityEngine.Object obj) {
            DontDestroyOnLoad(obj);
        }
        
        public Component AddUnityComponent<T>(Transform trs) where T:Component{
            return trs.AddComponent<T>();
        }
            
        #endregion
    }
}