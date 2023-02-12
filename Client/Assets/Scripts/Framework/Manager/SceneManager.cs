// author:KIPKIPS
// date:2023.02.12 01:29
// describe:场景管理器
using System;
using System.Collections;
using Framework.Singleton;
using UnityEngine;

namespace Framework.Manager {
    public class SceneManager: Singleton<SceneManager> {
        private readonly string _logTag = "SceneManager";

        public void Launch() {
            Utils.Log(_logTag, "scene manager is work");
        }
        /// <summary>
        /// 同步加载世界
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="callback">加载完成回调</param>
        public void LoadSceneSync(string sceneName,Action callback = null) {
            try {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                callback?.Invoke();
            } catch (Exception e) {
                Event.Dispatch(EventType.SCENE_FAILURE,0);
            }
        }
        
        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="callback">加载完成回调</param>
        public void LoadSceneAsync(string sceneName,Action callback = null) {
            try {
                this.StartCoroutine(DoLoadSceneAsync(sceneName,callback));
            } catch (Exception e) {
                Event.Dispatch(EventType.SCENE_FAILURE,0);
            }
        }

        private IEnumerator DoLoadSceneAsync(string sceneName,Action callback = null) {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!ao.isDone) {
                Event.Dispatch(EventType.SCENE_LOADING,ao.progress);
                yield return ao.progress;
            }
            Event.Dispatch(EventType.SCENE_LOAD_FINISHED,1);
            callback?.Invoke();
        }
    }
}